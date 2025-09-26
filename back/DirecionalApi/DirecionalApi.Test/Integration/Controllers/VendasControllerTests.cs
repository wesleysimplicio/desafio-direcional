using DirecionalApi.Aplication.DTOs.Auth;
using DirecionalApi.Aplication.DTOs.Cliente;
using DirecionalApi.Aplication.DTOs.Apartamento;
using DirecionalApi.Aplication.DTOs.Venda;
using DirecionalApi.Web;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;

namespace DirecionalApi.Test.Integration.Controllers;

public class VendasControllerTests : IClassFixture<DirecionalApiWebApplicationFactory<Program>>
{
    private readonly DirecionalApiWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public VendasControllerTests(DirecionalApiWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task GetVendas_WithoutAuth_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/vendas");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVendas_WithAuth_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/vendas");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var vendas = JsonSerializer.Deserialize<List<VendaDto>>(content, _jsonOptions);
        
        vendas.Should().NotBeNull();
        vendas.Should().BeOfType<List<VendaDto>>();
    }

    [Fact]
    public async Task CreateVenda_WithValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create cliente and apartamento
        var (clienteId, apartamentoId) = await CreateClienteAndApartamentoAsync();

        var createDto = new CreateVendaDto
        {
            ClienteId = clienteId,
            ApartamentoId = apartamentoId,
            ValorTotal = 250000m,
            ValorEntrada = 50000m,
            DataVenda = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/vendas", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var venda = JsonSerializer.Deserialize<VendaDto>(content, _jsonOptions);
        
        venda.Should().NotBeNull();
        venda.ClienteId.Should().Be(createDto.ClienteId);
        venda.ApartamentoId.Should().Be(createDto.ApartamentoId);
        venda.ValorTotal.Should().Be(createDto.ValorTotal);
        venda.StatusVenda.Should().Be("Pendente");
    }

    [Fact]
    public async Task CreateVenda_WithInvalidCliente_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateVendaDto
        {
            ClienteId = 99999, // Invalid cliente ID
            ApartamentoId = 1,
            ValorTotal = 250000m,
            DataVenda = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/vendas", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateVenda_WithInvalidApartamento_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create only cliente
        var clienteId = await CreateClienteAsync("Test Cliente Venda");

        var createDto = new CreateVendaDto
        {
            ClienteId = clienteId,
            ApartamentoId = 99999, // Invalid apartamento ID
            ValorTotal = 250000m,
            DataVenda = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/vendas", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateVenda_WithUnavailableApartamento_ReturnsConflict()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create cliente and apartamento
        var (clienteId1, apartamentoId) = await CreateClienteAndApartamentoAsync();
        
        // Create first venda to make apartamento unavailable
        var firstVendaDto = new CreateVendaDto
        {
            ClienteId = clienteId1,
            ApartamentoId = apartamentoId,
            ValorTotal = 250000m,
            DataVenda = DateTime.UtcNow
        };
        await _client.PostAsJsonAsync("/api/vendas", firstVendaDto);

        // Try to create second venda with same apartamento
        var clienteId2 = await CreateClienteAsync("Second Cliente");
        var secondVendaDto = new CreateVendaDto
        {
            ClienteId = clienteId2,
            ApartamentoId = apartamentoId, // Same apartamento
            ValorTotal = 250000m,
            DataVenda = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/vendas", secondVendaDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetVenda_WithValidId_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create venda
        var (clienteId, apartamentoId) = await CreateClienteAndApartamentoAsync();
        var createDto = new CreateVendaDto
        {
            ClienteId = clienteId,
            ApartamentoId = apartamentoId,
            ValorTotal = 300000m,
            DataVenda = DateTime.UtcNow
        };

        var createResponse = await _client.PostAsJsonAsync("/api/vendas", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdVenda = JsonSerializer.Deserialize<VendaDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.GetAsync($"/api/vendas/{createdVenda.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var venda = JsonSerializer.Deserialize<VendaDto>(content, _jsonOptions);
        
        venda.Should().NotBeNull();
        venda.Id.Should().Be(createdVenda.Id);
        venda.ValorTotal.Should().Be(createDto.ValorTotal);
    }

    [Fact]
    public async Task UpdateVenda_WithValidData_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create venda
        var (clienteId, apartamentoId) = await CreateClienteAndApartamentoAsync();
        var createDto = new CreateVendaDto
        {
            ClienteId = clienteId,
            ApartamentoId = apartamentoId,
            ValorTotal = 280000m,
            DataVenda = DateTime.UtcNow
        };

        var createResponse = await _client.PostAsJsonAsync("/api/vendas", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdVenda = JsonSerializer.Deserialize<VendaDto>(createContent, _jsonOptions);

        // Prepare update
        var updateDto = new UpdateVendaDto
        {
            ClienteId = clienteId,
            ApartamentoId = apartamentoId,
            ValorTotal = 290000m, // Updated
            ValorEntrada = 58000m, // Updated
            StatusVenda = "Em Análise", // Updated
            DataVenda = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/vendas/{createdVenda.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var updatedVenda = JsonSerializer.Deserialize<VendaDto>(content, _jsonOptions);
        
        updatedVenda.Should().NotBeNull();
        updatedVenda.ValorTotal.Should().Be(updateDto.ValorTotal);
        updatedVenda.ValorEntrada.Should().Be(updateDto.ValorEntrada);
        updatedVenda.StatusVenda.Should().Be(updateDto.StatusVenda);
    }

    [Fact]
    public async Task ConfirmVenda_WithPendingVenda_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create pending venda
        var (clienteId, apartamentoId) = await CreateClienteAndApartamentoAsync();
        var createDto = new CreateVendaDto
        {
            ClienteId = clienteId,
            ApartamentoId = apartamentoId,
            ValorTotal = 320000m,
            DataVenda = DateTime.UtcNow
        };

        var createResponse = await _client.PostAsJsonAsync("/api/vendas", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdVenda = JsonSerializer.Deserialize<VendaDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.PostAsync($"/api/vendas/{createdVenda.Id}/confirmar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var confirmedVenda = JsonSerializer.Deserialize<VendaDto>(content, _jsonOptions);
        
        confirmedVenda.Should().NotBeNull();
        confirmedVenda.StatusVenda.Should().Be("Confirmada");
    }

    [Fact]
    public async Task CancelVenda_WithPendingVenda_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create pending venda
        var (clienteId, apartamentoId) = await CreateClienteAndApartamentoAsync();
        var createDto = new CreateVendaDto
        {
            ClienteId = clienteId,
            ApartamentoId = apartamentoId,
            ValorTotal = 310000m,
            DataVenda = DateTime.UtcNow
        };

        var createResponse = await _client.PostAsJsonAsync("/api/vendas", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdVenda = JsonSerializer.Deserialize<VendaDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.PostAsync($"/api/vendas/{createdVenda.Id}/cancelar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var cancelledVenda = JsonSerializer.Deserialize<VendaDto>(content, _jsonOptions);
        
        cancelledVenda.Should().NotBeNull();
        cancelledVenda.StatusVenda.Should().Be("Cancelada");
    }

    [Fact]
    public async Task DeleteVenda_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create venda
        var (clienteId, apartamentoId) = await CreateClienteAndApartamentoAsync();
        var createDto = new CreateVendaDto
        {
            ClienteId = clienteId,
            ApartamentoId = apartamentoId,
            ValorTotal = 260000m,
            DataVenda = DateTime.UtcNow
        };

        var createResponse = await _client.PostAsJsonAsync("/api/vendas", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdVenda = JsonSerializer.Deserialize<VendaDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.DeleteAsync($"/api/vendas/{createdVenda.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify venda was deleted
        var getResponse = await _client.GetAsync($"/api/vendas/{createdVenda.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
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

    private async Task<int> CreateClienteAsync(string nome)
    {
        var createClienteDto = new CreateClienteDto
        {
            Nome = nome,
            CPF = Guid.NewGuid().ToString("N")[..11].Insert(3, ".").Insert(7, ".").Insert(11, "-"),
            Email = $"{nome.Replace(" ", "").ToLower()}@test.com"
        };

        var response = await _client.PostAsJsonAsync("/api/clientes", createClienteDto);
        var content = await response.Content.ReadAsStringAsync();
        var cliente = JsonSerializer.Deserialize<ClienteDto>(content, _jsonOptions);
        
        return cliente.Id;
    }

    private async Task<int> CreateApartamentoAsync(string numero, string bloco)
    {
        var createApartamentoDto = new CreateApartamentoDto
        {
            Numero = numero,
            Bloco = bloco,
            TipoApartamento = "Apartamento 2 Quartos",
            Quartos = 2,
            Banheiros = 1,
            Preco = 250000m
        };

        var response = await _client.PostAsJsonAsync("/api/apartamentos", createApartamentoDto);
        var content = await response.Content.ReadAsStringAsync();
        var apartamento = JsonSerializer.Deserialize<ApartamentoDto>(content, _jsonOptions);
        
        return apartamento.Id;
    }

    private async Task<(int ClienteId, int ApartamentoId)> CreateClienteAndApartamentoAsync()
    {
        var guid = Guid.NewGuid().ToString("N")[..8];
        var clienteId = await CreateClienteAsync($"Cliente Test {guid}");
        var apartamentoId = await CreateApartamentoAsync($"Apt{guid}", "T");
        
        return (clienteId, apartamentoId);
    }
}
