using AutoFixture;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Infrastructure.Data;
using DirecionalApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DirecionalApi.Test.Unit.Infrastructure;

public class ClienteRepositoryTests : IDisposable
{
    private readonly DirecionalContext _context;
    private readonly ClienteRepository _repository;
    private readonly Fixture _fixture;

    public ClienteRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DirecionalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DirecionalContext(options);
        _repository = new ClienteRepository(_context);
        _fixture = new Fixture();
        
        // Configure AutoFixture to avoid circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingClient_ShouldReturnClient()
    {
        // Arrange
        var cliente = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .Create();
        
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(cliente.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cliente.Id);
        result.Nome.Should().Be(cliente.Nome);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingClient_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllClients()
    {
        // Arrange
        var clientes = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .CreateMany(3)
            .ToList();

        _context.Clientes.AddRange(clientes);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAsync_ValidClient_ShouldAddSuccessfully()
    {
        // Arrange
        var cliente = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .Create();

        // Act
        var result = await _repository.AddAsync(cliente);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        
        var savedClient = await _context.Clientes.FindAsync(result.Id);
        savedClient.Should().NotBeNull();
        savedClient.Nome.Should().Be(cliente.Nome);
    }

    [Fact]
    public async Task UpdateAsync_ExistingClient_ShouldUpdateSuccessfully()
    {
        // Arrange
        var cliente = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .Create();
        
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        cliente.Nome = "Nome Atualizado";

        // Act
        _repository.Update(cliente);
        await _context.SaveChangesAsync();

        // Assert
        var updatedClient = await _context.Clientes.FindAsync(cliente.Id);
        updatedClient.Should().NotBeNull();
        updatedClient.Nome.Should().Be("Nome Atualizado");
    }

    [Fact]
    public async Task DeleteAsync_ExistingClient_ShouldDeleteSuccessfully()
    {
        // Arrange
        var cliente = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .Create();
        
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(cliente.Id);
        await _context.SaveChangesAsync();

        // Assert
        var deletedClient = await _context.Clientes.FindAsync(cliente.Id);
        deletedClient.Should().BeNull();
    }

    [Fact]
    public async Task GetByCPFAsync_ExistingCPF_ShouldReturnClient()
    {
        // Arrange
        var cpf = "123.456.789-00";
        var cliente = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .With(c => c.CPF, cpf)
            .Create();
        
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCPFAsync(cpf);

        // Assert
        result.Should().NotBeNull();
        result.CPF.Should().Be(cpf);
    }

    [Fact]
    public async Task GetByCPFAsync_NonExistingCPF_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByCPFAsync("999.999.999-99");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingEmail_ShouldReturnClient()
    {
        // Arrange
        var email = "test@email.com";
        var cliente = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .With(c => c.Email, email)
            .Create();
        
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

public class ApartamentoRepositoryTests : IDisposable
{
    private readonly DirecionalContext _context;
    private readonly ApartamentoRepository _repository;
    private readonly Fixture _fixture;

    public ApartamentoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DirecionalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DirecionalContext(options);
        _repository = new ApartamentoRepository(_context);
        _fixture = new Fixture();
        
        // Configure AutoFixture to avoid circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingApartment_ShouldReturnApartment()
    {
        // Arrange
        var apartamento = _fixture.Build<Apartamento>()
            .Without(a => a.Id)
            .Without(a => a.Vendas)
            .Create();
        
        _context.Apartamentos.Add(apartamento);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(apartamento.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(apartamento.Id);
        result.Numero.Should().Be(apartamento.Numero);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllApartments()
    {
        // Arrange
        var apartamentos = _fixture.Build<Apartamento>()
            .Without(a => a.Id)
            .Without(a => a.Vendas)
            .CreateMany(5)
            .ToList();

        _context.Apartamentos.AddRange(apartamentos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task AddAsync_ValidApartment_ShouldAddSuccessfully()
    {
        // Arrange
        var apartamento = _fixture.Build<Apartamento>()
            .Without(a => a.Id)
            .Without(a => a.Vendas)
            .Create();

        // Act
        var result = await _repository.AddAsync(apartamento);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetByNumeroAndBlocoAsync_ExistingApartment_ShouldReturnApartment()
    {
        // Arrange
        var apartamento = _fixture.Build<Apartamento>()
            .Without(a => a.Id)
            .Without(a => a.Vendas)
            .With(a => a.Numero, "101")
            .With(a => a.Bloco, "A")
            .Create();
        
        _context.Apartamentos.Add(apartamento);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNumeroAndBlocoAsync("101", "A");

        // Assert
        result.Should().NotBeNull();
        result.Numero.Should().Be("101");
        result.Bloco.Should().Be("A");
    }

    [Fact]
    public async Task GetAvailableAsync_ShouldReturnOnlyAvailableApartments()
    {
        // Arrange
        var apartamentos = new List<Apartamento>
        {
            _fixture.Build<Apartamento>()
                .Without(a => a.Id)
                .Without(a => a.Vendas)
                .With(a => a.StatusApartamento, "Disponível")
                .Create(),
            _fixture.Build<Apartamento>()
                .Without(a => a.Id)
                .Without(a => a.Vendas)
                .With(a => a.StatusApartamento, "Vendido")
                .Create(),
            _fixture.Build<Apartamento>()
                .Without(a => a.Id)
                .Without(a => a.Vendas)
                .With(a => a.StatusApartamento, "Disponível")
                .Create()
        };

        _context.Apartamentos.AddRange(apartamentos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAvailableAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.StatusApartamento == "Disponível");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

public class VendaRepositoryTests : IDisposable
{
    private readonly DirecionalContext _context;
    private readonly VendaRepository _repository;
    private readonly Fixture _fixture;

    public VendaRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DirecionalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DirecionalContext(options);
        _repository = new VendaRepository(_context);
        _fixture = new Fixture();
        
        // Configure AutoFixture to avoid circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingSale_ShouldReturnSale()
    {
        // Arrange
        var cliente = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .Create();
        var apartamento = _fixture.Build<Apartamento>()
            .Without(a => a.Id)
            .Without(a => a.Vendas)
            .Create();

        _context.Clientes.Add(cliente);
        _context.Apartamentos.Add(apartamento);
        await _context.SaveChangesAsync();

        var venda = _fixture.Build<Venda>()
            .Without(v => v.Id)
            .Without(v => v.Cliente)
            .Without(v => v.Apartamento)
            .With(v => v.ClienteId, cliente.Id)
            .With(v => v.ApartamentoId, apartamento.Id)
            .Create();

        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(venda.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(venda.Id);
        result.ClienteId.Should().Be(cliente.Id);
        result.ApartamentoId.Should().Be(apartamento.Id);
    }

    [Fact]
    public async Task GetAllWithDetailsAsync_ShouldReturnSalesWithClientAndApartmentInfo()
    {
        // Arrange
        var cliente = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .Create();
        var apartamento = _fixture.Build<Apartamento>()
            .Without(a => a.Id)
            .Without(a => a.Vendas)
            .Create();

        _context.Clientes.Add(cliente);
        _context.Apartamentos.Add(apartamento);
        await _context.SaveChangesAsync();

        var venda = _fixture.Build<Venda>()
            .Without(v => v.Id)
            .Without(v => v.Cliente)
            .Without(v => v.Apartamento)
            .With(v => v.ClienteId, cliente.Id)
            .With(v => v.ApartamentoId, apartamento.Id)
            .Create();

        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllWithDetailsAsync();

        // Assert
        result.Should().HaveCount(1);
        var vendaResult = result.First();
        vendaResult.Cliente.Should().NotBeNull();
        vendaResult.Apartamento.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByClienteIdAsync_ShouldReturnClientSales()
    {
        // Arrange
        var cliente1 = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .Create();
        var cliente2 = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .Create();
        var apartamento = _fixture.Build<Apartamento>()
            .Without(a => a.Id)
            .Without(a => a.Vendas)
            .Create();

        _context.Clientes.AddRange(cliente1, cliente2);
        _context.Apartamentos.Add(apartamento);
        await _context.SaveChangesAsync();

        var vendas = new List<Venda>
        {
            _fixture.Build<Venda>()
                .Without(v => v.Id)
                .Without(v => v.Cliente)
                .Without(v => v.Apartamento)
                .With(v => v.ClienteId, cliente1.Id)
                .With(v => v.ApartamentoId, apartamento.Id)
                .Create(),
            _fixture.Build<Venda>()
                .Without(v => v.Id)
                .Without(v => v.Cliente)
                .Without(v => v.Apartamento)
                .With(v => v.ClienteId, cliente2.Id)
                .With(v => v.ApartamentoId, apartamento.Id)
                .Create(),
            _fixture.Build<Venda>()
                .Without(v => v.Id)
                .Without(v => v.Cliente)
                .Without(v => v.Apartamento)
                .With(v => v.ClienteId, cliente1.Id)
                .With(v => v.ApartamentoId, apartamento.Id)
                .Create()
        };

        _context.Vendas.AddRange(vendas);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByClienteIdAsync(cliente1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(v => v.ClienteId == cliente1.Id);
    }

    [Fact]
    public async Task AddAsync_ValidSale_ShouldAddSuccessfully()
    {
        // Arrange
        var cliente = _fixture.Build<Cliente>()
            .Without(c => c.Id)
            .Without(c => c.Vendas)
            .Create();
        var apartamento = _fixture.Build<Apartamento>()
            .Without(a => a.Id)
            .Without(a => a.Vendas)
            .Create();

        _context.Clientes.Add(cliente);
        _context.Apartamentos.Add(apartamento);
        await _context.SaveChangesAsync();

        var venda = _fixture.Build<Venda>()
            .Without(v => v.Id)
            .Without(v => v.Cliente)
            .Without(v => v.Apartamento)
            .With(v => v.ClienteId, cliente.Id)
            .With(v => v.ApartamentoId, apartamento.Id)
            .Create();

        // Act
        var result = await _repository.AddAsync(venda);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
