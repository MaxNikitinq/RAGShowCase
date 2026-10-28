dotnet ef migrations add InitialCreate --project ./RAGShowCaseService.Infrastructure --startup-project ./RAGShowCaseService.Api
dotnet ef database update InitialCreate --project ./RAGShowCaseService.Infrastructure --startup-project ./RAGShowCaseService.Api 
docker exec -it ollama ollama run mxbai-embed-large
docker exec -it ollama ollama run llama3.1:8b
docker exec -it ollama ollama list