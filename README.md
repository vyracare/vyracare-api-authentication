Vyracare.Auth (.NET 8) - MongoDB + JWT
-------------------------------------

Setup local:
  - Install .NET 8 SDK
  - Configure a MongoDB Atlas cluster and set MONGO_URI env var or update backend/Vyracare.Auth/appsettings.json Mongo:ConnectionString
  - dotnet restore
  - dotnet run

To publish for Lambda (for Terraform):
  - dotnet publish -c Release -o ./publish
  - cd publish
  - zip -r ../lambda.zip .
  - move lambda.zip to infra/ or let Terraform's archive_file reference publish folder
