using AutoFixture;
using AutoFixture.Xunit2;
using DirecionalApi.Aplication.DTOs.Apartamento;
using DirecionalApi.Aplication.Services;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Domain.Interfaces;

namespace DirecionalApi.Test.Unit.Services;

public class ApartamentoServiceTests
{
    private readonly Mock<IApartamentoRepository> _apartamentoRepositoryMock;
    private readonly ApartamentoService _apartamentoService;
    private readonly Fixture _fixture;

    public ApartamentoServiceTests()
    {
        _apartamentoRepositoryMock = new Mock<IApartamentoRepository>();
        _apartamentoService = new ApartamentoService(_apartamentoRepositoryMock.Object);
        _fixture = new Fixture();
    }

    [Theory]
    [AutoData]
    public async Task GetAllAsync_ReturnsAllApartamentos(List<Apartamento> apartamentos)
    {
        // Arrange
        _apartamentoRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(apartamentos);

        // Act
        var result = await _apartamentoService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(apartamentos.Count);
        result.Should().BeEquivalentTo(apartamentos, options => 
            options.ExcludingMissingMembers());
    }

    [Theory]
    [AutoData]
    public async Task GetByIdAsync_WithValidId_ReturnsApartamento(Apartamento apartamento)
    {
        // Arrange
        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(apartamento.Id))
            .ReturnsAsync(apartamento);

        // Act
        var result = await _apartamentoService.GetByIdAsync(apartamento.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(apartamento.Id);
        result.Numero.Should().Be(apartamento.Numero);
        result.Bloco.Should().Be(apartamento.Bloco);
    }

    [Theory]
    [AutoData]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull(int invalidId)
    {
        // Arrange
        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
            .ReturnsAsync((Apartamento?)null);

        // Act
        var result = await _apartamentoService.GetByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task CreateAsync_WithValidData_ReturnsCreatedApartamento(CreateApartamentoDto createDto)
    {
        // Arrange
        var apartamento = _fixture.Build<Apartamento>()
            .With(a => a.Numero, createDto.Numero)
            .With(a => a.Bloco, createDto.Bloco)
            .With(a => a.TipoApartamento, createDto.TipoApartamento)
            .Create();

        _apartamentoRepositoryMock.Setup(x => x.GetByNumeroAndBlocoAsync(createDto.Numero, createDto.Bloco))
            .ReturnsAsync((Apartamento?)null);

        _apartamentoRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Apartamento>()))
            .ReturnsAsync(apartamento);

        // Act
        var result = await _apartamentoService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Numero.Should().Be(createDto.Numero);
        result.Bloco.Should().Be(createDto.Bloco);
        result.TipoApartamento.Should().Be(createDto.TipoApartamento);

