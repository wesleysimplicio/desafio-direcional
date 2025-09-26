using AutoFixture;
using DirecionalApi.Application.DTOs;

namespace DirecionalApi.Test.Unit.DTOs;

public class ClienteDTOTests
{
    private readonly Fixture _fixture;

    public ClienteDTOTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void CreateClienteDTO_ShouldAcceptValidData()
    {
        // Arrange
        var dto = _fixture.Build<CreateClienteDTO>()
            .With(c => c.Nome, "João Silva")
            .With(c => c.CPF, "123.456.789-00")
            .With(c => c.Email, "joao@email.com")
            .Create();

        // Act & Assert
        dto.Nome.Should().Be("João Silva");
        dto.CPF.Should().Be("123.456.789-00");
        dto.Email.Should().Be("joao@email.com");
    }

    [Fact]
    public void UpdateClienteDTO_ShouldAcceptValidData()
    {
        // Arrange
        var dto = _fixture.Build<UpdateClienteDTO>()
            .With(c => c.Nome, "Maria Santos")
            .With(c => c.Email, "maria@email.com")
            .Create();

        // Act & Assert
        dto.Nome.Should().Be("Maria Santos");
        dto.Email.Should().Be("maria@email.com");
    }

    [Fact]
    public void ClienteResponseDTO_ShouldContainAllProperties()
    {
        // Arrange & Act
        var dto = _fixture.Create<ClienteResponseDTO>();

        // Assert
        dto.Should().NotBeNull();
        dto.Should().HaveProperty("Id");
        dto.Should().HaveProperty("Nome");
        dto.Should().HaveProperty("CPF");
        dto.Should().HaveProperty("Email");
        dto.Should().HaveProperty("StatusCliente");
        dto.Should().HaveProperty("DataCadastro");
    }

    [Theory]
    [InlineData("Ativo")]
    [InlineData("Inativo")]
    [InlineData("Prospecto")]
    public void ClienteResponseDTO_ShouldAcceptValidStatus(string status)
    {
        // Arrange & Act
        var dto = _fixture.Build<ClienteResponseDTO>()
            .With(c => c.StatusCliente, status)
            .Create();

        // Assert
        dto.StatusCliente.Should().Be(status);
    }
}

public class ApartamentoDTOTests
{
    private readonly Fixture _fixture;

    public ApartamentoDTOTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void CreateApartamentoDTO_ShouldAcceptValidData()
    {
        // Arrange
        var dto = _fixture.Build<CreateApartamentoDTO>()
            .With(a => a.Numero, "101")
            .With(a => a.Bloco, "A")
            .With(a => a.Quartos, 2)
            .With(a => a.Preco, 250000m)
            .Create();

        // Act & Assert
        dto.Numero.Should().Be("101");
        dto.Bloco.Should().Be("A");
        dto.Quartos.Should().Be(2);
        dto.Preco.Should().Be(250000m);
    }

    [Fact]
    public void UpdateApartamentoDTO_ShouldAcceptValidData()
    {
        // Arrange
        var dto = _fixture.Build<UpdateApartamentoDTO>()
            .With(a => a.Preco, 300000m)
            .With(a => a.StatusApartamento, "Vendido")
            .Create();

        // Act & Assert
        dto.Preco.Should().Be(300000m);
        dto.StatusApartamento.Should().Be("Vendido");
    }

    [Fact]
    public void ApartamentoResponseDTO_ShouldContainAllProperties()
    {
        // Arrange & Act
        var dto = _fixture.Create<ApartamentoResponseDTO>();

        // Assert
        dto.Should().NotBeNull();
        dto.Should().HaveProperty("Id");
        dto.Should().HaveProperty("Numero");
        dto.Should().HaveProperty("Bloco");
        dto.Should().HaveProperty("StatusApartamento");
        dto.Should().HaveProperty("Preco");
        dto.Should().HaveProperty("DataCadastro");
    }

