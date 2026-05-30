vyracare-api-authentication (.NET 8) - MongoDB + JWT
-------------------------------------

Setup local:
  - Install .NET 8 SDK
  - Configure os secrets locais via `dotnet user-secrets` ou use as env vars `MONGO_URI` e `JWT_KEY`
  - dotnet restore
  - dotnet build
  - dotnet run

To publish:
  - dotnet publish -c Release -o ./publish
  - Em runtime, a API prioriza os secrets AWS `vyracare/shared/mongo` e `vyracare/shared/jwt-signing`
  - O `appsettings.json` versionado nao deve conter credenciais reais

--------------------------------------
