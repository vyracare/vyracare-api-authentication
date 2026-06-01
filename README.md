# vyracare-api-authentication

## Visao geral

Esta API concentra os fluxos de autenticacao da plataforma Vyracare:

- registro de usuario;
- login;
- verificacao de primeiro acesso;
- definicao de senha no primeiro acesso;
- recuperacao de senha.

Ela foi organizada em um modelo de `vertical slice`, o que significa que cada caso de uso fica agrupado por feature, em vez de espalhado em pastas globais de controller, service e model.

---

## Como ler este projeto pela primeira vez

Se voce esta chegando agora, leia nesta ordem:

1. [Program.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Program.cs)
   Aqui voce entende como a API sobe, registra dependencias, configura JWT, CORS, Swagger e Lambda.

2. [Features/Auth/AuthController.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Features/Auth/AuthController.cs)
   Aqui voce ve quais endpoints existem e para qual handler cada rota delega.

3. Uma feature completa, por exemplo:
   - [RegisterRequest.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Features/Auth/Register/RegisterRequest.cs)
   - [RegisterHandler.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Features/Auth/Register/RegisterHandler.cs)
   - [RegisterResponse.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Features/Auth/Register/RegisterResponse.cs)

4. As portas do dominio:
   - [IUserRepository.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Features/Auth/Shared/Ports/IUserRepository.cs)
   - [IPasswordHasher.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Features/Auth/Shared/Ports/IPasswordHasher.cs)
   - [IJwtTokenGenerator.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Features/Auth/Shared/Ports/IJwtTokenGenerator.cs)

5. Os adapters de infraestrutura:
   - [MongoUserRepository.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Infrastructure/Persistence/MongoUserRepository.cs)
   - [Sha256PasswordHasher.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Infrastructure/Security/Sha256PasswordHasher.cs)
   - [JwtTokenGenerator.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Infrastructure/Security/JwtTokenGenerator.cs)

6. Os testes:
   - [LoginHandlerTests.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Vyracare.Auth.Tests/Auth/Login/LoginHandlerTests.cs)
   - [RegisterHandlerTests.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Vyracare.Auth.Tests/Auth/Register/RegisterHandlerTests.cs)

---

## Estrutura de pastas

### `Common`

Aqui ficam componentes que podem ser usados por varias features:

- `Configuration`
  Classes tipadas que leem configuracoes do `appsettings.json` e das variaveis de ambiente.
- `Http`
  Extensoes para transformar `UseCaseResult` em resposta HTTP.
- `Results`
  Contrato padrao de sucesso e erro dos handlers.
- `Time`
  Abstracao do tempo para facilitar testes.

### `Features/Auth`

Aqui ficam os casos de uso do dominio de autenticacao.

Cada pasta representa um fluxo:

- `Register`
- `Login`
- `FirstAccessCheck`
- `FirstAccessSetPassword`
- `ForgotPassword`

Em cada fluxo, a ideia e sempre a mesma:

1. Um `Request` define a entrada.
2. Um `Handler` implementa a regra de negocio.
3. Quando necessario, um `Response` define a saida.

### `Features/Auth/Shared`

Aqui ficam as pecas compartilhadas da feature:

- entidade de dominio `User`;
- interfaces que representam as portas de saida da aplicacao;
- respostas simples, como `MessageResponse`.

### `Infrastructure`

Aqui ficam os detalhes tecnicos que a regra de negocio nao deve conhecer diretamente:

- acesso ao MongoDB;
- geracao de token JWT;
- hash de senha;
- leitura de secrets da AWS;
- registro de dependencias no container.

### `Vyracare.Auth.Tests`

Projeto de testes unitarios.

Ele valida os handlers e componentes tecnicos isoladamente, sem depender de API Gateway, Lambda ou banco real.

---

## Fluxo passo a passo de uma requisicao

Vamos usar o login como exemplo.

1. O cliente faz `POST /api/auth/login`.
2. O controller recebe o body e cria um `LoginRequest`.
3. O controller resolve o `LoginHandler` via DI.
4. O handler consulta `IUserRepository`.
5. O handler valida a senha usando `IPasswordHasher`.
6. Se estiver tudo certo, usa `IJwtTokenGenerator`.
7. O handler devolve um `UseCaseResult<LoginResponse>`.
8. O controller transforma esse resultado em resposta HTTP.

Essa separacao existe para que:

- a regra de negocio possa ser testada sem banco e sem HTTP;
- a infraestrutura possa mudar sem quebrar os handlers;
- o codigo fique mais previsivel para evolucao.

---

## Endpoints

Base path:

- `/api/auth`

Rotas:

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/first-access/check`
- `POST /api/auth/first-access/set-password`
- `POST /api/auth/forgot-password`

Observacao:

- Essas rotas estao com `AllowAnonymous` porque sao a porta de entrada da autenticacao.
- O restante da API continua protegido por JWT.

---

## Seguranca e configuracao

### JWT

O JWT e configurado no [Program.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Program.cs) usando as opcoes de:

- issuer;
- audience;
- key.

### Secrets

Os valores sensiveis nao ficam versionados no repositorio.

Em runtime, a API tenta ler:

- `vyracare/shared/mongo`
- `vyracare/shared/jwt-signing`

Isso acontece em [SecretsManagerBootstrapper.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Infrastructure/SecretsManagerBootstrapper.cs).

### Fallbacks

Se o secret nao estiver disponivel, ainda existem fallbacks por variavel de ambiente:

- `MONGO_URI`
- `JWT_KEY`
- `JWT_ISSUER`
- `JWT_AUDIENCE`
- `CORS_ALLOWED_ORIGINS`

---

## Testes unitarios

### O que esta coberto hoje

- login com usuario inexistente;
- login com credenciais validas;
- registro com conflito;
- registro com sucesso;
- hash e validacao de senha.

### Como rodar

```bash
dotnet restore
dotnet build --no-restore
dotnet test Vyracare.Auth.Tests/Vyracare.Auth.Tests.csproj --no-restore
```

### Como pensar em novos testes

Quando voce criar um novo handler:

1. crie um arquivo de teste espelhando a pasta da feature;
2. use fakes das portas do dominio;
3. teste sucesso e falha;
4. evite depender de MongoDB real.

---

## Como adicionar um novo caso de uso

Exemplo: `ResetPassword`.

Passo a passo:

1. Criar a pasta `Features/Auth/ResetPassword`.
2. Criar o `ResetPasswordRequest`.
3. Criar o `ResetPasswordHandler`.
4. Reutilizar as portas existentes ou criar uma nova se necessario.
5. Expor a rota no `AuthController`.
6. Registrar o handler em `ServiceCollectionExtensions`.
7. Criar os testes em `Vyracare.Auth.Tests`.

---

## Como executar localmente

```bash
dotnet restore
dotnet build
dotnet run
```

Swagger:

- `/swagger/index.html`

---

## Como a API sobe em producao

1. O projeto e publicado via pipeline .NET.
2. A aplicacao sobe em AWS Lambda.
3. O API Gateway HTTP expõe as rotas.
4. O Swagger tambem fica publicado.
5. Os secrets sao lidos no startup.

---

## Resumo para desenvolvedores

Se voce lembrar de uma regra, lembre desta:

- controller recebe a request;
- handler executa a regra;
- porta define o contrato;
- infraestrutura implementa o contrato;
- testes validam o handler sem depender do mundo externo.
