using DirecionalApi.Application.DTOs;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Domain.Interfaces;

namespace DirecionalApi.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var isValid = await _userRepository.ValidateCredentialsAsync(loginDto.Username, loginDto.Password);
        
        if (!isValid)
            return null;

        var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
        if (user == null || !user.IsActive)
            return null;

        var token = _tokenService.GenerateToken(user.Username, user.Email, user.Role);

        return new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddHours(8)
        };
    }

    public async Task<bool> RegisterAsync(RegisterDto registerDto)
    {
        // Verificar se usuário já existe
        var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username);
        if (existingUser != null)
            return false;

        // Verificar se email já está em uso
        var existingEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingEmail != null)
            return false;

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = registerDto.Password, // Será hasheado no repositório
            Role = registerDto.Role
        };

        await _userRepository.CreateAsync(user);
        return true;
    }
}
