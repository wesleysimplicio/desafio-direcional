using DirecionalApi.Application.DTOs;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Domain.Interfaces;

namespace DirecionalApi.Application.Services;

public class ApartamentoService
{
    private readonly IApartamentoRepository _apartamentoRepository;

    public ApartamentoService(IApartamentoRepository apartamentoRepository)
    {
        _apartamentoRepository = apartamentoRepository;
    }

    public async Task<IEnumerable<ApartamentoDto>> GetAllAsync()
    {
        var apartamentos = await _apartamentoRepository.GetAllAsync();
        return apartamentos.Select(MapToDto);
    }

    public async Task<ApartamentoDto?> GetByIdAsync(int id)
    {
        var apartamento = await _apartamentoRepository.GetByIdAsync(id);
        return apartamento != null ? MapToDto(apartamento) : null;
    }

    public async Task<IEnumerable<ApartamentoDto>> GetByStatusAsync(string status)
    {
        var apartamentos = await _apartamentoRepository.GetByStatusAsync(status);
        return apartamentos.Select(MapToDto);
    }

    public async Task<ApartamentoDto> CreateAsync(CreateApartamentoDto createApartamentoDto)
    {
        var apartamento = new Apartamento
        {
            NumeroApartamento = createApartamentoDto.NumeroApartamento,
            Bloco = createApartamentoDto.Bloco,
            Andar = createApartamentoDto.Andar,
            AreaTotal = createApartamentoDto.AreaTotal,
            AreaPrivativa = createApartamentoDto.AreaPrivativa,
            Quartos = createApartamentoDto.Quartos,
            Suites = createApartamentoDto.Suites,
            Banheiros = createApartamentoDto.Banheiros,
            VagasGaragem = createApartamentoDto.VagasGaragem,
            Varanda = createApartamentoDto.Varanda,
            ValorVenda = createApartamentoDto.ValorVenda,
            ValorCondominio = createApartamentoDto.ValorCondominio,
            Descricao = createApartamentoDto.Descricao,
            Empreendimento = createApartamentoDto.Empreendimento,
            EntregaPrevista = createApartamentoDto.EntregaPrevista
        };

        var createdApartamento = await _apartamentoRepository.CreateAsync(apartamento);
        return MapToDto(createdApartamento);
    }

    public async Task<ApartamentoDto> UpdateAsync(int id, UpdateApartamentoDto updateApartamentoDto)
    {
        var apartamento = await _apartamentoRepository.GetByIdAsync(id);
        if (apartamento == null)
            throw new ArgumentException("Apartamento não encontrado.");

        apartamento.NumeroApartamento = updateApartamentoDto.NumeroApartamento;
        apartamento.Bloco = updateApartamentoDto.Bloco;
        apartamento.Andar = updateApartamentoDto.Andar;
        apartamento.AreaTotal = updateApartamentoDto.AreaTotal;
        apartamento.AreaPrivativa = updateApartamentoDto.AreaPrivativa;
        apartamento.Quartos = updateApartamentoDto.Quartos;
        apartamento.Suites = updateApartamentoDto.Suites;
        apartamento.Banheiros = updateApartamentoDto.Banheiros;
        apartamento.VagasGaragem = updateApartamentoDto.VagasGaragem;
        apartamento.Varanda = updateApartamentoDto.Varanda;
        apartamento.ValorVenda = updateApartamentoDto.ValorVenda;
        apartamento.ValorCondominio = updateApartamentoDto.ValorCondominio;
        apartamento.StatusApartamento = updateApartamentoDto.StatusApartamento;
        apartamento.Descricao = updateApartamentoDto.Descricao;
        apartamento.Empreendimento = updateApartamentoDto.Empreendimento;
        apartamento.EntregaPrevista = updateApartamentoDto.EntregaPrevista;

        var updatedApartamento = await _apartamentoRepository.UpdateAsync(apartamento);
        return MapToDto(updatedApartamento);
    }

    public async Task DeleteAsync(int id)
    {
        var apartamento = await _apartamentoRepository.GetByIdAsync(id);
        if (apartamento == null)
            throw new ArgumentException("Apartamento não encontrado.");

        await _apartamentoRepository.DeleteAsync(id);
    }

    private static ApartamentoDto MapToDto(Apartamento apartamento)
    {
        return new ApartamentoDto
        {
            ApartamentoId = apartamento.ApartamentoId,
            NumeroApartamento = apartamento.NumeroApartamento,
            Bloco = apartamento.Bloco,
            Andar = apartamento.Andar,
            AreaTotal = apartamento.AreaTotal,
            AreaPrivativa = apartamento.AreaPrivativa,
            Quartos = apartamento.Quartos,
            Suites = apartamento.Suites,
            Banheiros = apartamento.Banheiros,
            VagasGaragem = apartamento.VagasGaragem,
            Varanda = apartamento.Varanda,
            ValorVenda = apartamento.ValorVenda,
            ValorCondominio = apartamento.ValorCondominio,
            StatusApartamento = apartamento.StatusApartamento,
            DataCadastro = apartamento.DataCadastro,
            Descricao = apartamento.Descricao,
            Empreendimento = apartamento.Empreendimento,
            EntregaPrevista = apartamento.EntregaPrevista
        };
    }
}
