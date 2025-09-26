namespace DirecionalApi.Application.DTOs;

public class ClienteDto
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }
    public DateTime? DataNascimento { get; set; }
    public decimal? RendaMensal { get; set; }
    public string StatusCliente { get; set; } = "Ativo";
    public DateTime DataCadastro { get; set; }
    public string? Observacoes { get; set; }
}

public class CreateClienteDto
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }
    public DateTime? DataNascimento { get; set; }
    public decimal? RendaMensal { get; set; }
    public string? Observacoes { get; set; }
}

public class UpdateClienteDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }
    public DateTime? DataNascimento { get; set; }
    public decimal? RendaMensal { get; set; }
    public string StatusCliente { get; set; } = "Ativo";
    public string? Observacoes { get; set; }
}
