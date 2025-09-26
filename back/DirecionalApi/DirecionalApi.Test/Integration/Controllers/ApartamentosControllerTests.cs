using DirecionalApi.Aplication.DTOs.Auth;
using DirecionalApi.Aplication.DTOs.Apartamento;
using DirecionalApi.Web;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;

namespace DirecionalApi.Test.Integration.Controllers;

public class ApartamentosControllerTests : IClassFixture<DirecionalApiWebApplicationFactory<Program>>
{
    private readonly DirecionalApiWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApartamentosControllerTests(DirecionalApiWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task GetApartamentos_WithoutAuth_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/apartamentos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetApartamentos_WithAuth_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/apartamentos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var apartamentos = JsonSerializer.Deserialize<List<ApartamentoDto>>(content, _jsonOptions);
        
        apartamentos.Should().NotBeNull();
        apartamentos.Should().BeOfType<List<ApartamentoDto>>();
    }

    [Fact]
    public async Task CreateApartamento_WithValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateApartamentoDto
        {
            Numero = "101",
            Bloco = "A",
            TipoApartamento = "Apartamento 2 Quartos",
            Quartos = 2,
            Banheiros = 1,
            AreaTotal = 65.5m,
            Preco = 250000m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/apartamentos", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var apartamento = JsonSerializer.Deserialize<ApartamentoDto>(content, _jsonOptions);
        
        apartamento.Should().NotBeNull();
        apartamento.Numero.Should().Be(createDto.Numero);
        apartamento.Bloco.Should().Be(createDto.Bloco);
        apartamento.TipoApartamento.Should().Be(createDto.TipoApartamento);
        apartamento.StatusApartamento.Should().Be("Disponível");
    }

    [Fact]
    public async Task CreateApartamento_WithDuplicateNumeroAndBloco_ReturnsConflict()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateApartamentoDto
        {
            Numero = "102",
            Bloco = "A",
            TipoApartamento = "Apartamento 3 Quartos",
            Quartos = 3,
            Banheiros = 2
        };

        // Create first apartamento
        await _client.PostAsJsonAsync("/api/apartamentos", createDto);

