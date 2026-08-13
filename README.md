# vyracare-api-authentication

## Visao geral

Esta API concentra os fluxos de autenticacao da plataforma Vyracare:

- registro de usuario;
- login;
- verificacao de primeiro acesso;
- definicao de senha no primeiro acesso;
- recuperacao de senha.

Ela foi organizada em um modelo de `vertical slice`, ou seja, cada caso de uso fica agrupado por feature, em vez de espalhado em pastas globais como `Controllers`, `Services`, `Models` e `DTOs`.

Runtime atual da aplicacao:

- `TargetFramework`: `net10.0`
- runtime AWS Lambda: `dotnet10`

---

## Como ler este projeto pela primeira vez

Se voce esta chegando agora, leia nesta ordem:

1. [Program.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Program.cs)  
   Aqui voce entende como a API sobe, registra dependencias, configura JWT, CORS, Swagger e o adaptador para AWS Lambda.

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
   - [ParameterStoreBootstrapper.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Infrastructure/ParameterStoreBootstrapper.cs)

6. Os testes:
   - [LoginHandlerTests.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Vyracare.Auth.Tests/Auth/Login/LoginHandlerTests.cs)
   - [RegisterHandlerTests.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Vyracare.Auth.Tests/Auth/Register/RegisterHandlerTests.cs)
   - [Sha256PasswordHasherTests.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Vyracare.Auth.Tests/Infrastructure/Security/Sha256PasswordHasherTests.cs)

---

## Estrutura de pastas

### `Common`

Aqui ficam componentes compartilhados por mais de uma feature:

- `Configuration`  
  Classes tipadas que leem configuracoes do `appsettings.json` e das variaveis de ambiente.
- `Http`  
  Extensoes para transformar `UseCaseResult` em resposta HTTP.
- `Results`  
  Contrato padrao de sucesso e erro usado pelos handlers.
- `Time`  
  Abstracao de tempo para facilitar testes e regras que dependem de data.

### `Features/Auth`

Aqui ficam os casos de uso do dominio de autenticacao.

Cada pasta representa um fluxo:

- `Register`
- `Login`
- `FirstAccessCheck`
- `FirstAccessSetPassword`
- `ForgotPassword`

Em cada fluxo, a ideia e sempre a mesma:

1. um `Request` define a entrada;
2. um `Handler` implementa a regra de negocio;
3. quando necessario, um `Response` define a saida.

### `Features/Auth/Shared`

Aqui ficam as pecas compartilhadas da feature:

- entidade de dominio `User`;
- interfaces que representam portas de saida da aplicacao;
- respostas simples como `MessageResponse`.

### `Infrastructure`

Aqui ficam os detalhes tecnicos que a regra de negocio nao deve conhecer diretamente:

- acesso ao MongoDB;
- geracao de token JWT;
- hash de senha;
- leitura de parametros seguros da AWS;
- registro de dependencias no container.

### `Vyracare.Auth.Tests`

Projeto de testes unitarios.

Ele valida handlers e componentes tecnicos isoladamente, sem depender de API Gateway, Lambda ou banco real.

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

Observacoes:

- essas rotas estao com `AllowAnonymous`, porque sao a porta de entrada da autenticacao;
- o restante da plataforma consome o token gerado aqui para acessar APIs protegidas.

---

## Seguranca e configuracao

### JWT

O JWT e configurado no [Program.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Program.cs) usando as opcoes de:

- `Issuer`;
- `Audience`;
- `ExpiryMinutes`;
- `Key` carregada via Parameter Store ou fallback.

As configuracoes base versionadas hoje estao em [appsettings.json](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/appsettings.json).

### Parameter Store

Os valores sensiveis nao ficam versionados no repositorio.

Em runtime, a API usa nomes de parametro diferentes para cada ambiente:

- `prod`
  - `vyracare/shared/mongo-prod`
  - `vyracare/shared/jwt-signing-prod`
- `hml`
  - `vyracare/shared/mongo-hml`
  - `vyracare/shared/jwt-signing-hml`
- `dev`
  - `vyracare/shared/mongo-dev`
  - `vyracare/shared/jwt-signing-dev`

Isso acontece em [ParameterStoreBootstrapper.cs](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/Infrastructure/ParameterStoreBootstrapper.cs).

### Banco de dados por ambiente