    [Theory]
    [InlineData("Disponível")]
    [InlineData("Reservado")]
    [InlineData("Vendido")]
    public void ApartamentoResponseDTO_ShouldAcceptValidStatus(string status)
    {
        // Arrange & Act
        var dto = _fixture.Build<ApartamentoResponseDTO>()
            .With(a => a.StatusApartamento, status)
            .Create();

        // Assert
        dto.StatusApartamento.Should().Be(status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CreateApartamentoDTO_ShouldAcceptValidQuartos(int quartos)
    {
        // Arrange & Act
        var dto = _fixture.Build<CreateApartamentoDTO>()
            .With(a => a.Quartos, quartos)
            .Create();

        // Assert
        dto.Quartos.Should().Be(quartos);
    }
}

public class VendaDTOTests
{
    private readonly Fixture _fixture;

    public VendaDTOTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void CreateVendaDTO_ShouldAcceptValidData()
    {
        // Arrange
        var dto = _fixture.Build<CreateVendaDTO>()
            .With(v => v.ClienteId, 1)
            .With(v => v.ApartamentoId, 1)
            .With(v => v.ValorTotal, 250000m)
            .With(v => v.DataVenda, DateTime.UtcNow.Date)
            .Create();

        // Act & Assert
        dto.ClienteId.Should().Be(1);
        dto.ApartamentoId.Should().Be(1);
        dto.ValorTotal.Should().Be(250000m);
        dto.DataVenda.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void UpdateVendaDTO_ShouldAcceptValidData()
    {
        // Arrange
        var dto = _fixture.Build<UpdateVendaDTO>()
            .With(v => v.StatusVenda, "Confirmada")
            .With(v => v.ValorEntrada, 50000m)
            .Create();

        // Act & Assert
        dto.StatusVenda.Should().Be("Confirmada");
        dto.ValorEntrada.Should().Be(50000m);
    }

    [Fact]
    public void VendaResponseDTO_ShouldContainAllProperties()
    {
        // Arrange & Act
        var dto = _fixture.Create<VendaResponseDTO>();

        // Assert
        dto.Should().NotBeNull();
        dto.Should().HaveProperty("Id");
        dto.Should().HaveProperty("ClienteId");
        dto.Should().HaveProperty("ApartamentoId");
        dto.Should().HaveProperty("ValorTotal");
        dto.Should().HaveProperty("StatusVenda");
        dto.Should().HaveProperty("DataVenda");
        dto.Should().HaveProperty("DataCadastro");
    }

    [Theory]
    [InlineData("Pendente")]
    [InlineData("Confirmada")]
    [InlineData("Cancelada")]
    public void VendaResponseDTO_ShouldAcceptValidStatus(string status)
    {
        // Arrange & Act
        var dto = _fixture.Build<VendaResponseDTO>()
            .With(v => v.StatusVenda, status)
            .Create();

        // Assert
        dto.StatusVenda.Should().Be(status);
    }

    [Theory]
    [InlineData(100000)]
    [InlineData(250000)]
    [InlineData(500000)]
    public void CreateVendaDTO_ShouldAcceptValidValues(decimal valor)
    {
        // Arrange & Act
        var dto = _fixture.Build<CreateVendaDTO>()
            .With(v => v.ValorTotal, valor)
            .Create();

        // Assert
        dto.ValorTotal.Should().Be(valor);
    }

    [Fact]
    public void CreateVendaDTO_ShouldAllowOptionalFields()
    {
        // Arrange & Act
        var dto = new CreateVendaDTO
        {
            ClienteId = 1,
            ApartamentoId = 1,
            ValorTotal = 250000,
            DataVenda = DateTime.UtcNow.Date,
            ValorEntrada = null,
            NumeroParcelasFinanciamento = null,
            ValorParcelasFinanciamento = null,
            Observacoes = null
        };

        // Assert
        dto.ValorEntrada.Should().BeNull();
        dto.NumeroParcelasFinanciamento.Should().BeNull();
        dto.ValorParcelasFinanciamento.Should().BeNull();
        dto.Observacoes.Should().BeNull();
    }
}

public class AuthDTOTests
{
    private readonly Fixture _fixture;

    public AuthDTOTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void LoginDTO_ShouldAcceptValidCredentials()
    {
        // Arrange & Act
        var dto = new LoginDTO
        {
            Email = "admin@direcional.com",
            Password = "Admin123!"
        };

        // Assert
        dto.Email.Should().Be("admin@direcional.com");
        dto.Password.Should().Be("Admin123!");
    }

    [Fact]
    public void RegisterDTO_ShouldAcceptValidData()
    {
        // Arrange & Act
        var dto = _fixture.Build<RegisterDTO>()
            .With(r => r.Email, "newuser@direcional.com")
            .With(r => r.Password, "Password123!")
            .With(r => r.Nome, "New User")
            .Create();

        // Assert
        dto.Email.Should().Be("newuser@direcional.com");
        dto.Password.Should().Be("Password123!");
        dto.Nome.Should().Be("New User");
    }

    [Fact]
    public void AuthResponseDTO_ShouldContainTokenAndUserInfo()
    {
        // Arrange & Act
        var dto = _fixture.Build<AuthResponseDTO>()
            .With(a => a.Token, "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")
            .With(a => a.Email, "user@direcional.com")
            .With(a => a.Nome, "User Name")
            .Create();

        // Assert
        dto.Token.Should().NotBeNullOrEmpty();
        dto.Email.Should().Be("user@direcional.com");
        dto.Nome.Should().Be("User Name");
    }

    [Theory]
    [InlineData("user@domain.com")]
    [InlineData("test.email@subdomain.example.org")]
    [InlineData("admin@direcional.com")]
    public void LoginDTO_ShouldAcceptValidEmails(string email)
    {
        // Arrange & Act
        var dto = new LoginDTO
        {
            Email = email,
            Password = "Password123!"
        };

        // Assert
        dto.Email.Should().Be(email);
    }
}
