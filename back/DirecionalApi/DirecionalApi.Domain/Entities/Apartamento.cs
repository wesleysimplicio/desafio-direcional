namespace DirecionalApi.Domain.Entities;

public class Apartamento
{
    public int ApartamentoId { get; set; }
    public string NumeroApartamento { get; set; } = string.Empty;
    public string? Bloco { get; set; }
    public int? Andar { get; set; }
    public decimal AreaTotal { get; set; }
    public decimal? AreaPrivativa { get; set; }
    public int Quartos { get; set; }
    public int Suites { get; set; } = 0;
    public int Banheiros { get; set; }
    public int VagasGaragem { get; set; } = 0;
    public bool Varanda { get; set; } = false;
    public decimal ValorVenda { get; set; }
    public decimal? ValorCondominio { get; set; }
    public string StatusApartamento { get; set; } = "Disponível";
    public DateTime DataCadastro { get; set; } = DateTime.Now;
    public string? Descricao { get; set; }
    public string? Empreendimento { get; set; }
    public DateTime? EntregaPrevista { get; set; }

    // Navigation properties
    public virtual ICollection<Venda> Vendas { get; set; } = new List<Venda>();
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
