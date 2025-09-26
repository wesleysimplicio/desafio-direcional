namespace DirecionalApi.Domain.Entities;

public class Venda
{
    public int VendaId { get; set; }
    public int ClienteId { get; set; }
    public int ApartamentoId { get; set; }
    public DateTime DataVenda { get; set; } = DateTime.Now;
    public decimal ValorVenda { get; set; }
    public string? FormaPagamento { get; set; }
    public decimal? ValorEntrada { get; set; }
    public int? NumeroParcelas { get; set; }
    public decimal? ValorParcela { get; set; }
    public DateTime? DataPrimeiraParcela { get; set; }
    public string StatusVenda { get; set; } = "Ativa";
    public string? Vendedor { get; set; }
    public decimal? ComissaoVendedor { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? DataQuitacao { get; set; }

    // Navigation properties
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual Apartamento Apartamento { get; set; } = null!;
}
