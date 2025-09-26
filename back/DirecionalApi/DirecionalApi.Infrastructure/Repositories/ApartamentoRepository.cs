using Microsoft.EntityFrameworkCore;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Domain.Interfaces;
using DirecionalApi.Infrastructure.Data;

namespace DirecionalApi.Infrastructure.Repositories;

public class ApartamentoRepository : IApartamentoRepository
{
    private readonly DirecionalDbContext _context;

    public ApartamentoRepository(DirecionalDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Apartamento>> GetAllAsync()
    {
        return await _context.Apartamentos.ToListAsync();
    }

    public async Task<Apartamento?> GetByIdAsync(int id)
    {
        return await _context.Apartamentos.FindAsync(id);
    }

    public async Task<IEnumerable<Apartamento>> GetByStatusAsync(string status)
    {
        return await _context.Apartamentos
            .Where(a => a.StatusApartamento == status)
            .ToListAsync();
    }

    public async Task<Apartamento> CreateAsync(Apartamento apartamento)
    {
        _context.Apartamentos.Add(apartamento);
        await _context.SaveChangesAsync();
        return apartamento;
    }

    public async Task<Apartamento> UpdateAsync(Apartamento apartamento)
    {
        _context.Entry(apartamento).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return apartamento;
    }

    public async Task DeleteAsync(int id)
    {
        var apartamento = await _context.Apartamentos.FindAsync(id);
        if (apartamento != null)
        {
            _context.Apartamentos.Remove(apartamento);
            await _context.SaveChangesAsync();
        }
    }
}
