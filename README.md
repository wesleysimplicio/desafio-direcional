# 🏢 Sistema Direcional - API de Gestão Imobiliária

Sistema completo desenvolvido em **.NET 9** para gerenciamento de clientes, apartamentos e vendas de uma empresa do setor imobiliário.

## ✅ Funcionalidades Implementadas

### 🔐 Autenticação e Autorização
- ✅ **JWT (JSON Web Token)** para autenticação
- ✅ **Middleware de autorização** protegendo todos os endpoints
- ✅ **Criptografia BCrypt** para senhas
- ✅ **Endpoints de login e registro**
- ✅ **Usuários padrão** para testes

### 👥 Gestão de Clientes (CRUD Completo)
- ✅ **Listar** todos os clientes
- ✅ **Obter** cliente por ID
- ✅ **Criar** novo cliente (com validação de CPF único)
- ✅ **Atualizar** dados do cliente
- ✅ **Excluir** cliente

### 🏢 Gestão de Apartamentos (CRUD Completo)
- ✅ **Listar** todos os apartamentos
- ✅ **Obter** apartamento por ID
- ✅ **Filtrar** apartamentos por status
- ✅ **Criar** novo apartamento
- ✅ **Atualizar** dados do apartamento
- ✅ **Excluir** apartamento

### 💰 Gestão de Vendas (CRUD Completo)
- ✅ **Listar** todas as vendas
- ✅ **Obter** venda por ID
- ✅ **Filtrar** vendas por cliente
- ✅ **Filtrar** vendas por status
- ✅ **Criar** nova venda (com validações)
- ✅ **Atualizar** dados da venda
- ✅ **Excluir** venda
- ✅ **Controle automático** de status do apartamento

## 🏗️ Arquitetura e Padrões

### Clean Architecture
```
📁 DirecionalApi.Web/          # 🎯 Apresentação (Controllers, Middleware)
📁 DirecionalApi.Application/  # 🧠 Aplicação (Services, DTOs)
📁 DirecionalApi.Infrastructure/ # 🔧 Infraestrutura (DbContext, Repositories)
📁 DirecionalApi.Domain/       # 💎 Domínio (Entities, Interfaces)
```

### Tecnologias e Padrões Implementados
- ✅ **Repository Pattern**
- ✅ **Dependency Injection**
- ✅ **Entity Framework Core 9.0**
- ✅ **DTOs (Data Transfer Objects)**
- ✅ **Service Layer Pattern**
- ✅ **RESTful API Design**
- ✅ **Swagger/OpenAPI Documentation**
- ✅ **CORS Policy**
- ✅ **Error Handling Global**

## 🗄️ Banco de Dados

### Estrutura das Tabelas
- ✅ **clientes** - Dados dos clientes
- ✅ **apartamentos** - Informações dos imóveis
- ✅ **vendas** - Transações de vendas
- ✅ **reservas** - Reservas de apartamentos
- ✅ **users** - Usuários do sistema

### Relacionamentos
- ✅ **1:N** Cliente → Vendas
- ✅ **1:N** Cliente → Reservas
- ✅ **1:N** Apartamento → Vendas
- ✅ **1:N** Apartamento → Reservas
- ✅ **N:1** Reserva → Venda (quando convertida)

## 🚀 Como Executar

### 🐳 Método 1: Docker Compose (Recomendado)
```bash
# Opção A: Script interativo
docker-manager.bat

# Opção B: Comando direto
docker-compose up -d --build

# URLs disponíveis:
# - API: http://localhost:5000
# - Swagger: http://localhost:5000/swagger
# - SQL Server: localhost:1433 (sa/DirecionalAPI@2024)
```

### 🛠️ Método 2: Script Local (.NET)
```bash
# Execute o script de inicialização
setup-and-run.bat
```

### ⚙️ Método 3: Manual

1. **Instalar .NET 9 SDK**
2. **Configurar SQL Server Express**
3. **Executar scripts do banco:**
   ```sql
   -- Execute: db/DDL_direcional.sql
   -- Execute: db/InsertUsers.sql
   ```
4. **Restaurar e executar:**
   ```bash
   cd back/DirecionalApi
   dotnet restore
   dotnet run --project DirecionalApi.Web
   ```

### 🔧 Método 4: Desenvolvimento com Hot-Reload
```bash
# Para desenvolvimento com auto-reload
docker-compose -f docker-compose.dev.yml up -d
```

> 📖 **Guia Completo**: Veja [DOCKER-GUIDE.md](DOCKER-GUIDE.md) para instruções detalhadas do Docker.

## 🔑 Credenciais de Teste

| Usuário | Senha | Perfil |
|---------|-------|--------|
| `admin` | `admin123` | Admin |
| `user` | `admin123` | User |

