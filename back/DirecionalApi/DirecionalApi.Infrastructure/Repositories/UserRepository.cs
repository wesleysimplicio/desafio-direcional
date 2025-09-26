using Microsoft.EntityFrameworkCore;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Domain.Interfaces;
using DirecionalApi.Infrastructure.Data;
using BCrypt.Net;

namespace DirecionalApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DirecionalDbContext _context;

    public UserRepository(DirecionalDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateAsync(User user)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        var user = await GetByUsernameAsync(username);
        if (user == null || !user.IsActive)
            return false;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }
}