        _apartamentoRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Apartamento>()), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task CreateAsync_WithExistingNumeroAndBloco_ThrowsInvalidOperationException(
        CreateApartamentoDto createDto,
        Apartamento existingApartamento)
    {
        // Arrange
        _apartamentoRepositoryMock.Setup(x => x.GetByNumeroAndBlocoAsync(createDto.Numero, createDto.Bloco))
            .ReturnsAsync(existingApartamento);

        // Act & Assert
        await _apartamentoService.Invoking(x => x.CreateAsync(createDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Já existe um apartamento com este número neste bloco");

        _apartamentoRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Apartamento>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithNullDto_ThrowsArgumentNullException()
    {
        // Act & Assert
        await _apartamentoService.Invoking(x => x.CreateAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_WithValidData_ReturnsUpdatedApartamento(
        int apartamentoId,
        UpdateApartamentoDto updateDto,
        Apartamento existingApartamento)
    {
        // Arrange
        existingApartamento.Id = apartamentoId;
        
        var updatedApartamento = _fixture.Build<Apartamento>()
            .With(a => a.Id, apartamentoId)
            .With(a => a.Numero, updateDto.Numero)
            .With(a => a.Bloco, updateDto.Bloco)
            .With(a => a.TipoApartamento, updateDto.TipoApartamento)
            .Create();

        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(apartamentoId))
            .ReturnsAsync(existingApartamento);

        _apartamentoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Apartamento>()))
            .ReturnsAsync(updatedApartamento);

        // Act
        var result = await _apartamentoService.UpdateAsync(apartamentoId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(apartamentoId);
        result.Numero.Should().Be(updateDto.Numero);
        result.Bloco.Should().Be(updateDto.Bloco);
        
        _apartamentoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Apartamento>()), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_WithInvalidId_ThrowsKeyNotFoundException(
        int invalidId,
        UpdateApartamentoDto updateDto)
    {
        // Arrange
        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
            .ReturnsAsync((Apartamento?)null);

        // Act & Assert
        await _apartamentoService.Invoking(x => x.UpdateAsync(invalidId, updateDto))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Apartamento com ID {invalidId} não encontrado");

        _apartamentoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Apartamento>()), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task DeleteAsync_WithValidId_DeletesApartamento(Apartamento apartamento)
    {
        // Arrange
        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(apartamento.Id))
            .ReturnsAsync(apartamento);

        _apartamentoRepositoryMock.Setup(x => x.DeleteAsync(apartamento.Id))
            .Returns(Task.CompletedTask);

        // Act
        await _apartamentoService.DeleteAsync(apartamento.Id);

        // Assert
        _apartamentoRepositoryMock.Verify(x => x.DeleteAsync(apartamento.Id), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException(int invalidId)
    {
        // Arrange
        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
            .ReturnsAsync((Apartamento?)null);

        // Act & Assert
        await _apartamentoService.Invoking(x => x.DeleteAsync(invalidId))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Apartamento com ID {invalidId} não encontrado");

        _apartamentoRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task GetAvailableAsync_ReturnsOnlyAvailableApartamentos(List<Apartamento> allApartamentos)
    {
        // Arrange
        var availableApartamentos = allApartamentos.Take(3).ToList();
        foreach (var apt in availableApartamentos)
        {
            apt.StatusApartamento = "Disponível";
        }

        _apartamentoRepositoryMock.Setup(x => x.GetByStatusAsync("Disponível"))
            .ReturnsAsync(availableApartamentos);

        // Act
        var result = await _apartamentoService.GetAvailableAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(availableApartamentos.Count);
        result.Should().AllSatisfy(a => a.StatusApartamento.Should().Be("Disponível"));
    }

    [Theory]
    [AutoData]
    public async Task ReserveAsync_WithAvailableApartamento_UpdatesStatus(Apartamento apartamento)
    {
        // Arrange
        apartamento.StatusApartamento = "Disponível";
        
        var reservedApartamento = _fixture.Build<Apartamento>()
            .With(a => a.Id, apartamento.Id)
            .With(a => a.StatusApartamento, "Reservado")
            .Create();

        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(apartamento.Id))
            .ReturnsAsync(apartamento);

        _apartamentoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Apartamento>()))
            .ReturnsAsync(reservedApartamento);

        // Act
        var result = await _apartamentoService.ReserveAsync(apartamento.Id);

        // Assert
        result.Should().NotBeNull();
        result.StatusApartamento.Should().Be("Reservado");
        
        _apartamentoRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Apartamento>(a => 
            a.Id == apartamento.Id && a.StatusApartamento == "Reservado")), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task ReserveAsync_WithUnavailableApartamento_ThrowsInvalidOperationException(Apartamento apartamento)
    {
        // Arrange
        apartamento.StatusApartamento = "Vendido";

        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(apartamento.Id))
            .ReturnsAsync(apartamento);

        // Act & Assert
        await _apartamentoService.Invoking(x => x.ReserveAsync(apartamento.Id))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Apartamento não está disponível para reserva");

        _apartamentoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Apartamento>()), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task ReserveAsync_WithInvalidId_ThrowsKeyNotFoundException(int invalidId)
    {
        // Arrange
        _apartamentoRepositoryMock.Setup(x => x.GetByIdAsync(invalidId))
            .ReturnsAsync((Apartamento?)null);

        // Act & Assert
        await _apartamentoService.Invoking(x => x.ReserveAsync(invalidId))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Apartamento com ID {invalidId} não encontrado");
    }

    [Theory]
    [AutoData]
    public async Task GetByStatusAsync_ReturnsApartamentosWithSpecificStatus(
        List<Apartamento> apartamentos,
        string status)
    {
        // Arrange
        _apartamentoRepositoryMock.Setup(x => x.GetByStatusAsync(status))
            .ReturnsAsync(apartamentos);

        // Act
        var result = await _apartamentoService.GetByStatusAsync(status);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(apartamentos, options => 
            options.ExcludingMissingMembers());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetByStatusAsync_WithNullOrEmptyStatus_ReturnsEmptyList(string status)
    {
        // Act
        var result = await _apartamentoService.GetByStatusAsync(status);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
