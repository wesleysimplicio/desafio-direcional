# Testes Unitarios e de Integracao

## Visao Geral

Este projeto conta com uma suite completa de testes que inclui:

- Testes Unitarios para Services, Entities e DTOs
- Testes de Integracao para Controllers
- Testes de Repositorio com banco de dados em memoria
- Cobertura de Autenticacao JWT
- Factory para Testes com configuracao de ambiente de teste

## Estrutura dos Testes

```
DirecionalApi.Test/
├── DirecionalApiWebApplicationFactory.cs    # Factory para testes de integracao
├── Unit/
│   ├── Application/
│   │   ├── AuthServiceTests.cs             # Testes do servico de autenticacao
│   │   ├── ClienteServiceTests.cs          # Testes do servico de clientes
│   │   ├── ApartamentoServiceTests.cs      # Testes do servico de apartamentos
│   │   └── VendaServiceTests.cs            # Testes do servico de vendas
│   ├── Domain/
│   │   └── EntitiesTests.cs                # Testes das entidades de dominio
│   ├── DTOs/
│   │   └── DTOTests.cs                     # Testes dos DTOs
│   └── Infrastructure/
│       └── RepositoryTests.cs              # Testes dos repositorios
└── Integration/
    ├── AuthControllerTests.cs              # Testes de integracao - autenticacao
    ├── ClientesControllerTests.cs          # Testes de integracao - clientes
    ├── ApartamentosControllerTests.cs      # Testes de integracao - apartamentos
    └── VendasControllerTests.cs            # Testes de integracao - vendas
```

## Tecnologias Utilizadas

- xUnit - Framework de testes
- Moq - Biblioteca para mocks
- FluentAssertions - Assertions mais legiveis
- AutoFixture - Geracao automatica de dados de teste
- InMemory Database - Banco de dados em memoria para testes
- ASP.NET Core Testing - Testes de integracao

## Como Executar os Testes

### Pre-requisitos

- .NET 9 SDK instalado
- Visual Studio 2022 ou VS Code

### Comandos para Execucao

1. Todos os testes:
```bash
dotnet test
```

2. Com relatorio detalhado:
```bash
dotnet test --logger "console;verbosity=detailed"
```

3. Apenas testes unitarios:
```bash
dotnet test --filter "FullyQualifiedName~Unit"
```

4. Apenas testes de integracao:
```bash
dotnet test --filter "FullyQualifiedName~Integration"
```

5. Com cobertura de codigo:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Cobertura de Testes

### Controllers (Integracao)
- AuthController - Login, registro, validacao de tokens
- ClientesController - CRUD completo com validacoes
- ApartamentosController - CRUD completo com filtros
- VendasController - CRUD completo com relacionamentos

### Services (Unitarios)
- AuthService - Autenticacao JWT, validacoes de usuario
- ClienteService - Operacoes de negocio para clientes
- ApartamentoService - Operacoes de negocio para apartamentos
- VendaService - Operacoes de negocio para vendas

### Entities (Unitarios)
- Cliente - Validacoes de propriedades e comportamentos
- Apartamento - Validacoes de propriedades e status
- Venda - Validacoes de relacionamentos e calculos

### DTOs (Unitarios)
- CreateDTO/UpdateDTO - Validacoes de entrada
- ResponseDTO - Estrutura de resposta
- AuthDTO - Login, registro e resposta de autenticacao

### Repositories (Unitarios)
- ClienteRepository - Operacoes CRUD e consultas especificas
- ApartamentoRepository - Operacoes CRUD e filtros
- VendaRepository - Operacoes CRUD e relacionamentos

## Cenarios de Teste

### Autenticacao
- Login com credenciais validas
- Login com credenciais invalidas
- Registro de novo usuario
- Validacao de token JWT
- Acesso a endpoints protegidos

### CRUD Operations
- Criacao com dados validos
- Criacao com dados invalidos
- Listagem com filtros
- Busca por ID existente/inexistente
- Atualizacao com dados validos/invalidos
- Exclusao de registros existentes/inexistentes

### Validacoes de Negocio
- CPF unico para clientes
- Email unico para clientes
- Apartamento unico por numero/bloco
- Status validos para entidades
- Relacionamentos entre vendas, clientes e apartamentos

### Edge Cases
- Dados nulos/vazios
- Strings vazias
- IDs inexistentes
- Concorrencia de operacoes
- Dados limítrofes

## Configuracao dos Testes

### Factory de Teste
```csharp
public class DirecionalApiWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o contexto real
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DirecionalContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Adiciona contexto em memoria
            services.AddDbContext<DirecionalContext>(options =>
                options.UseInMemoryDatabase("InMemoryDbForTesting"));
        });
    }
}
```

### Exemplo de Teste Unitario
```csharp
[Fact]
public async Task CreateClienteAsync_ValidData_ShouldReturnSuccess()
{
    // Arrange
    var cliente = _fixture.Create<CreateClienteDTO>();
    var expectedCliente = _fixture.Create<Cliente>();

    _mockRepository.Setup(r => r.GetByCPFAsync(cliente.CPF))
        .ReturnsAsync((Cliente)null);
    _mockRepository.Setup(r => r.AddAsync(It.IsAny<Cliente>()))
        .ReturnsAsync(expectedCliente);

    // Act
    var result = await _service.CreateClienteAsync(cliente);

    // Assert
    result.Should().NotBeNull();
    result.Nome.Should().Be(cliente.Nome);
}
```

## Integracao Continua

Os testes sao executados automaticamente em:
- Build local
- Pull Requests
- Pipeline de CI/CD
- Deploy para ambientes

## Notas Importantes

1. Banco em Memoria: Os testes usam Entity Framework InMemory para isolamento
2. Dados de Teste: AutoFixture gera dados aleatorios validos automaticamente
3. Cleanup: Cada teste e isolado com seu proprio contexto
4. Performance: Testes otimizados para execucao rapida
5. Manutenibilidade: Estrutura clara e reutilizavel
