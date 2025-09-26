using DirecionalApi.Domain.Entities;

namespace DirecionalApi.Domain.Interfaces;

public interface IApartamentoRepository
{
    Task<IEnumerable<Apartamento>> GetAllAsync();
    Task<Apartamento?> GetByIdAsync(int id);
    Task<IEnumerable<Apartamento>> GetByStatusAsync(string status);
    Task<Apartamento> CreateAsync(Apartamento apartamento);
    Task<Apartamento> UpdateAsync(Apartamento apartamento);
    Task DeleteAsync(int id);
}