A autenticacao nao usa o mesmo nome de banco em `dev` e `prod`.

Regra atual:

- `main` publica usando `vyracare_db`
- `develop` publica usando `vyracare_db_dev`

A connection string pode ser a mesma, mas o banco selecionado muda por variavel de ambiente da Lambda.

### Fallbacks

Se o parametro nao estiver disponivel, ainda existem fallbacks por variavel de ambiente:

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
- hash e validacao de senha;
- partes de infraestrutura de seguranca que podem ser testadas isoladamente.

### Como rodar

```bash
dotnet restore
dotnet build --no-restore
dotnet test Vyracare.Auth.Tests/Vyracare.Auth.Tests.csproj --no-restore
```

### Como pensar em novos testes

Quando voce criar um novo handler:

1. crie um arquivo de teste espelhando a pasta da feature;
2. use fakes ou mocks das portas do dominio;
3. teste sucesso e falha;
4. evite depender de MongoDB real.

---

## Como adicionar um novo caso de uso

Exemplo: `ResetPassword`.

Passo a passo:

1. criar a pasta `Features/Auth/ResetPassword`;
2. criar o `ResetPasswordRequest`;
3. criar o `ResetPasswordHandler`;
4. reutilizar as portas existentes ou criar uma nova se necessario;
5. expor a rota no `AuthController`;
6. registrar o handler em `ServiceCollectionExtensions`;
7. criar os testes em `Vyracare.Auth.Tests`.

---

## Como executar localmente

```bash
dotnet restore
dotnet build
dotnet run
```

Swagger:

- `/swagger/index.html`

Para desenvolvimento local, o ideal e fornecer os valores sensiveis por:

- `dotnet user-secrets`; ou
- variaveis de ambiente.

---

## Como a API sobe em dev e prod

### Pipeline

O workflow de publicacao esta em [.github/workflows/publish.yml](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/.github/workflows/publish.yml).

Regra atual:

- `push` em `develop` publica em `dev`
- `push` em `release/*` publica em `hml`
- `push` em `main` publica em `prod`

### Recursos AWS da autenticacao

A esteira reutilizavel da auth cria e atualiza recursos com nomes padronizados:

- Lambda `prod`
  - `vyracare-api-authentication`
- Lambda `dev`
  - `vyracare-api-authentication-dev`
- API Gateway `prod`
  - `vyracare-api-authentication`
- API Gateway `dev`
  - `vyracare-api-authentication-dev`

### O que muda por ambiente

Em `develop`:

- Lambda com sufixo `-dev`
- API Gateway com sufixo `-dev`
- banco `vyracare_db_dev`
- parametros `*-dev`

Em `release/*`:

- Lambda com sufixo `-hml`
- API Gateway com sufixo `-hml`
- banco `vyracare_db_hml`
- parametros `*-hml`

Em `main`:

- Lambda sem sufixo
- API Gateway sem sufixo
- banco `vyracare_db`
- parametros `*-prod`

---

## Integracao automatica com frontend consumidor

Esta API publica metadados em [.vyracare/mfe-consumer.json](C:/Users/lenin/OneDrive/Desktop/GitHub/Vyracare/vyracare-api-authentication/.vyracare/mfe-consumer.json).

Hoje o consumidor configurado e:

- `vyracare/vyracare-app-shell`

Quando o deploy termina, a esteira atualiza automaticamente no shell:

- `apiUrl`

nos arquivos:

- `src/environments/environments.dev.ts`
- `src/environments/environments.hml.ts`
- `src/environments/environments.prod.ts`

O arquivo `src/environments/environments.ts` fica reservado para desenvolvimento local.

Isso evita ficar trocando manualmente a URL do API Gateway sempre que a autenticacao muda.

---

## Resumo para desenvolvedores

Se voce lembrar de uma regra, lembre desta:

- o controller recebe a request;
- o handler executa a regra;
- a porta define o contrato;
- a infraestrutura implementa o contrato;
- os testes validam o handler sem depender do mundo externo;
- a esteira separa `dev`, `hml` e `prod` por nome de recurso, parametro e banco.

## Convencao de commits

Os commits deste repositorio devem ser escritos em portugues.

Padrao recomendado:

- `feat: adiciona validacao de primeiro acesso`
- `fix: corrige leitura de parametro da autenticacao`
- `docs: atualiza explicacao do fluxo de homologacao`
