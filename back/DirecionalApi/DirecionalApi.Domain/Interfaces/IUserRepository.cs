using DirecionalApi.Domain.Entities;

namespace DirecionalApi.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<bool> ValidateCredentialsAsync(string username, string password);
}
