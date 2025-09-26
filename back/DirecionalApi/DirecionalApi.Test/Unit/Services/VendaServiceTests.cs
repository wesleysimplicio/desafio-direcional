using AutoFixture;
using AutoFixture.Xunit2;
using DirecionalApi.Aplication.DTOs.Venda;
using DirecionalApi.Aplication.Services;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Domain.Interfaces;

namespace DirecionalApi.Test.Unit.Services;

public class VendaServiceTests
{
    private readonly Mock<IVendaRepository> _vendaRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IApartamentoRepository> _apartamentoRepositoryMock;
    private readonly VendaService _vendaService;
    private readonly Fixture _fixture;

    public VendaServiceTests()
    {
        _vendaRepositoryMock = new Mock<IVendaRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _apartamentoRepositoryMock = new Mock<IApartamentoRepository>();
        
        _vendaService = new VendaService(
            _vendaRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _apartamentoRepositoryMock.Object);
        
        _fixture = new Fixture();
    }

    [Theory]
    [AutoData]
    public async Task GetAllAsync_ReturnsAllVendas(List<Venda> vendas)
    {
        // Arrange
        _vendaRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(vendas);

        // Act
        var result = await _vendaService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(vendas.Count);
        result.Should().BeEquivalentTo(vendas, options => 
            options.ExcludingMissingMembers());
    }

