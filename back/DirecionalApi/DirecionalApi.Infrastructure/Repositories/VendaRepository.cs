using Microsoft.EntityFrameworkCore;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Domain.Interfaces;
using DirecionalApi.Infrastructure.Data;

namespace DirecionalApi.Infrastructure.Repositories;

public class VendaRepository : IVendaRepository
{
    private readonly DirecionalDbContext _context;

    public VendaRepository(DirecionalDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Venda>> GetAllAsync()
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Apartamento)
            .ToListAsync();
    }

    public async Task<Venda?> GetByIdAsync(int id)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Apartamento)
            .FirstOrDefaultAsync(v => v.VendaId == id);
    }

    public async Task<IEnumerable<Venda>> GetByClienteIdAsync(int clienteId)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Apartamento)
            .Where(v => v.ClienteId == clienteId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venda>> GetByStatusAsync(string status)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Apartamento)
            .Where(v => v.StatusVenda == status)
            .ToListAsync();
    }

    public async Task<Venda> CreateAsync(Venda venda)
    {
        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync();
        return venda;
    }

    public async Task<Venda> UpdateAsync(Venda venda)
    {
        _context.Entry(venda).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return venda;
    }

    public async Task DeleteAsync(int id)
    {
        var venda = await _context.Vendas.FindAsync(id);
        if (venda != null)
        {
            _context.Vendas.Remove(venda);
            await _context.SaveChangesAsync();
        }
    }
}
