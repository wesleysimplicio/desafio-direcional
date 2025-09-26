# Direcional API - Sistema de Gestão Imobiliária

API REST desenvolvida em .NET 9 para gerenciamento de clientes, apartamentos e vendas de uma empresa imobiliária.

## 🚀 Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **SQL Server Express** - Banco de dados
- **JWT (JSON Web Token)** - Autenticação e autorização
- **BCrypt** - Criptografia de senhas
- **Swagger/OpenAPI** - Documentação da API

## 📋 Funcionalidades

### 🔐 Autenticação
- **POST** `/api/auth/login` - Login com geração de token JWT
- **POST** `/api/auth/register` - Registro de novos usuários

### 👥 Clientes
- **GET** `/api/clientes` - Listar todos os clientes
- **GET** `/api/clientes/{id}` - Obter cliente por ID
- **POST** `/api/clientes` - Criar novo cliente
- **PUT** `/api/clientes/{id}` - Atualizar cliente
- **DELETE** `/api/clientes/{id}` - Excluir cliente

### 🏢 Apartamentos
- **GET** `/api/apartamentos` - Listar todos os apartamentos
- **GET** `/api/apartamentos/{id}` - Obter apartamento por ID
- **GET** `/api/apartamentos/status/{status}` - Filtrar por status
- **POST** `/api/apartamentos` - Criar novo apartamento
- **PUT** `/api/apartamentos/{id}` - Atualizar apartamento
- **DELETE** `/api/apartamentos/{id}` - Excluir apartamento

### 💰 Vendas
- **GET** `/api/vendas` - Listar todas as vendas
- **GET** `/api/vendas/{id}` - Obter venda por ID
- **GET** `/api/vendas/cliente/{clienteId}` - Vendas por cliente
- **GET** `/api/vendas/status/{status}` - Filtrar por status
- **POST** `/api/vendas` - Criar nova venda
- **PUT** `/api/vendas/{id}` - Atualizar venda
- **DELETE** `/api/vendas/{id}` - Excluir venda

## 🛠️ Configuração e Execução

### Pré-requisitos
1. **.NET 9 SDK** instalado
2. **SQL Server Express** ou SQL Server
3. **Visual Studio 2022** ou VS Code

### Configuração do Banco de Dados

1. Execute o script DDL no SQL Server:
```sql
-- Execute o arquivo: db/DDL_direcional.sql
```

2. Insira os usuários padrão:
```sql
-- Execute o arquivo: db/InsertUsers.sql
```

### Configuração da Connection String

Atualize o arquivo `appsettings.json` com sua connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=DirecionalDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Executando a Aplicação

1. Navegue até o diretório do projeto:
```bash
cd back/DirecionalApi/DirecionalApi.Web
```

2. Restaure os pacotes:
```bash
dotnet restore
```

3. Execute as migrações (se necessário):
```bash
dotnet ef database update
```

4. Execute a aplicação:
```bash
dotnet run
```

A API estará disponível em: `https://localhost:7000` ou `http://localhost:5000`

## 📖 Documentação da API

A documentação completa da API está disponível via Swagger UI em:
- **Desenvolvimento**: `https://localhost:7000/swagger`

## 🔑 Autenticação

A API utiliza JWT (JSON Web Token) para autenticação. Todos os endpoints, exceto `/api/auth/login` e `/api/auth/register`, requerem autenticação.

### Usuários Padrão

| Usuário | Senha | Perfil |
|---------|-------|--------|
| admin   | admin123 | Admin |
| user    | admin123 | User |

### Como Autenticar

1. **Fazer login**:
```bash
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

2. **Usar o token** nas requisições:
```bash
Authorization: Bearer {token}
```

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**:

```
DirecionalApi.Web/          # Controllers e configuração
├── Controllers/            # Endpoints da API
└── Program.cs             # Configuração da aplicação

DirecionalApi.Application/  # Lógica de aplicação
├── DTOs/                  # Data Transfer Objects
└── Services/             # Serviços de aplicação

DirecionalApi.Infrastructure/ # Infraestrutura
├── Data/                 # Entity Framework DbContext
├── Repositories/         # Implementação dos repositórios
└── Services/            # Serviços de infraestrutura

DirecionalApi.Domain/      # Regras de negócio
├── Entities/            # Entidades de domínio
└── Interfaces/         # Contratos/Interfaces
```

## 📊 Modelo de Dados

### Principais Entidades

- **Cliente**: Informações dos clientes (nome, CPF, contato, etc.)
- **Apartamento**: Detalhes dos apartamentos (área, quartos, valor, etc.)
- **Venda**: Transações de vendas (valor, forma de pagamento, parcelas, etc.)
- **Reserva**: Reservas de apartamentos
- **User**: Usuários do sistema (autenticação)

### Status Disponíveis

**Cliente**: Ativo, Inativo, Prospecto
**Apartamento**: Disponível, Reservado, Vendido, Indisponível
**Venda**: Ativa, Quitada, Cancelada, Inadimplente
**Reserva**: Ativa, Convertida, Cancelada, Expirada

## 🧪 Testando a API

### Exemplo de Requisições

**1. Login**:
```bash
curl -X POST "https://localhost:7000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"admin\",\"password\":\"admin123\"}"
```

**2. Listar Clientes**:
```bash
curl -X GET "https://localhost:7000/api/clientes" \
  -H "Authorization: Bearer {seu-token}"
```

**3. Criar Cliente**:
```bash
curl -X POST "https://localhost:7000/api/clientes" \
  -H "Authorization: Bearer {seu-token}" \
  -H "Content-Type: application/json" \
  -d "{
    \"nome\": \"João Silva\",
    \"cpf\": \"12345678901\",
    \"email\": \"joao@email.com\",
    \"telefone\": \"11999999999\"
  }"
```

## 🔧 Configurações Avançadas

### JWT Settings
```json
{
  "Jwt": {
    "SecretKey": "YourVeryLongSecretKeyForJWTTokenGeneration123456789",
    "Issuer": "DirecionalApi",
    "Audience": "DirecionalApi"
  }
}
```

### CORS
A API está configurada para aceitar requisições de qualquer origem em desenvolvimento. Para produção, configure adequadamente as origens permitidas.

## 📝 Logs e Monitoramento

A aplicação utiliza o sistema de logs padrão do .NET. Os logs são configurados no `appsettings.json`.

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👨‍💻 Desenvolvedor

Desenvolvido para o Grupo Direcional - Sistema de Gestão Imobiliária

---

**Nota**: Certifique-se de alterar as configurações de segurança (chaves JWT, connection strings) antes de usar em produção.
