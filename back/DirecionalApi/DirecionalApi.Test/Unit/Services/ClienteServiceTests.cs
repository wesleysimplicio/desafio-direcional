using AutoFixture;
using AutoFixture.Xunit2;
using DirecionalApi.Aplication.DTOs.Cliente;
using DirecionalApi.Aplication.Services;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Domain.Interfaces;

namespace DirecionalApi.Test.Unit.Services;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly ClienteService _clienteService;
    private readonly Fixture _fixture;

    public ClienteServiceTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _clienteService = new ClienteService(_clienteRepositoryMock.Object);
        _fixture = new Fixture();
    }

    [Theory]
    [AutoData]
    public async Task GetAllAsync_ReturnsAllClientes(List<Cliente> clientes)
    {
        // Arrange
        _clienteRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(clientes);

        // Act
        var result = await _clienteService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(clientes.Count);
        result.Should().BeEquivalentTo(clientes, options => 
            options.ExcludingMissingMembers());
    }

    [Theory]
    [AutoData]
    public async Task GetByIdAsync_WithValidId_ReturnsCliente(Cliente cliente)
    {
        // Arrange
        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        // Act
        var result = await _clienteService.GetByIdAsync(cliente.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cliente.Id);
        result.Nome.Should().Be(cliente.Nome);
        result.CPF.Should().Be(cliente.CPF);
    }

    [Theory]
    [AutoData]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull(int invalidId)
    {
        // Arrange
        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
            .ReturnsAsync((Cliente?)null);

        // Act
        var result = await _clienteService.GetByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task CreateAsync_WithValidData_ReturnsCreatedCliente(CreateClienteDto createDto)
    {
        // Arrange
        var cliente = _fixture.Build<Cliente>()
            .With(c => c.Nome, createDto.Nome)
            .With(c => c.CPF, createDto.CPF)
            .With(c => c.Email, createDto.Email)
            .Create();

        _clienteRepositoryMock.Setup(x => x.GetByCPFAsync(createDto.CPF))
            .ReturnsAsync((Cliente?)null);

        _clienteRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Cliente>()))
            .ReturnsAsync(cliente);

        // Act
        var result = await _clienteService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Nome.Should().Be(createDto.Nome);
        result.CPF.Should().Be(createDto.CPF);
        result.Email.Should().Be(createDto.Email);

        _clienteRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task CreateAsync_WithExistingCPF_ThrowsInvalidOperationException(
        CreateClienteDto createDto, 
        Cliente existingCliente)
    {
        // Arrange
        _clienteRepositoryMock.Setup(x => x.GetByCPFAsync(createDto.CPF))
            .ReturnsAsync(existingCliente);

        // Act & Assert
        await _clienteService.Invoking(x => x.CreateAsync(createDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Já existe um cliente cadastrado com este CPF");

        _clienteRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithNullDto_ThrowsArgumentNullException()
    {
        // Act & Assert
        await _clienteService.Invoking(x => x.CreateAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_WithValidData_ReturnsUpdatedCliente(
        int clienteId, 
        UpdateClienteDto updateDto,
        Cliente existingCliente)
    {
        // Arrange
        existingCliente.Id = clienteId;
        
        var updatedCliente = _fixture.Build<Cliente>()
            .With(c => c.Id, clienteId)
            .With(c => c.Nome, updateDto.Nome)
            .With(c => c.Email, updateDto.Email)
            .Create();

        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(clienteId))
            .ReturnsAsync(existingCliente);

        _clienteRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Cliente>()))
            .ReturnsAsync(updatedCliente);

        // Act
        var result = await _clienteService.UpdateAsync(clienteId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(clienteId);
        result.Nome.Should().Be(updateDto.Nome);
        
        _clienteRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_WithInvalidId_ThrowsKeyNotFoundException(
        int invalidId,
        UpdateClienteDto updateDto)
    {
        // Arrange
        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
            .ReturnsAsync((Cliente?)null);

        // Act & Assert
        await _clienteService.Invoking(x => x.UpdateAsync(invalidId, updateDto))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Cliente com ID {invalidId} não encontrado");

        _clienteRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_WithNullDto_ThrowsArgumentNullException(int clienteId)
    {
        // Act & Assert
        await _clienteService.Invoking(x => x.UpdateAsync(clienteId, null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoData]
    public async Task DeleteAsync_WithValidId_DeletesCliente(Cliente cliente)
    {
        // Arrange
        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _clienteRepositoryMock.Setup(x => x.DeleteAsync(cliente.Id))
            .Returns(Task.CompletedTask);

        // Act
        await _clienteService.DeleteAsync(cliente.Id);

        // Assert
        _clienteRepositoryMock.Verify(x => x.DeleteAsync(cliente.Id), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException(int invalidId)
    {
        // Arrange
        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
            .ReturnsAsync((Cliente?)null);

        // Act & Assert
        await _clienteService.Invoking(x => x.DeleteAsync(invalidId))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Cliente com ID {invalidId} não encontrado");

        _clienteRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task GetByCPFAsync_WithValidCPF_ReturnsCliente(Cliente cliente)
    {
        // Arrange
        _clienteRepositoryMock.Setup(x => x.GetByCPFAsync(cliente.CPF))
            .ReturnsAsync(cliente);

        // Act
        var result = await _clienteService.GetByCPFAsync(cliente.CPF);

        // Assert
        result.Should().NotBeNull();
        result.CPF.Should().Be(cliente.CPF);
        result.Nome.Should().Be(cliente.Nome);
    }

    [Theory]
    [AutoData]
    public async Task GetByCPFAsync_WithInvalidCPF_ReturnsNull(string invalidCPF)
    {
        // Arrange
        _clienteRepositoryMock.Setup(x => x.GetByCPFAsync(invalidCPF))
            .ReturnsAsync((Cliente?)null);

        // Act
        var result = await _clienteService.GetByCPFAsync(invalidCPF);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetByCPFAsync_WithNullOrEmptyCPF_ReturnsNull(string cpf)
    {
        // Act
        var result = await _clienteService.GetByCPFAsync(cpf);

        // Assert
        result.Should().BeNull();
    }
}
