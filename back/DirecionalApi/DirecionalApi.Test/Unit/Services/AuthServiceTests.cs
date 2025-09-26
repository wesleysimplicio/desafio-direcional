using AutoFixture;
using AutoFixture.Xunit2;
using DirecionalApi.Aplication.DTOs.Auth;
using DirecionalApi.Aplication.Services;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace DirecionalApi.Test.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;
    private readonly Fixture _fixture;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _fixture = new Fixture();

        // Setup JWT configuration
        _configurationMock.Setup(x => x["Jwt:Key"]).Returns("test-secret-key-with-at-least-32-characters");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("test-issuer");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("test-audience");

        _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
    }

    [Theory]
    [AutoData]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse(string email, string password, string nome)
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(u => u.Email, email)
            .With(u => u.Nome, nome)
            .With(u => u.PasswordHash, BCrypt.Net.BCrypt.HashPassword(password))
            .Create();

        var loginRequest = new LoginRequest { Email = email, Password = password };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(email);
        result.User.Nome.Should().Be(nome);
    }

    [Theory]
    [AutoData]
    public async Task LoginAsync_WithInvalidEmail_ReturnsFailure(string email, string password)
    {
        // Arrange
        var loginRequest = new LoginRequest { Email = email, Password = password };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email ou senha inválidos");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task LoginAsync_WithInvalidPassword_ReturnsFailure(string email, string correctPassword, string wrongPassword, string nome)
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(u => u.Email, email)
            .With(u => u.Nome, nome)
            .With(u => u.PasswordHash, BCrypt.Net.BCrypt.HashPassword(correctPassword))
            .Create();

        var loginRequest = new LoginRequest { Email = email, Password = wrongPassword };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email ou senha inválidos");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await _authService.Invoking(x => x.LoginAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("email", "")]
    [InlineData(null, "password")]
    [InlineData("email", null)]
    public async Task LoginAsync_WithInvalidInput_ReturnsFailure(string email, string password)
    {
        // Arrange
        var loginRequest = new LoginRequest { Email = email, Password = password };

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email e senha são obrigatórios");
    }

    [Theory]
    [AutoData]
    public async Task ValidateTokenAsync_WithValidToken_ReturnsUser(string email, string nome)
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(u => u.Email, email)
            .With(u => u.Nome, nome)
            .Create();

        // First generate a token
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        var loginRequest = new LoginRequest { Email = email, Password = "TestPassword123!" };
        var loginResult = await _authService.LoginAsync(loginRequest);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.ValidateTokenAsync(loginResult.Token!);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
        result.Nome.Should().Be(nome);
    }

    [Fact]
    public async Task ValidateTokenAsync_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid-token";

        // Act
        var result = await _authService.ValidateTokenAsync(invalidToken);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task ValidateTokenAsync_WithNullOrEmptyToken_ReturnsNull(string token)
    {
        // Act
        var result = await _authService.ValidateTokenAsync(token);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task GetCurrentUserAsync_WithValidClaims_ReturnsUser(int userId, string email, string nome)
    {
        // Arrange
        var claims = new[]
        {
            new Claim("sub", userId.ToString()),
            new Claim("email", email),
            new Claim("name", nome)
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        var user = _fixture.Build<User>()
            .With(u => u.Id, userId)
            .With(u => u.Email, email)
            .With(u => u.Nome, nome)
            .Create();

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.GetCurrentUserAsync(principal);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Email.Should().Be(email);
        result.Nome.Should().Be(nome);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithNullPrincipal_ReturnsNull()
    {
        // Act
        var result = await _authService.GetCurrentUserAsync(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithInvalidClaims_ReturnsNull()
    {
        // Arrange
        var claims = new[]
        {
            new Claim("invalid", "claim")
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await _authService.GetCurrentUserAsync(principal);

        // Assert
        result.Should().BeNull();
    }
}
