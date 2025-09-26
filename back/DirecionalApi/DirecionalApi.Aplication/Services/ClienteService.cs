using DirecionalApi.Application.DTOs;
using DirecionalApi.Domain.Entities;
using DirecionalApi.Domain.Interfaces;

namespace DirecionalApi.Application.Services;

public class ClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<ClienteDto>> GetAllAsync()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        return clientes.Select(MapToDto);
    }

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        return cliente != null ? MapToDto(cliente) : null;
    }

    public async Task<ClienteDto> CreateAsync(CreateClienteDto createClienteDto)
    {
        // Verificar se CPF já existe
        var existingCliente = await _clienteRepository.GetByCpfAsync(createClienteDto.Cpf);
        if (existingCliente != null)
            throw new ArgumentException("CPF já cadastrado no sistema.");

        var cliente = new Cliente
        {
            Nome = createClienteDto.Nome,
            Cpf = createClienteDto.Cpf,
            Email = createClienteDto.Email,
            Telefone = createClienteDto.Telefone,
            Endereco = createClienteDto.Endereco,
            Cidade = createClienteDto.Cidade,
            Estado = createClienteDto.Estado,
            Cep = createClienteDto.Cep,
            DataNascimento = createClienteDto.DataNascimento,
            RendaMensal = createClienteDto.RendaMensal,
            Observacoes = createClienteDto.Observacoes
        };

        var createdCliente = await _clienteRepository.CreateAsync(cliente);
        return MapToDto(createdCliente);
    }

    public async Task<ClienteDto> UpdateAsync(int id, UpdateClienteDto updateClienteDto)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente == null)
            throw new ArgumentException("Cliente não encontrado.");

        cliente.Nome = updateClienteDto.Nome;
        cliente.Email = updateClienteDto.Email;
        cliente.Telefone = updateClienteDto.Telefone;
        cliente.Endereco = updateClienteDto.Endereco;
        cliente.Cidade = updateClienteDto.Cidade;
        cliente.Estado = updateClienteDto.Estado;
        cliente.Cep = updateClienteDto.Cep;
        cliente.DataNascimento = updateClienteDto.DataNascimento;
        cliente.RendaMensal = updateClienteDto.RendaMensal;
        cliente.StatusCliente = updateClienteDto.StatusCliente;
        cliente.Observacoes = updateClienteDto.Observacoes;

        var updatedCliente = await _clienteRepository.UpdateAsync(cliente);
        return MapToDto(updatedCliente);
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente == null)
            throw new ArgumentException("Cliente não encontrado.");

        await _clienteRepository.DeleteAsync(id);
    }

    private static ClienteDto MapToDto(Cliente cliente)
    {
        return new ClienteDto
        {
            ClienteId = cliente.ClienteId,
            Nome = cliente.Nome,
            Cpf = cliente.Cpf,
            Email = cliente.Email,
            Telefone = cliente.Telefone,
            Endereco = cliente.Endereco,
            Cidade = cliente.Cidade,
            Estado = cliente.Estado,
            Cep = cliente.Cep,
            DataNascimento = cliente.DataNascimento,
            RendaMensal = cliente.RendaMensal,
            StatusCliente = cliente.StatusCliente,
            DataCadastro = cliente.DataCadastro,
            Observacoes = cliente.Observacoes
        };
    }
}
