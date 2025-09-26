using Microsoft.EntityFrameworkCore;
using DirecionalApi.Domain.Entities;

namespace DirecionalApi.Infrastructure.Data;

public class DirecionalDbContext : DbContext
{
    public DirecionalDbContext(DbContextOptions<DirecionalDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Apartamento> Apartamentos { get; set; }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cliente configuration
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("clientes");
            entity.HasKey(e => e.ClienteId).HasName("PK_clientes");
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Cpf).HasColumnName("cpf").HasMaxLength(11).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
            entity.Property(e => e.Telefone).HasColumnName("telefone").HasMaxLength(15);
            entity.Property(e => e.Endereco).HasColumnName("endereco").HasMaxLength(200);
            entity.Property(e => e.Cidade).HasColumnName("cidade").HasMaxLength(50);
            entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(2);
            entity.Property(e => e.Cep).HasColumnName("cep").HasMaxLength(8);
            entity.Property(e => e.DataNascimento).HasColumnName("data_nascimento").HasColumnType("date");
            entity.Property(e => e.RendaMensal).HasColumnName("renda_mensal").HasColumnType("decimal(10,2)");
            entity.Property(e => e.StatusCliente).HasColumnName("status_cliente").HasMaxLength(20).HasDefaultValue("Ativo");
            entity.Property(e => e.DataCadastro).HasColumnName("data_cadastro").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.Observacoes).HasColumnName("observacoes").HasMaxLength(500);
            
            entity.HasIndex(e => e.Cpf).IsUnique().HasDatabaseName("IX_clientes_cpf");
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_clientes_email");
        });

        // Apartamento configuration
        modelBuilder.Entity<Apartamento>(entity =>
        {
            entity.ToTable("apartamentos");
            entity.HasKey(e => e.ApartamentoId).HasName("PK_apartamentos");
            entity.Property(e => e.ApartamentoId).HasColumnName("apartamento_id");
            entity.Property(e => e.NumeroApartamento).HasColumnName("numero_apartamento").HasMaxLength(10).IsRequired();
            entity.Property(e => e.Bloco).HasColumnName("bloco").HasMaxLength(10);
            entity.Property(e => e.Andar).HasColumnName("andar");
            entity.Property(e => e.AreaTotal).HasColumnName("area_total").HasColumnType("decimal(8,2)").IsRequired();
            entity.Property(e => e.AreaPrivativa).HasColumnName("area_privativa").HasColumnType("decimal(8,2)");
            entity.Property(e => e.Quartos).HasColumnName("quartos").IsRequired();
            entity.Property(e => e.Suites).HasColumnName("suites").HasDefaultValue(0);
            entity.Property(e => e.Banheiros).HasColumnName("banheiros").IsRequired();
            entity.Property(e => e.VagasGaragem).HasColumnName("vagas_garagem").HasDefaultValue(0);
            entity.Property(e => e.Varanda).HasColumnName("varanda").HasDefaultValue(false);
            entity.Property(e => e.ValorVenda).HasColumnName("valor_venda").HasColumnType("decimal(12,2)").IsRequired();
            entity.Property(e => e.ValorCondominio).HasColumnName("valor_condominio").HasColumnType("decimal(8,2)");
            entity.Property(e => e.StatusApartamento).HasColumnName("status_apartamento").HasMaxLength(20).HasDefaultValue("Disponível");
            entity.Property(e => e.DataCadastro).HasColumnName("data_cadastro").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.Descricao).HasColumnName("descricao").HasMaxLength(500);
            entity.Property(e => e.Empreendimento).HasColumnName("empreendimento").HasMaxLength(100);
            entity.Property(e => e.EntregaPrevista).HasColumnName("entrega_prevista").HasColumnType("date");
        });

        // Venda configuration
        modelBuilder.Entity<Venda>(entity =>
        {
            entity.ToTable("vendas");
            entity.HasKey(e => e.VendaId).HasName("PK_vendas");
            entity.Property(e => e.VendaId).HasColumnName("venda_id");
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id").IsRequired();
            entity.Property(e => e.ApartamentoId).HasColumnName("apartamento_id").IsRequired();
            entity.Property(e => e.DataVenda).HasColumnName("data_venda").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.ValorVenda).HasColumnName("valor_venda").HasColumnType("decimal(12,2)").IsRequired();
            entity.Property(e => e.FormaPagamento).HasColumnName("forma_pagamento").HasMaxLength(50);
            entity.Property(e => e.ValorEntrada).HasColumnName("valor_entrada").HasColumnType("decimal(12,2)");
            entity.Property(e => e.NumeroParcelas).HasColumnName("numero_parcelas");
            entity.Property(e => e.ValorParcela).HasColumnName("valor_parcela").HasColumnType("decimal(10,2)");
            entity.Property(e => e.DataPrimeiraParcela).HasColumnName("data_primeira_parcela").HasColumnType("date");
            entity.Property(e => e.StatusVenda).HasColumnName("status_venda").HasMaxLength(20).HasDefaultValue("Ativa");
            entity.Property(e => e.Vendedor).HasColumnName("vendedor").HasMaxLength(100);
            entity.Property(e => e.ComissaoVendedor).HasColumnName("comissao_vendedor").HasColumnType("decimal(10,2)");
            entity.Property(e => e.Observacoes).HasColumnName("observacoes").HasMaxLength(500);
            entity.Property(e => e.DataQuitacao).HasColumnName("data_quitacao").HasColumnType("date");
            
            entity.HasOne(d => d.Cliente)
                .WithMany(p => p.Vendas)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_vendas_clientes");

            entity.HasOne(d => d.Apartamento)
                .WithMany(p => p.Vendas)
                .HasForeignKey(d => d.ApartamentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_vendas_apartamentos");
        });

        // Reserva configuration
        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.ToTable("reservas");
            entity.HasKey(e => e.ReservaId).HasName("PK_reservas");
            entity.Property(e => e.ReservaId).HasColumnName("reserva_id");
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id").IsRequired();
            entity.Property(e => e.ApartamentoId).HasColumnName("apartamento_id").IsRequired();
            entity.Property(e => e.DataReserva).HasColumnName("data_reserva").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.ValorReserva).HasColumnName("valor_reserva").HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(e => e.DataValidadeReserva).HasColumnName("data_validade_reserva").HasColumnType("date").IsRequired();
            entity.Property(e => e.StatusReserva).HasColumnName("status_reserva").HasMaxLength(20).HasDefaultValue("Ativa");
            entity.Property(e => e.FormaPagamentoReserva).HasColumnName("forma_pagamento_reserva").HasMaxLength(50);
            entity.Property(e => e.Vendedor).HasColumnName("vendedor").HasMaxLength(100);
            entity.Property(e => e.Observacoes).HasColumnName("observacoes").HasMaxLength(500);
            entity.Property(e => e.DataConversaoVenda).HasColumnName("data_conversao_venda").HasColumnType("date");
            entity.Property(e => e.VendaId).HasColumnName("venda_id");
            
            entity.HasOne(d => d.Cliente)
                .WithMany(p => p.Reservas)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reservas_clientes");

            entity.HasOne(d => d.Apartamento)
                .WithMany(p => p.Reservas)
                .HasForeignKey(d => d.ApartamentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reservas_apartamentos");

            entity.HasOne(d => d.Venda)
                .WithMany()
                .HasForeignKey(d => d.VendaId)
                .HasConstraintName("FK_reservas_vendas");
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue("User");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}
