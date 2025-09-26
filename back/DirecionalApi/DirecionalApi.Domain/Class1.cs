namespace DirecionalApi.Domain.Entities;

public class Cliente
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
    public DateTime DataCadastro { get; set; } = DateTime.Now;
    public string? Observacoes { get; set; }

    // Navigation properties
    public virtual ICollection<Venda> Vendas { get; set; } = new List<Venda>();
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
