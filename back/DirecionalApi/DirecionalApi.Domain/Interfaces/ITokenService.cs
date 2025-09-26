namespace DirecionalApi.Domain.Interfaces;

public interface ITokenService
{
    string GenerateToken(string username, string email, string role);
}
