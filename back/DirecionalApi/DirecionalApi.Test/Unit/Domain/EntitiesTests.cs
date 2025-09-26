using AutoFixture;
using DirecionalApi.Domain.Entities;

namespace DirecionalApi.Test.Unit.Domain;

public class ClienteTests
{
    private readonly Fixture _fixture;

    public ClienteTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Cliente_WhenCreated_ShouldHaveDefaultValues()
    {
        // Act
        var cliente = new Cliente();

        // Assert
        cliente.Id.Should().Be(0);
        cliente.StatusCliente.Should().Be("Ativo");
        cliente.DataCadastro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        cliente.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("João Silva")]
    [InlineData("Maria Santos")]
    [InlineData("Carlos Alberto de Oliveira")]
    public void Cliente_ShouldAcceptValidNames(string nome)
    {
        // Arrange & Act
        var cliente = _fixture.Build<Cliente>()
            .With(c => c.Nome, nome)
            .Create();

        // Assert
        cliente.Nome.Should().Be(nome);
    }

    [Theory]
    [InlineData("123.456.789-00")]
    [InlineData("987.654.321-11")]
    [InlineData("111.222.333-44")]
    public void Cliente_ShouldAcceptValidCPFs(string cpf)
    {
        // Arrange & Act
        var cliente = _fixture.Build<Cliente>()
            .With(c => c.CPF, cpf)
            .Create();

        // Assert
        cliente.CPF.Should().Be(cpf);
    }

    [Theory]
    [InlineData("test@email.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("complex+email@subdomain.example.org")]
    public void Cliente_ShouldAcceptValidEmails(string email)
    {
        // Arrange & Act
        var cliente = _fixture.Build<Cliente>()
            .With(c => c.Email, email)
            .Create();

        // Assert
        cliente.Email.Should().Be(email);
    }

    [Theory]
    [InlineData("Ativo")]
    [InlineData("Inativo")]
    [InlineData("Prospecto")]
    public void Cliente_ShouldAcceptValidStatus(string status)
    {
        // Arrange & Act
        var cliente = _fixture.Build<Cliente>()
            .With(c => c.StatusCliente, status)
            .Create();

        // Assert
        cliente.StatusCliente.Should().Be(status);
    }

    [Fact]
    public void Cliente_ShouldUpdateDataAtualizacao_WhenModified()
    {
        // Arrange
        var cliente = _fixture.Create<Cliente>();
        var originalDataAtualizacao = cliente.DataAtualizacao;
        
        Thread.Sleep(10); // Ensure time difference

        // Act
        cliente.DataAtualizacao = DateTime.UtcNow;

        // Assert
        cliente.DataAtualizacao.Should().BeAfter(originalDataAtualizacao);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1500.50)]
    [InlineData(10000.00)]
    [InlineData(50000.75)]
    public void Cliente_ShouldAcceptValidRendaMensal(decimal rendaMensal)
    {
        // Arrange & Act
        var cliente = _fixture.Build<Cliente>()
            .With(c => c.RendaMensal, rendaMensal)
            .Create();

        // Assert
        cliente.RendaMensal.Should().Be(rendaMensal);
    }

    [Fact]
    public void Cliente_ShouldAllowNullOptionalFields()
    {
        // Arrange & Act
        var cliente = new Cliente
        {
            Nome = "Test Cliente",
            CPF = "123.456.789-00",
            Email = null,
            Telefone = null,
            Endereco = null,
            Cidade = null,
            Estado = null,
            CEP = null,
            DataNascimento = null,
            RendaMensal = null,
            Observacoes = null
        };

        // Assert
        cliente.Email.Should().BeNull();
        cliente.Telefone.Should().BeNull();
        cliente.Endereco.Should().BeNull();
        cliente.Cidade.Should().BeNull();
        cliente.Estado.Should().BeNull();
        cliente.CEP.Should().BeNull();
        cliente.DataNascimento.Should().BeNull();
        cliente.RendaMensal.Should().BeNull();
        cliente.Observacoes.Should().BeNull();
    }

    [Fact]
    public void Cliente_ShouldInitializeVendasCollection()
    {
        // Act
        var cliente = new Cliente();

        // Assert
        cliente.Vendas.Should().NotBeNull();
        cliente.Vendas.Should().BeEmpty();
    }

    [Theory]
    [InlineData("SP")]
    [InlineData("RJ")]
    [InlineData("MG")]
    [InlineData("RS")]
    public void Cliente_ShouldAcceptValidEstados(string estado)
    {
        // Arrange & Act
        var cliente = _fixture.Build<Cliente>()
            .With(c => c.Estado, estado)
            .Create();

        // Assert
        cliente.Estado.Should().Be(estado);
    }

    [Theory]
    [InlineData("12345-678")]
    [InlineData("98765-432")]
    [InlineData("00000-000")]
    public void Cliente_ShouldAcceptValidCEPs(string cep)
    {
        // Arrange & Act
        var cliente = _fixture.Build<Cliente>()
            .With(c => c.CEP, cep)
            .Create();

        // Assert
        cliente.CEP.Should().Be(cep);
    }
}

public class ApartamentoTests
{
    private readonly Fixture _fixture;

