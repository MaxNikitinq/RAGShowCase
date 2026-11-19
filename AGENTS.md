# RAGShowCase — Agent Instructions

RAG pipeline learning project: ASP.NET Core API + Azure Functions document processor,
PostgreSQL/pgvector storage, Azure Blob/Queue, Ollama for embeddings and chat.

## Architecture Contract (Clean Architecture)

Dependency flow — dependencies point inward, and this is enforced:

```
Api ──► Application ──► Core
 │           ▲            ▲
 └──────────► Infrastructure ──► Core
```

| Layer | Project | May reference | Must NOT reference |
| --- | --- | --- | --- |
| Composition root | `RAGShowCaseService.Api` | Application, Infrastructure | — |
| Application | `RAGShowCaseService.Application` | Core | Infrastructure, Api |
| Domain | `RAGShowCaseService.Core` | nothing | Application, Infrastructure, Api |
| Infrastructure | `RAGShowCaseService.Infrastructure` | Application, Core | Api |
| Worker | `RAGShowCaseService.DocumentProcessor` | no project references | any project (see below) |

Rules that apply to every change:

- **Api is the only layer that references Infrastructure.** Never add an
  Infrastructure reference to Application or Core.
- **Abstractions live in Application** (`Application/Abstractions/**`) and Core
  (`Core/Repositories/**`); Infrastructure provides the implementations and wires
  them in `Infrastructure/DependencyInjection.cs`.
- **Core contains entities and repository interfaces only.** No services, no
  external SDKs, no I/O.
- **EF Core stays in Infrastructure** (`Data/`, `Data/Configuration`, `Migrations/`).
  No `DbContext`, no EF types in Api/Application/Core code.
- **`DocumentProcessor` is a standalone Azure Functions worker.** It deliberately
  has no project references — it talks to the rest of the system through queues,
  blobs, and PostgreSQL (Dapper). Do not add project references to it.
- DI is registered via a per-layer `DependencyInjection.cs` extension method
  (`AddApplication()`, `AddInfrastructure(configuration)`), called from `Api/Program.cs`.
- Nullable reference types and implicit usings are enabled in every project — keep them.

## Project Map

- `RAGShowCaseService.Api` — controllers (`Controllers/`), `Program.cs` composition root, Swagger.
- `RAGShowCaseService.Application` — `Features/<Feature>/Services` (+ `Services/Abstractions`
  for feature interfaces), cross-cutting ports in `Abstractions/` (BlobStorage, QueueStorage,
  Embeddings, ChatClient, UnitOfWork).
- `RAGShowCaseService.Core` — `Entities/PrivateDocument`, `Entities/PrivateDocumentEmbedding`,
  repository interfaces.
- `RAGShowCaseService.Infrastructure` — Azure Blob/Queue clients, Ollama embeddings/chat clients,
  EF Core `ApplicationDbContext` + repositories + migrations.
- `RAGShowCaseService.DocumentProcessor` — queue-triggered function: text extraction, chunking,
  embedding insertion.
- `RAGShowCaseService.Tests` — xUnit test project. References **Application and Core only**;
  Infrastructure is exercised separately (docker-compose services), not from unit tests.

## Commands

```bash
dotnet build RAGShowCase.slnx          # build the whole solution
dotnet test                            # run all tests
dotnet test --filter "FullyQualifiedName~PrivateDocumentTests"
```

EF migration (from repo root):

```bash
dotnet ef database update InitialCreate --project ./RAGShowCaseService.Infrastructure --startup-project ./RAGShowCaseService.Api
```

## Testing Conventions

- xUnit, strict **Arrange / Act / Assert** with a comment banner for each phase.
- One test class per unit under test, named `<UnitUnderTest>Tests`, mirroring the source
  namespace (e.g. `RAGShowCaseService.Tests.Core.Entities.PrivateDocumentTests`).
- Test naming: `MethodName_Scenario_ExpectedResult` (e.g. `Deactivate_Always_SetsIsActiveToFalse`).
- Tests of Application services fake the ports from `Application/Abstractions` with
  hand-written fakes — the test project must not reference Infrastructure.
- Never report a test as passing without having run `dotnet test` yourself.

## Agent Harness

Custom subagents live in `.agents/agents/` (project scope, committed to the repo):

- **`test-writer`** — writes xUnit tests in the Arrange/Act/Assert style above. Delegate when
  new features/services need coverage or when a change needs regression tests.
- **`code-reviewer`** — read-only review of a diff or file set, with explicit Clean
  Architecture checks. Delegate before merging non-trivial changes; it reports
  severity-ranked findings and never edits code.

### Mandatory completion workflow

When you (the main agent) have finished implementing a feature — a new endpoint, service,
feature method, or behavior change — do NOT report completion until you have:

1. Dispatched `test-writer` to cover the new/changed behavior with AAA xUnit tests, and
   confirmed the tests it added pass (`dotnet test` green).
2. Dispatched `code-reviewer` on the full diff of the feature, and applied or explicitly
   rejected (with reasons given to the user) every Critical and Should-fix finding.

Report the feature as done only after both steps. Skip the workflow only when the change
is genuinely non-behavioral (config, docs, renaming) — and say that you skipped it.
