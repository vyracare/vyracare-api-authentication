# vyracare-api-authentication

API .NET 8 responsavel por registro, login, primeiro acesso e recuperacao de senha da plataforma Vyracare.

## Estrutura

O projeto foi reorganizado em um modelo pragmatico de `vertical slice` com `ports and adapters`.

- `Features/Auth`
  Casos de uso do dominio de autenticacao. Cada fluxo fica isolado em sua propria pasta (`Register`, `Login`, `FirstAccessCheck`, `FirstAccessSetPassword`, `ForgotPassword`).
- `Features/Auth/Shared`
  Entidade de dominio `User` e contratos de borda (`IUserRepository`, `IPasswordHasher`, `IJwtTokenGenerator`).
- `Common`
  Tipos compartilhados de configuracao, mapeamento HTTP, resultados de caso de uso e controle de tempo.
- `Infrastructure/Persistence`
  Adapter de MongoDB com documento `UserDocument` e repositorio `MongoUserRepository`.
- `Infrastructure/Security`
  Implementacoes de hash e emissao de JWT.
- `Infrastructure/DependencyInjection`
  Composicao do container e configuracao do acesso ao Mongo.

## Fluxo da request

1. O controller HTTP em `Features/Auth/AuthController.cs` recebe a requisicao.
2. O handler da feature executa validacoes e regras do caso de uso.
3. Os handlers dependem apenas de portas do dominio.
4. Os adapters de infraestrutura resolvem persistencia MongoDB e emissao de token JWT.

## Seguranca e configuracao

- JWT obrigatorio por default na aplicacao; apenas os endpoints de autenticacao estao marcados com `AllowAnonymous`.
- Secrets sensiveis nao ficam versionados.
- Em runtime, a API tenta carregar:
  - `vyracare/shared/mongo`
  - `vyracare/shared/jwt-signing`
- Tambem existem fallbacks para `MONGO_URI`, `JWT_KEY`, `JWT_ISSUER`, `JWT_AUDIENCE` e `CORS_ALLOWED_ORIGINS`.

Arquivos centrais:
- [Program.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Program.cs)
- [SecretsManagerBootstrapper.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Infrastructure/SecretsManagerBootstrapper.cs)
- [ServiceCollectionExtensions.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs)

## Testes unitarios

Existe uma camada dedicada em:

- [Vyracare.Auth.Tests.csproj](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Vyracare.Auth.Tests/Vyracare.Auth.Tests.csproj)

Cobertura inicial incluida:
- `LoginHandler`
- `RegisterHandler`
- `Sha256PasswordHasher`

Comando esperado:

```bash
dotnet test Vyracare.Auth.Tests/Vyracare.Auth.Tests.csproj
```

## Execucao local

```bash
dotnet restore
dotnet build
dotnet run
```

Para desenvolvimento local:
- configure `dotnet user-secrets`
- ou use as env vars de fallback

## Deploy

A API publica em AWS Lambda + HTTP API e expoe Swagger em:

- `/swagger/index.html`
- `/swagger/v1/swagger.json`

O deploy depende da esteira reutilizavel do repositório `vyracare-infra-pipes-dot-net`.