        // Try to create another with same numero and bloco
        var duplicateDto = new CreateApartamentoDto
        {
            Numero = "102", // Same numero
            Bloco = "A",    // Same bloco
            TipoApartamento = "Apartamento 2 Quartos",
            Quartos = 2,
            Banheiros = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/apartamentos", duplicateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateApartamento_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateApartamentoDto
        {
            Numero = "", // Invalid - empty numero
            Bloco = "",  // Invalid - empty bloco
            TipoApartamento = "", // Invalid - empty tipo
            Quartos = -1, // Invalid - negative quartos
            Banheiros = 0 // Invalid - zero banheiros
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/apartamentos", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetApartamento_WithValidId_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create an apartamento
        var createDto = new CreateApartamentoDto
        {
            Numero = "201",
            Bloco = "B",
            TipoApartamento = "Apartamento 1 Quarto",
            Quartos = 1,
            Banheiros = 1,
            Preco = 180000m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/apartamentos", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdApartamento = JsonSerializer.Deserialize<ApartamentoDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.GetAsync($"/api/apartamentos/{createdApartamento.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var apartamento = JsonSerializer.Deserialize<ApartamentoDto>(content, _jsonOptions);
        
        apartamento.Should().NotBeNull();
        apartamento.Id.Should().Be(createdApartamento.Id);
        apartamento.Numero.Should().Be(createDto.Numero);
        apartamento.Bloco.Should().Be(createDto.Bloco);
    }

    [Fact]
    public async Task GetApartamento_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/apartamentos/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateApartamento_WithValidData_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create an apartamento
        var createDto = new CreateApartamentoDto
        {
            Numero = "301",
            Bloco = "C",
            TipoApartamento = "Apartamento 2 Quartos",
            Quartos = 2,
            Banheiros = 1,
            Preco = 220000m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/apartamentos", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdApartamento = JsonSerializer.Deserialize<ApartamentoDto>(createContent, _jsonOptions);

        // Prepare update
        var updateDto = new UpdateApartamentoDto
        {
            Numero = "301",
            Bloco = "C",
            TipoApartamento = "Apartamento 2 Quartos", 
            Quartos = 2,
            Banheiros = 2, // Updated
            AreaTotal = 70.0m, // Updated
            Preco = 230000m, // Updated
            StatusApartamento = "Reservado" // Updated
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/apartamentos/{createdApartamento.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var updatedApartamento = JsonSerializer.Deserialize<ApartamentoDto>(content, _jsonOptions);
        
        updatedApartamento.Should().NotBeNull();
        updatedApartamento.Banheiros.Should().Be(updateDto.Banheiros);
        updatedApartamento.AreaTotal.Should().Be(updateDto.AreaTotal);
        updatedApartamento.Preco.Should().Be(updateDto.Preco);
        updatedApartamento.StatusApartamento.Should().Be(updateDto.StatusApartamento);
    }

    [Fact]
    public async Task ReserveApartamento_WithAvailableApartamento_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create an available apartamento
        var createDto = new CreateApartamentoDto
        {
            Numero = "401",
            Bloco = "D",
            TipoApartamento = "Apartamento 3 Quartos",
            Quartos = 3,
            Banheiros = 2,
            Preco = 320000m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/apartamentos", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdApartamento = JsonSerializer.Deserialize<ApartamentoDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.PostAsync($"/api/apartamentos/{createdApartamento.Id}/reservar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var reservedApartamento = JsonSerializer.Deserialize<ApartamentoDto>(content, _jsonOptions);
        
        reservedApartamento.Should().NotBeNull();
        reservedApartamento.StatusApartamento.Should().Be("Reservado");
    }

    [Fact]
    public async Task ReserveApartamento_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync("/api/apartamentos/99999/reservar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAvailableApartamentos_ReturnsOnlyAvailable()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create available apartamento
        var availableDto = new CreateApartamentoDto
        {
            Numero = "501",
            Bloco = "E",
            TipoApartamento = "Apartamento 1 Quarto",
            Quartos = 1,
            Banheiros = 1,
            Preco = 150000m
        };

        await _client.PostAsJsonAsync("/api/apartamentos", availableDto);

        // Create and reserve another apartamento
        var reservedDto = new CreateApartamentoDto
        {
            Numero = "502",
            Bloco = "E",
            TipoApartamento = "Apartamento 2 Quartos",
            Quartos = 2,
            Banheiros = 1,
            Preco = 200000m
        };

        var reservedResponse = await _client.PostAsJsonAsync("/api/apartamentos", reservedDto);
        var reservedContent = await reservedResponse.Content.ReadAsStringAsync();
        var reservedApartamento = JsonSerializer.Deserialize<ApartamentoDto>(reservedContent, _jsonOptions);
        
        // Reserve it
        await _client.PostAsync($"/api/apartamentos/{reservedApartamento.Id}/reservar", null);

        // Act
        var response = await _client.GetAsync("/api/apartamentos/disponiveis");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var availableApartamentos = JsonSerializer.Deserialize<List<ApartamentoDto>>(content, _jsonOptions);
        
        availableApartamentos.Should().NotBeNull();
        availableApartamentos.Should().AllSatisfy(a => a.StatusApartamento.Should().Be("Disponível"));
    }

    [Fact]
    public async Task DeleteApartamento_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create an apartamento
        var createDto = new CreateApartamentoDto
        {
            Numero = "999",
            Bloco = "Z",
            TipoApartamento = "Studio",
            Quartos = 0,
            Banheiros = 1,
            Preco = 120000m
        };

        var createResponse = await _client.PostAsJsonAsync("/api/apartamentos", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdApartamento = JsonSerializer.Deserialize<ApartamentoDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.DeleteAsync($"/api/apartamentos/{createdApartamento.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify apartamento was deleted
        var getResponse = await _client.GetAsync($"/api/apartamentos/{createdApartamento.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteApartamento_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/apartamentos/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var loginRequest = new LoginRequest
        {
            Email = "admin@test.com",
            Password = "Test123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);
        
        return authResponse.Token;
    }
}