    [Theory]
    [AutoData]
    public async Task GetByIdAsync_WithValidId_ReturnsVenda(Venda venda)
    {
        // Arrange
        _vendaRepositoryMock.Setup(x => x.GetByIdAsync(venda.Id))
            .ReturnsAsync(venda);

        // Act
        var result = await _vendaService.GetByIdAsync(venda.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(venda.Id);
        result.ClienteId.Should().Be(venda.ClienteId);
        result.ApartamentoId.Should().Be(venda.ApartamentoId);
    }

    [Theory]
    [AutoData]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull(int invalidId)
    {
        // Arrange
        _vendaRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
            .ReturnsAsync((Venda?)null);

        // Act
        var result = await _vendaService.GetByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task CreateAsync_WithValidData_ReturnsCreatedVenda(
        CreateVendaDto createDto,
        Cliente cliente,
        Apartamento apartamento)
    {
        // Arrange
        apartamento.StatusApartamento = "Disponível";
        
        var venda = _fixture.Build<Venda>()
            .With(v => v.ClienteId, createDto.ClienteId)
            .With(v => v.ApartamentoId, createDto.ApartamentoId)
            .With(v => v.ValorTotal, createDto.ValorTotal)
            .Create();

        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(createDto.ClienteId))
            .ReturnsAsync(cliente);

        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(createDto.ApartamentoId))
            .ReturnsAsync(apartamento);

        _vendaRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Venda>()))
            .ReturnsAsync(venda);

        _apartamentoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Apartamento>()))
            .ReturnsAsync(apartamento);

        // Act
        var result = await _vendaService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.ClienteId.Should().Be(createDto.ClienteId);
        result.ApartamentoId.Should().Be(createDto.ApartamentoId);
        result.ValorTotal.Should().Be(createDto.ValorTotal);

        _vendaRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Venda>()), Times.Once);
        _apartamentoRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Apartamento>(a => 
            a.StatusApartamento == "Reservado")), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task CreateAsync_WithInvalidCliente_ThrowsKeyNotFoundException(
        CreateVendaDto createDto)
    {
        // Arrange
        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(createDto.ClienteId))
            .ReturnsAsync((Cliente?)null);

        // Act & Assert
        await _vendaService.Invoking(x => x.CreateAsync(createDto))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Cliente com ID {createDto.ClienteId} não encontrado");

        _vendaRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Venda>()), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task CreateAsync_WithInvalidApartamento_ThrowsKeyNotFoundException(
        CreateVendaDto createDto,
        Cliente cliente)
    {
        // Arrange
        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(createDto.ClienteId))
            .ReturnsAsync(cliente);

        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(createDto.ApartamentoId))
            .ReturnsAsync((Apartamento?)null);

        // Act & Assert
        await _vendaService.Invoking(x => x.CreateAsync(createDto))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Apartamento com ID {createDto.ApartamentoId} não encontrado");

        _vendaRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Venda>()), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task CreateAsync_WithUnavailableApartamento_ThrowsInvalidOperationException(
        CreateVendaDto createDto,
        Cliente cliente,
        Apartamento apartamento)
    {
        // Arrange
        apartamento.StatusApartamento = "Vendido";

        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(createDto.ClienteId))
            .ReturnsAsync(cliente);

        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(createDto.ApartamentoId))
            .ReturnsAsync(apartamento);

        // Act & Assert
        await _vendaService.Invoking(x => x.CreateAsync(createDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Apartamento não está disponível para venda");

        _vendaRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Venda>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithNullDto_ThrowsArgumentNullException()
    {
        // Act & Assert
        await _vendaService.Invoking(x => x.CreateAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_WithValidData_ReturnsUpdatedVenda(
        int vendaId,
        UpdateVendaDto updateDto,
        Venda existingVenda,
        Cliente cliente,
        Apartamento apartamento)
    {
        // Arrange
        existingVenda.Id = vendaId;
        
        var updatedVenda = _fixture.Build<Venda>()
            .With(v => v.Id, vendaId)
            .With(v => v.ClienteId, updateDto.ClienteId)
            .With(v => v.ApartamentoId, updateDto.ApartamentoId)
            .Create();

        _vendaRepositoryMock.Setup(x => x.GetByIdAsync(vendaId))
            .ReturnsAsync(existingVenda);

        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(updateDto.ClienteId))
            .ReturnsAsync(cliente);

        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(updateDto.ApartamentoId))
            .ReturnsAsync(apartamento);

        _vendaRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Venda>()))
            .ReturnsAsync(updatedVenda);

        // Act
        var result = await _vendaService.UpdateAsync(vendaId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(vendaId);
        
        _vendaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Venda>()), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_WithInvalidId_ThrowsKeyNotFoundException(
        int invalidId,
        UpdateVendaDto updateDto)
    {
        // Arrange
        _vendaRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
            .ReturnsAsync((Venda?)null);

        // Act & Assert
        await _vendaService.Invoking(x => x.UpdateAsync(invalidId, updateDto))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Venda com ID {invalidId} não encontrada");

        _vendaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Venda>()), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task DeleteAsync_WithValidId_DeletesVenda(Venda venda, Apartamento apartamento)
    {
        // Arrange
        apartamento.Id = venda.ApartamentoId;
        apartamento.StatusApartamento = "Reservado";

        _vendaRepositoryMock.Setup(x => x.GetByIdAsync(venda.Id))
            .ReturnsAsync(venda);

        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(venda.ApartamentoId))
            .ReturnsAsync(apartamento);

        _vendaRepositoryMock.Setup(x => x.DeleteAsync(venda.Id))
            .Returns(Task.CompletedTask);

        _apartamentoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Apartamento>()))
            .ReturnsAsync(apartamento);

        // Act
        await _vendaService.DeleteAsync(venda.Id);

        // Assert
        _vendaRepositoryMock.Verify(x => x.DeleteAsync(venda.Id), Times.Once);
        _apartamentoRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Apartamento>(a => 
            a.StatusApartamento == "Disponível")), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException(int invalidId)
    {
        // Arrange
        _vendaRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
            .ReturnsAsync((Venda?)null);

        // Act & Assert
        await _vendaService.Invoking(x => x.DeleteAsync(invalidId))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Venda com ID {invalidId} não encontrada");

        _vendaRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task GetByClienteIdAsync_ReturnsVendasForCliente(int clienteId, List<Venda> vendas)
    {
        // Arrange
        _vendaRepositoryMock.Setup(x => x.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(vendas);

        // Act
        var result = await _vendaService.GetByClienteIdAsync(clienteId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(vendas, options => 
            options.ExcludingMissingMembers());
    }

    [Theory]
    [AutoData]
    public async Task GetByApartamentoIdAsync_ReturnsVendasForApartamento(int apartamentoId, List<Venda> vendas)
    {
        // Arrange
        _vendaRepositoryMock.Setup(x => x.GetByApartamentoIdAsync(apartamentoId))
            .ReturnsAsync(vendas);

        // Act
        var result = await _vendaService.GetByApartamentoIdAsync(apartamentoId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(vendas, options => 
            options.ExcludingMissingMembers());
    }

    [Theory]
    [AutoData]
    public async Task ConfirmAsync_WithPendingVenda_UpdatesStatusAndApartamento(
        Venda venda,
        Apartamento apartamento)
    {
        // Arrange
        venda.StatusVenda = "Pendente";
        apartamento.Id = venda.ApartamentoId;

        var confirmedVenda = _fixture.Build<Venda>()
            .With(v => v.Id, venda.Id)
            .With(v => v.StatusVenda, "Confirmada")
            .Create();

        _vendaRepositoryMock.Setup(x => x.GetByIdAsync(venda.Id))
            .ReturnsAsync(venda);

        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(venda.ApartamentoId))
            .ReturnsAsync(apartamento);

        _vendaRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Venda>()))
            .ReturnsAsync(confirmedVenda);

        _apartamentoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Apartamento>()))
            .ReturnsAsync(apartamento);

        // Act
        var result = await _vendaService.ConfirmAsync(venda.Id);

        // Assert
        result.Should().NotBeNull();
        result.StatusVenda.Should().Be("Confirmada");
        
        _vendaRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Venda>(v => 
            v.StatusVenda == "Confirmada")), Times.Once);
        _apartamentoRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Apartamento>(a => 
            a.StatusApartamento == "Vendido")), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task ConfirmAsync_WithAlreadyConfirmedVenda_ThrowsInvalidOperationException(Venda venda)
    {
        // Arrange
        venda.StatusVenda = "Confirmada";

        _vendaRepositoryMock.Setup(x => x.GetByIdAsync(venda.Id))
            .ReturnsAsync(venda);

        // Act & Assert
        await _vendaService.Invoking(x => x.ConfirmAsync(venda.Id))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Venda já foi confirmada ou cancelada");

        _vendaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Venda>()), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task CancelAsync_WithPendingVenda_UpdatesStatusAndApartamento(
        Venda venda,
        Apartamento apartamento)
    {
        // Arrange
        venda.StatusVenda = "Pendente";
        apartamento.Id = venda.ApartamentoId;

        var cancelledVenda = _fixture.Build<Venda>()
            .With(v => v.Id, venda.Id)
            .With(v => v.StatusVenda, "Cancelada")
            .Create();

        _vendaRepositoryMock.Setup(x => x.GetByIdAsync(venda.Id))
            .ReturnsAsync(venda);

        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(venda.ApartamentoId))
            .ReturnsAsync(apartamento);

        _vendaRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Venda>()))
            .ReturnsAsync(cancelledVenda);

        _apartamentoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Apartamento>()))
            .ReturnsAsync(apartamento);

        // Act
        var result = await _vendaService.CancelAsync(venda.Id);

        // Assert
        result.Should().NotBeNull();
        result.StatusVenda.Should().Be("Cancelada");
        
        _vendaRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Venda>(v => 
            v.StatusVenda == "Cancelada")), Times.Once);
        _apartamentoRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Apartamento>(a => 
            a.StatusApartamento == "Disponível")), Times.Once);
    }
}
