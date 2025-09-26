namespace DirecionalApi.Domain.Entities;

public class Reserva
{
    public int ReservaId { get; set; }
    public int ClienteId { get; set; }
    public int ApartamentoId { get; set; }
    public DateTime DataReserva { get; set; } = DateTime.Now;
    public decimal ValorReserva { get; set; }
    public DateTime DataValidadeReserva { get; set; }
    public string StatusReserva { get; set; } = "Ativa";
    public string? FormaPagamentoReserva { get; set; }
    public string? Vendedor { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? DataConversaoVenda { get; set; }
    public int? VendaId { get; set; }

    // Navigation properties
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual Apartamento Apartamento { get; set; } = null!;
    public virtual Venda? Venda { get; set; }
}
