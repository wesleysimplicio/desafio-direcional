using DirecionalApi.Domain.Entities;

namespace DirecionalApi.Domain.Interfaces;

public interface IVendaRepository
{
    Task<IEnumerable<Venda>> GetAllAsync();
    Task<Venda?> GetByIdAsync(int id);
    Task<IEnumerable<Venda>> GetByClienteIdAsync(int clienteId);
    Task<IEnumerable<Venda>> GetByStatusAsync(string status);
    Task<Venda> CreateAsync(Venda venda);
    Task<Venda> UpdateAsync(Venda venda);
    Task DeleteAsync(int id);
}
