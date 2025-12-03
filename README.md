vyracare-api-authentication (.NET 8) - MongoDB + JWT
-------------------------------------

Setup local:
  - Install .NET 8 SDK
  - Configure a MongoDB Atlas cluster and set MONGO_URI env var or update vyracare-api-authentication/appsettings.json Mongo:ConnectionString
  - dotnet restore
  - dotnet build
  - dotnet run

To publish:
  - dotnet publish -c Release -o ./publish

-------------------------------------