using DirecionalApi.Aplication.DTOs.Auth;
using DirecionalApi.Aplication.DTOs.Cliente;
using DirecionalApi.Web;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;

namespace DirecionalApi.Test.Integration.Controllers;

public class ClientesControllerTests : IClassFixture<DirecionalApiWebApplicationFactory<Program>>
{
    private readonly DirecionalApiWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ClientesControllerTests(DirecionalApiWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task GetClientes_WithoutAuth_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/clientes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetClientes_WithAuth_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/clientes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var clientes = JsonSerializer.Deserialize<List<ClienteDto>>(content, _jsonOptions);
        
        clientes.Should().NotBeNull();
        clientes.Should().BeOfType<List<ClienteDto>>();
    }

    [Fact]
    public async Task CreateCliente_WithValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateClienteDto
        {
            Nome = "João Silva Test",
            CPF = "123.456.789-00",
            Email = "joao.test@email.com",
            Telefone = "(11) 99999-9999"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/clientes", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var cliente = JsonSerializer.Deserialize<ClienteDto>(content, _jsonOptions);
        
        cliente.Should().NotBeNull();
        cliente.Nome.Should().Be(createDto.Nome);
        cliente.CPF.Should().Be(createDto.CPF);
        cliente.Email.Should().Be(createDto.Email);
    }

    [Fact]
    public async Task CreateCliente_WithDuplicateCPF_ReturnsConflict()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateClienteDto
        {
            Nome = "Maria Silva Test",
            CPF = "111.222.333-44",
            Email = "maria.test@email.com"
        };

        // Create first cliente
        await _client.PostAsJsonAsync("/api/clientes", createDto);

        // Try to create another with same CPF
        var duplicateDto = new CreateClienteDto
        {
            Nome = "Ana Silva Test",
            CPF = "111.222.333-44", // Same CPF
            Email = "ana.test@email.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/clientes", duplicateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateCliente_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateClienteDto
        {
            Nome = "", // Invalid - empty name
            CPF = "invalid-cpf", // Invalid CPF format
            Email = "invalid-email" // Invalid email format
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/clientes", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCliente_WithValidId_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create a cliente
        var createDto = new CreateClienteDto
        {
            Nome = "Pedro Santos Test",
            CPF = "555.666.777-88",
            Email = "pedro.test@email.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/clientes", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdCliente = JsonSerializer.Deserialize<ClienteDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.GetAsync($"/api/clientes/{createdCliente.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var cliente = JsonSerializer.Deserialize<ClienteDto>(content, _jsonOptions);
        
        cliente.Should().NotBeNull();
        cliente.Id.Should().Be(createdCliente.Id);
        cliente.Nome.Should().Be(createDto.Nome);
    }

    [Fact]
    public async Task GetCliente_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/clientes/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCliente_WithValidData_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create a cliente
        var createDto = new CreateClienteDto
        {
            Nome = "Carlos Lima Test",
            CPF = "777.888.999-00",
            Email = "carlos.test@email.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/clientes", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdCliente = JsonSerializer.Deserialize<ClienteDto>(createContent, _jsonOptions);

        // Prepare update
        var updateDto = new UpdateClienteDto
        {
            Nome = "Carlos Lima Santos Test", // Updated name
            Email = "carlos.santos.test@email.com", // Updated email
            Telefone = "(11) 88888-8888"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/clientes/{createdCliente.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var updatedCliente = JsonSerializer.Deserialize<ClienteDto>(content, _jsonOptions);
        
        updatedCliente.Should().NotBeNull();
        updatedCliente.Nome.Should().Be(updateDto.Nome);
        updatedCliente.Email.Should().Be(updateDto.Email);
        updatedCliente.Telefone.Should().Be(updateDto.Telefone);
    }

    [Fact]
    public async Task UpdateCliente_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateClienteDto
        {
            Nome = "Updated Name",
            Email = "updated@email.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/clientes/99999", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCliente_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create a cliente
        var createDto = new CreateClienteDto
        {
            Nome = "Delete Test Cliente",
            CPF = "000.111.222-33",
            Email = "delete.test@email.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/clientes", createDto);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdCliente = JsonSerializer.Deserialize<ClienteDto>(createContent, _jsonOptions);

        // Act
        var response = await _client.DeleteAsync($"/api/clientes/{createdCliente.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify cliente was deleted
        var getResponse = await _client.GetAsync($"/api/clientes/{createdCliente.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCliente_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/clientes/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetClienteByCPF_WithValidCPF_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create a cliente
        var createDto = new CreateClienteDto
        {
            Nome = "CPF Search Test",
            CPF = "444.555.666-77",
            Email = "cpf.search.test@email.com"
        };

        await _client.PostAsJsonAsync("/api/clientes", createDto);

        // Act
        var response = await _client.GetAsync($"/api/clientes/cpf/{createDto.CPF}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var cliente = JsonSerializer.Deserialize<ClienteDto>(content, _jsonOptions);
        
        cliente.Should().NotBeNull();
        cliente.CPF.Should().Be(createDto.CPF);
        cliente.Nome.Should().Be(createDto.Nome);
    }

    [Fact]
    public async Task GetClienteByCPF_WithInvalidCPF_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/clientes/cpf/999.999.999-99");

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
