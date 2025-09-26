using DirecionalApi.Application.DTOs;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Domain.Interfaces;

namespace DirecionalApi.Application.Services;

public class VendaService
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IApartamentoRepository _apartamentoRepository;

    public VendaService(
        IVendaRepository vendaRepository, 
        IClienteRepository clienteRepository,
        IApartamentoRepository apartamentoRepository)
    {
        _vendaRepository = vendaRepository;
        _clienteRepository = clienteRepository;
        _apartamentoRepository = apartamentoRepository;
    }

    public async Task<IEnumerable<VendaDto>> GetAllAsync()
    {
        var vendas = await _vendaRepository.GetAllAsync();
        return vendas.Select(MapToDto);
    }

    public async Task<VendaDto?> GetByIdAsync(int id)
    {
        var venda = await _vendaRepository.GetByIdAsync(id);
        return venda != null ? MapToDto(venda) : null;
    }

    public async Task<IEnumerable<VendaDto>> GetByClienteIdAsync(int clienteId)
    {
        var vendas = await _vendaRepository.GetByClienteIdAsync(clienteId);
        return vendas.Select(MapToDto);
    }

    public async Task<IEnumerable<VendaDto>> GetByStatusAsync(string status)
    {
        var vendas = await _vendaRepository.GetByStatusAsync(status);
        return vendas.Select(MapToDto);
    }

    public async Task<VendaDto> CreateAsync(CreateVendaDto createVendaDto)
    {
        // Verificar se cliente existe
        var cliente = await _clienteRepository.GetByIdAsync(createVendaDto.ClienteId);
        if (cliente == null)
            throw new ArgumentException("Cliente não encontrado.");

        // Verificar se apartamento existe e está disponível
        var apartamento = await _apartamentoRepository.GetByIdAsync(createVendaDto.ApartamentoId);
        if (apartamento == null)
            throw new ArgumentException("Apartamento não encontrado.");

        if (apartamento.StatusApartamento != "Disponível")
            throw new ArgumentException("Apartamento não está disponível para venda.");

        var venda = new Venda
        {
            ClienteId = createVendaDto.ClienteId,
            ApartamentoId = createVendaDto.ApartamentoId,
            ValorVenda = createVendaDto.ValorVenda,
            FormaPagamento = createVendaDto.FormaPagamento,
            ValorEntrada = createVendaDto.ValorEntrada,
            NumeroParcelas = createVendaDto.NumeroParcelas,
            ValorParcela = createVendaDto.ValorParcela,
            DataPrimeiraParcela = createVendaDto.DataPrimeiraParcela,
            Vendedor = createVendaDto.Vendedor,
            ComissaoVendedor = createVendaDto.ComissaoVendedor,
            Observacoes = createVendaDto.Observacoes
        };

        var createdVenda = await _vendaRepository.CreateAsync(venda);

        // Atualizar status do apartamento para "Vendido"
        apartamento.StatusApartamento = "Vendido";
        await _apartamentoRepository.UpdateAsync(apartamento);

        return MapToDto(createdVenda);
    }

    public async Task<VendaDto> UpdateAsync(int id, UpdateVendaDto updateVendaDto)
    {
        var venda = await _vendaRepository.GetByIdAsync(id);
        if (venda == null)
            throw new ArgumentException("Venda não encontrada.");

        venda.ValorVenda = updateVendaDto.ValorVenda;
        venda.FormaPagamento = updateVendaDto.FormaPagamento;
        venda.ValorEntrada = updateVendaDto.ValorEntrada;
        venda.NumeroParcelas = updateVendaDto.NumeroParcelas;
        venda.ValorParcela = updateVendaDto.ValorParcela;
        venda.DataPrimeiraParcela = updateVendaDto.DataPrimeiraParcela;
        venda.StatusVenda = updateVendaDto.StatusVenda;
        venda.Vendedor = updateVendaDto.Vendedor;
        venda.ComissaoVendedor = updateVendaDto.ComissaoVendedor;
        venda.Observacoes = updateVendaDto.Observacoes;
        venda.DataQuitacao = updateVendaDto.DataQuitacao;

        var updatedVenda = await _vendaRepository.UpdateAsync(venda);
        return MapToDto(updatedVenda);
    }

    public async Task DeleteAsync(int id)
    {
        var venda = await _vendaRepository.GetByIdAsync(id);
        if (venda == null)
            throw new ArgumentException("Venda não encontrada.");

        // Voltar status do apartamento para "Disponível" se a venda for cancelada
        if (venda.Apartamento != null)
        {
            venda.Apartamento.StatusApartamento = "Disponível";
            await _apartamentoRepository.UpdateAsync(venda.Apartamento);
        }

        await _vendaRepository.DeleteAsync(id);
    }

    private static VendaDto MapToDto(Venda venda)
    {
        return new VendaDto
        {
            VendaId = venda.VendaId,
            ClienteId = venda.ClienteId,
            ApartamentoId = venda.ApartamentoId,
            DataVenda = venda.DataVenda,
            ValorVenda = venda.ValorVenda,
            FormaPagamento = venda.FormaPagamento,
            ValorEntrada = venda.ValorEntrada,
            NumeroParcelas = venda.NumeroParcelas,
            ValorParcela = venda.ValorParcela,
            DataPrimeiraParcela = venda.DataPrimeiraParcela,
            StatusVenda = venda.StatusVenda,
            Vendedor = venda.Vendedor,
            ComissaoVendedor = venda.ComissaoVendedor,
            Observacoes = venda.Observacoes,
            DataQuitacao = venda.DataQuitacao,
            Cliente = venda.Cliente != null ? MapClienteToDto(venda.Cliente) : null,
            Apartamento = venda.Apartamento != null ? MapApartamentoToDto(venda.Apartamento) : null
        };
    }

    private static ClienteDto MapClienteToDto(Cliente cliente)
    {
        return new ClienteDto
        {
            ClienteId = cliente.ClienteId,
            Nome = cliente.Nome,
            Cpf = cliente.Cpf,
            Email = cliente.Email,
            Telefone = cliente.Telefone,
            StatusCliente = cliente.StatusCliente
        };
    }

    private static ApartamentoDto MapApartamentoToDto(Apartamento apartamento)
    {
        return new ApartamentoDto
        {
            ApartamentoId = apartamento.ApartamentoId,
            NumeroApartamento = apartamento.NumeroApartamento,
            Bloco = apartamento.Bloco,
            Andar = apartamento.Andar,
            AreaTotal = apartamento.AreaTotal,
            Quartos = apartamento.Quartos,
            ValorVenda = apartamento.ValorVenda,
            StatusApartamento = apartamento.StatusApartamento,
            Empreendimento = apartamento.Empreendimento
        };
    }
}
