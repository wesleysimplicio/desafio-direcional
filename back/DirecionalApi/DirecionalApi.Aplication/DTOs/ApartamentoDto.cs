namespace DirecionalApi.Application.DTOs;

public class ApartamentoDto
{
    public int ApartamentoId { get; set; }
    public string NumeroApartamento { get; set; } = string.Empty;
    public string? Bloco { get; set; }
    public int? Andar { get; set; }
    public decimal AreaTotal { get; set; }
    public decimal? AreaPrivativa { get; set; }
    public int Quartos { get; set; }
    public int Suites { get; set; }
    public int Banheiros { get; set; }
    public int VagasGaragem { get; set; }
    public bool Varanda { get; set; }
    public decimal ValorVenda { get; set; }
    public decimal? ValorCondominio { get; set; }
    public string StatusApartamento { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
    public string? Descricao { get; set; }
    public string? Empreendimento { get; set; }
    public DateTime? EntregaPrevista { get; set; }
}

public class CreateApartamentoDto
{
    public string NumeroApartamento { get; set; } = string.Empty;
    public string? Bloco { get; set; }
    public int? Andar { get; set; }
    public decimal AreaTotal { get; set; }
    public decimal? AreaPrivativa { get; set; }
    public int Quartos { get; set; }
    public int Suites { get; set; }
    public int Banheiros { get; set; }
    public int VagasGaragem { get; set; }
    public bool Varanda { get; set; }
    public decimal ValorVenda { get; set; }
    public decimal? ValorCondominio { get; set; }
    public string? Descricao { get; set; }
    public string? Empreendimento { get; set; }
    public DateTime? EntregaPrevista { get; set; }
}

public class UpdateApartamentoDto
{
    public string NumeroApartamento { get; set; } = string.Empty;
    public string? Bloco { get; set; }
    public int? Andar { get; set; }
    public decimal AreaTotal { get; set; }
    public decimal? AreaPrivativa { get; set; }
    public int Quartos { get; set; }
    public int Suites { get; set; }
    public int Banheiros { get; set; }
    public int VagasGaragem { get; set; }
    public bool Varanda { get; set; }
    public decimal ValorVenda { get; set; }
    public decimal? ValorCondominio { get; set; }
    public string StatusApartamento { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Empreendimento { get; set; }
    public DateTime? EntregaPrevista { get; set; }
}
