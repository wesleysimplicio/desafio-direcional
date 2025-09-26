namespace DirecionalApi.Application.DTOs;

public class VendaDto
{
    public int VendaId { get; set; }
    public int ClienteId { get; set; }
    public int ApartamentoId { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal ValorVenda { get; set; }
    public string? FormaPagamento { get; set; }
    public decimal? ValorEntrada { get; set; }
    public int? NumeroParcelas { get; set; }
    public decimal? ValorParcela { get; set; }
    public DateTime? DataPrimeiraParcela { get; set; }
    public string StatusVenda { get; set; } = string.Empty;
    public string? Vendedor { get; set; }
    public decimal? ComissaoVendedor { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? DataQuitacao { get; set; }

    public ClienteDto? Cliente { get; set; }
    public ApartamentoDto? Apartamento { get; set; }
}

public class CreateVendaDto
{
    public int ClienteId { get; set; }
    public int ApartamentoId { get; set; }
    public decimal ValorVenda { get; set; }
    public string? FormaPagamento { get; set; }
    public decimal? ValorEntrada { get; set; }
    public int? NumeroParcelas { get; set; }
    public decimal? ValorParcela { get; set; }
    public DateTime? DataPrimeiraParcela { get; set; }
    public string? Vendedor { get; set; }
    public decimal? ComissaoVendedor { get; set; }
    public string? Observacoes { get; set; }
}

public class UpdateVendaDto
{
    public decimal ValorVenda { get; set; }
    public string? FormaPagamento { get; set; }
    public decimal? ValorEntrada { get; set; }
    public int? NumeroParcelas { get; set; }
    public decimal? ValorParcela { get; set; }
    public DateTime? DataPrimeiraParcela { get; set; }
    public string StatusVenda { get; set; } = string.Empty;
    public string? Vendedor { get; set; }
    public decimal? ComissaoVendedor { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? DataQuitacao { get; set; }
}
