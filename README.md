RAG Pipeline Learning Project
=============================

**Overview**
------------

This project is a learning exercise to demonstrate the implementation of a RAG (Retrieval-Augmented Generation)
pipeline.

**Technologies Used**
---------------------

* **ASP.NET Core**: Used as the API layer for interacting with the pipeline
* **Azure Functions**: Used as the serverless compute layer for embeddings generation
* **Azure Blob Storage Queue**: Used for storing and retrieving documents
* **PostgreSQL**: Used as the database layer for storing and retrieving data. **pgvector** extension for
  storing documents embedding
* **Ollama**: Used as the local LLM
  * **mxbai-embed-large** as embedding model
  * **llama3.1:8b** as chat model

**Getting Started**
-------------------

To get started, follow these steps:

1. Start all containers `docker compose up -d`
2. Download embedding model `docker exec -it ollama ollama run mxbai-embed-large`
3. Download chat model `docker exec -it ollama ollama run llama3.1:8b`
4. Double check downloaded models `docker exec -it ollama ollama list`
5. Execute EF migration `dotnet ef database update InitialCreate --project ./RAGShowCaseService.Infrastructure --startup-project ./RAGShowCaseService.Api`