## 📖 Documentação da API

- **Swagger UI**: `https://localhost:7000/swagger`
- **Arquivo de exemplos**: `api-examples.http` (para VS Code REST Client)

### Endpoints Principais

```http
# Autenticação
POST /api/auth/login
POST /api/auth/register

# Clientes
GET    /api/clientes
POST   /api/clientes
GET    /api/clientes/{id}
PUT    /api/clientes/{id}
DELETE /api/clientes/{id}

# Apartamentos
GET    /api/apartamentos
POST   /api/apartamentos
GET    /api/apartamentos/{id}
PUT    /api/apartamentos/{id}
DELETE /api/apartamentos/{id}
GET    /api/apartamentos/status/{status}

# Vendas
GET    /api/vendas
POST   /api/vendas
GET    /api/vendas/{id}
PUT    /api/vendas/{id}
DELETE /api/vendas/{id}
GET    /api/vendas/cliente/{clienteId}
GET    /api/vendas/status/{status}
```

## ⚡ Exemplo de Uso Rápido

### 1. Login
```bash
curl -X POST "https://localhost:7000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

### 2. Listar Clientes (com token)
```bash
curl -X GET "https://localhost:7000/api/clientes" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 3. Criar Cliente
```bash
curl -X POST "https://localhost:7000/api/clientes" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "cpf": "12345678901",
    "email": "joao@email.com",
    "telefone": "11999999999"
  }'
```

## 📊 Status de Implementação

| Requisito | Status | Observações |
|-----------|--------|-------------|
| API .NET 9 | ✅ | Implementado com Clean Architecture |
| Entity Framework | ✅ | EF Core 9.0 com SQL Server |
| CRUD Clientes | ✅ | Completo com validações |
| CRUD Apartamentos | ✅ | Completo com filtros |
| CRUD Vendas | ✅ | Completo com regras de negócio |
| JWT Authentication | ✅ | Login/Register + middleware |
| Endpoints Protegidos | ✅ | Todos exceto auth endpoints |
| Documentação | ✅ | Swagger + README + exemplos |
| Banco de Dados | ✅ | Scripts SQL + migrations |
| Docker | ✅ | docker-compose completo |

## 🛡️ Validações e Regras de Negócio

- ✅ **CPF único** por cliente
- ✅ **Status automático** dos apartamentos (Disponível → Vendido)
- ✅ **Validação de apartamento disponível** antes da venda
- ✅ **Relacionamento** cliente-venda obrigatório
- ✅ **Criptografia** de senhas com BCrypt
- ✅ **Token JWT** com expiração de 8 horas

## 📁 Estrutura de Arquivos

```
📁 back/DirecionalApi/
├── 📁 DirecionalApi.Web/
│   ├── 📁 Controllers/         # ClientesController, ApartamentosController, VendasController, AuthController
│   ├── 📁 Extensions/          # DatabaseExtensions
│   ├── Program.cs              # Configuração principal
│   ├── appsettings.json        # Configurações
│   └── Dockerfile              # Container da API
├── 📁 DirecionalApi.Application/
│   ├── 📁 DTOs/               # CreateClienteDto, UpdateClienteDto, etc.
│   └── 📁 Services/           # ClienteService, ApartamentoService, VendaService, AuthService
├── 📁 DirecionalApi.Infrastructure/
│   ├── 📁 Data/               # DirecionalDbContext
│   ├── 📁 Repositories/       # ClienteRepository, ApartamentoRepository, etc.
│   └── 📁 Services/           # TokenService
└── 📁 DirecionalApi.Domain/
    ├── 📁 Entities/           # Cliente, Apartamento, Venda, Reserva, User
    └── 📁 Interfaces/         # IClienteRepository, ITokenService, etc.

📁 db/
├── DDL_direcional.sql         # Script de criação das tabelas
└── InsertUsers.sql            # Usuários padrão

📁 docs/
└── testes-analista-desenvolvedor-senior.pdf

📄 docker-compose.yml          # Infraestrutura completa
📄 setup-and-run.bat          # Script de inicialização
📄 api-examples.http          # Exemplos de requisições
📄 README.md                  # Este arquivo
```

## 💡 Próximos Passos (Sugestões)

- 🔄 **Migrations automáticas** no startup
- 📊 **Relatórios** de vendas
- 🔔 **Notificações** por email
- 📱 **Rate limiting**
- 📈 **Health checks**
- 🧪 **Testes unitários**
- 🔐 **Refresh tokens**
- 📦 **Versionamento da API**

---

✨ **API desenvolvida seguindo as melhores práticas de Clean Architecture, SOLID e RESTful APIs.**
