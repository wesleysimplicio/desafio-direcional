using Microsoft.EntityFrameworkCore;
using DirecionalApi.Infrastructure.Data;

namespace DirecionalApi.Web.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DirecionalDbContext>();
        
        try
        {
            // Criar o banco se não existir
            await context.Database.EnsureCreatedAsync();
            
            // Aplicar migrations pendentes
            await context.Database.MigrateAsync();
            
            Console.WriteLine("Banco de dados inicializado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inicializar banco de dados: {ex.Message}");
            throw;
        }
    }
}