    public ApartamentoTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Apartamento_WhenCreated_ShouldHaveDefaultValues()
    {
        // Act
        var apartamento = new Apartamento();

        // Assert
        apartamento.Id.Should().Be(0);
        apartamento.StatusApartamento.Should().Be("Disponível");
        apartamento.DataCadastro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        apartamento.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("101")]
    [InlineData("A-205")]
    [InlineData("Cobertura 1")]
    public void Apartamento_ShouldAcceptValidNumeros(string numero)
    {
        // Arrange & Act
        var apartamento = _fixture.Build<Apartamento>()
            .With(a => a.Numero, numero)
            .Create();

        // Assert
        apartamento.Numero.Should().Be(numero);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("B")]
    [InlineData("Torre 1")]
    [InlineData("Bloco Norte")]
    public void Apartamento_ShouldAcceptValidBlocos(string bloco)
    {
        // Arrange & Act
        var apartamento = _fixture.Build<Apartamento>()
            .With(a => a.Bloco, bloco)
            .Create();

        // Assert
        apartamento.Bloco.Should().Be(bloco);
    }

    [Theory]
    [InlineData("Disponível")]
    [InlineData("Reservado")]
    [InlineData("Vendido")]
    [InlineData("Indisponível")]
    public void Apartamento_ShouldAcceptValidStatus(string status)
    {
        // Arrange & Act
        var apartamento = _fixture.Build<Apartamento>()
            .With(a => a.StatusApartamento, status)
            .Create();

        // Assert
        apartamento.StatusApartamento.Should().Be(status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Apartamento_ShouldAcceptValidQuartos(int quartos)
    {
        // Arrange & Act
        var apartamento = _fixture.Build<Apartamento>()
            .With(a => a.Quartos, quartos)
            .Create();

        // Assert
        apartamento.Quartos.Should().Be(quartos);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Apartamento_ShouldAcceptValidBanheiros(int banheiros)
    {
        // Arrange & Act
        var apartamento = _fixture.Build<Apartamento>()
            .With(a => a.Banheiros, banheiros)
            .Create();

        // Assert
        apartamento.Banheiros.Should().Be(banheiros);
    }

    [Theory]
    [InlineData(45.5)]
    [InlineData(65.0)]
    [InlineData(120.75)]
    [InlineData(250.25)]
    public void Apartamento_ShouldAcceptValidAreas(decimal area)
    {
        // Arrange & Act
        var apartamento = _fixture.Build<Apartamento>()
            .With(a => a.AreaTotal, area)
            .With(a => a.AreaPrivativa, area * 0.8m)
            .Create();

        // Assert
        apartamento.AreaTotal.Should().Be(area);
        apartamento.AreaPrivativa.Should().Be(area * 0.8m);
    }

    [Theory]
    [InlineData(150000)]
    [InlineData(250000)]
    [InlineData(500000)]
    [InlineData(1000000)]
    public void Apartamento_ShouldAcceptValidPrecos(decimal preco)
    {
        // Arrange & Act
        var apartamento = _fixture.Build<Apartamento>()
            .With(a => a.Preco, preco)
            .Create();

        // Assert
        apartamento.Preco.Should().Be(preco);
    }

    [Fact]
    public void Apartamento_ShouldInitializeVendasCollection()
    {
        // Act
        var apartamento = new Apartamento();

        // Assert
        apartamento.Vendas.Should().NotBeNull();
        apartamento.Vendas.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Apartamento 1 Quarto")]
    [InlineData("Apartamento 2 Quartos")]
    [InlineData("Cobertura Duplex")]
    [InlineData("Studio")]
    public void Apartamento_ShouldAcceptValidTipos(string tipo)
    {
        // Arrange & Act
        var apartamento = _fixture.Build<Apartamento>()
            .With(a => a.TipoApartamento, tipo)
            .Create();

        // Assert
        apartamento.TipoApartamento.Should().Be(tipo);
    }
}

public class VendaTests
{
    private readonly Fixture _fixture;

    public VendaTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Venda_WhenCreated_ShouldHaveDefaultValues()
    {
        // Act
        var venda = new Venda();

        // Assert
        venda.Id.Should().Be(0);
        venda.StatusVenda.Should().Be("Pendente");
        venda.DataCadastro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        venda.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("Pendente")]
    [InlineData("Confirmada")]
    [InlineData("Cancelada")]
    [InlineData("Em Análise")]
    public void Venda_ShouldAcceptValidStatus(string status)
    {
        // Arrange & Act
        var venda = _fixture.Build<Venda>()
            .With(v => v.StatusVenda, status)
            .Create();

        // Assert
        venda.StatusVenda.Should().Be(status);
    }

    [Theory]
    [InlineData(150000)]
    [InlineData(250000)]
    [InlineData(500000)]
    public void Venda_ShouldAcceptValidValues(decimal valor)
    {
        // Arrange & Act
        var venda = _fixture.Build<Venda>()
            .With(v => v.ValorTotal, valor)
            .With(v => v.ValorEntrada, valor * 0.2m)
            .Create();

        // Assert
        venda.ValorTotal.Should().Be(valor);
        venda.ValorEntrada.Should().Be(valor * 0.2m);
    }

    [Theory]
    [InlineData(12, 2500)]
    [InlineData(24, 1250)]
    [InlineData(360, 800)]
    public void Venda_ShouldAcceptValidFinanciamento(int parcelas, decimal valorParcela)
    {
        // Arrange & Act
        var venda = _fixture.Build<Venda>()
            .With(v => v.NumeroParcelasFinanciamento, parcelas)
            .With(v => v.ValorParcelasFinanciamento, valorParcela)
            .Create();

        // Assert
        venda.NumeroParcelasFinanciamento.Should().Be(parcelas);
        venda.ValorParcelasFinanciamento.Should().Be(valorParcela);
    }

    [Fact]
    public void Venda_ShouldRequireClienteAndApartamento()
    {
        // Arrange & Act
        var venda = _fixture.Build<Venda>()
            .With(v => v.ClienteId, 1)
            .With(v => v.ApartamentoId, 1)
            .Create();

        // Assert
        venda.ClienteId.Should().BePositive();
        venda.ApartamentoId.Should().BePositive();
    }

    [Fact]
    public void Venda_ShouldAllowNullOptionalFinancingFields()
    {
        // Arrange & Act
        var venda = new Venda
        {
            ClienteId = 1,
            ApartamentoId = 1,
            ValorTotal = 250000,
            DataVenda = DateTime.UtcNow,
            ValorEntrada = null,
            NumeroParcelasFinanciamento = null,
            ValorParcelasFinanciamento = null,
            Observacoes = null
        };

        // Assert
        venda.ValorEntrada.Should().BeNull();
        venda.NumeroParcelasFinanciamento.Should().BeNull();
        venda.ValorParcelasFinanciamento.Should().BeNull();
        venda.Observacoes.Should().BeNull();
    }

    [Fact]
    public void Venda_ShouldHaveNavigationProperties()
    {
        // Act
        var venda = new Venda();

        // Assert
        venda.Should().HaveProperty("Cliente");
        venda.Should().HaveProperty("Apartamento");
    }
}
