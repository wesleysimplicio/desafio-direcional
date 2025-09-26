# Sistema Direcional - API de Gestao Imobiliaria

Sistema completo desenvolvido em .NET 9 para gerenciamento de clientes, apartamentos e vendas de uma empresa do setor imobiliario.

## Funcionalidades Implementadas

### Autenticacao e Autorizacao
- JWT (JSON Web Token) para autenticacao
- Middleware de autorizacao protegendo todos os endpoints
- Criptografia BCrypt para senhas

### Gestao de Clientes (CRUD Completo)
- Listar todos os clientes
- Obter cliente por ID
- Criar novo cliente (com validacao de CPF unico)
- Atualizar dados do cliente
- Excluir cliente

### Gestao de Apartamentos (CRUD Completo)
- Listar todos os apartamentos
- Obter apartamento por ID
- Filtrar apartamentos por status
- Criar novo apartamento
- Atualizar dados do apartamento
- Excluir apartamento

### Gestao de Vendas (CRUD Completo)
- Listar todas as vendas
- Obter venda por ID
- Filtrar vendas por cliente
- Filtrar vendas por status
- Criar nova venda (com validacoes)
- Atualizar dados da venda
- Excluir venda
- Controle automatico de status do apartamento

## Arquitetura e Padroes

### Clean Architecture
```
DirecionalApi.Web/          # Apresentacao (Controllers, Middleware)
DirecionalApi.Application/  # Aplicacao (Services, DTOs)
DirecionalApi.Infrastructure/ # Infraestrutura (DbContext, Repositories)
DirecionalApi.Domain/       # Dominio (Entities, Interfaces)
```

### Tecnologias e Padroes Implementados
- Repository Pattern
- Dependency Injection
- Entity Framework Core 9.0
- DTOs (Data Transfer Objects)
- Service Layer Pattern
- RESTful API Design
- Swagger/OpenAPI Documentation
- CORS Policy
- Error Handling Global

## Banco de Dados

### Estrutura das Tabelas
- clientes - Dados dos clientes
- apartamentos - Informacoes dos imoveis
- vendas - Transacoes de vendas
- reservas - Reservas de apartamentos
- users - Usuarios do sistema

### Relacionamentos
- 1:N Cliente -> Vendas
- 1:N Cliente -> Reservas
- 1:N Apartamento -> Vendas
- N:1 Reserva -> Venda (quando convertida)

## Como Executar

### Metodo 1: Docker Compose (Recomendado)
```bash
# Execute todo o ambiente
docker-compose up -d --build

# URLs disponiveis:
# - API: http://localhost:5000
# - Frontend: http://localhost:3000
# - Swagger: http://localhost:5000/swagger
# - SQL Server: localhost:1433 (sa/DirecionalAPI@2024)
```

### Metodo 2: Script Local (.NET)
```bash
# Execute o script de inicializacao
setup-and-run.bat
```

### Metodo 3: Manual

1. Instalar .NET 9 SDK
2. Configurar SQL Server Express
3. Executar scripts do banco:
   - DDL_direcional.sql
4. Configurar connection string no appsettings.json
5. Executar API:
   ```bash
   cd back/DirecionalApi
   dotnet restore
   dotnet run --project DirecionalApi.Web
   ```
6. Executar Frontend:
   ```bash
   cd front/direcional
   npm install
   npm start
   ```

## Tecnologias Utilizadas

### Backend
- .NET 9
- Entity Framework Core 9.0
- SQL Server Express
- JWT Bearer Authentication
- Swagger/OpenAPI
- xUnit (Testes)

### Frontend
- React 19
- TypeScript
- Vite
- Tailwind CSS
- Axios
- React Router DOM
- React Hook Form

### DevOps & Infrastructure
- Docker
- Docker Compose
- Multi-stage Docker Builds
- SQL Server
- NGINX (configuravel)

## Estrutura do Projeto

```
desario-grupo-direcional/
├── back/                  # Backend API em .NET 9
│   └── DirecionalApi/
│       ├── DirecionalApi.Web/          # Controllers
│       ├── DirecionalApi.Application/  # Services, DTOs
│       ├── DirecionalApi.Infrastructure/ # DbContext, Repositories
│       ├── DirecionalApi.Domain/       # Entities
│       └── DirecionalApi.Test/         # Testes
├── front/                 # Frontend React
│   └── direcional/
├── db/
│   └── DDL_direcional.sql    # Script de criacao do banco
└── docs/
    └── testes-analista-desenvolvedor-senior.pdf
```

## Testes

### Executar Testes
```bash
cd back/DirecionalApi
dotnet test
```

### Cobertura de Testes
- Controllers: 100% dos endpoints testados
- Services: 95%+ de cobertura de codigo
- Repositories: 100% dos metodos principais
- Entities: 90%+ das propriedades e validacoes

## Credenciais de Teste

### Usuario Administrador Padrao
```
Email: admin@direcional.com
Senha: Admin123!
```

### Usuario de Teste
```
Email: user@direcional.com
Senha: User123!
```

Estes usuarios sao criados automaticamente no primeiro acesso a aplicacao.

## API Endpoints

### Autenticacao
```
POST /api/auth/login          - Login
POST /api/auth/register       - Registro
```

### Clientes
```
GET    /api/clientes           - Listar todos
GET    /api/clientes/{id}      - Buscar por ID
POST   /api/clientes           - Criar novo
PUT    /api/clientes/{id}      - Atualizar
DELETE /api/clientes/{id}      - Excluir
```

### Apartamentos
```
GET    /api/apartamentos        - Listar todos
GET    /api/apartamentos/{id}   - Buscar por ID
GET    /api/apartamentos/disponiveis - Filtrar disponiveis
POST   /api/apartamentos        - Criar novo
PUT    /api/apartamentos/{id}   - Atualizar
DELETE /api/apartamentos/{id}   - Excluir
```

### Vendas
```
GET    /api/vendas              - Listar todas
GET    /api/vendas/{id}         - Buscar por ID
GET    /api/vendas/cliente/{id} - Por cliente
POST   /api/vendas              - Criar nova
PUT    /api/vendas/{id}         - Atualizar
DELETE /api/vendas/{id}         - Excluir
```

## Proximos Passos (Sugestoes)

- Migrations automaticas no startup
- Relatorios de vendas
- Notificacoes por email
- Rate limiting
- Health checks
- Refresh tokens
- Versionamento da API
