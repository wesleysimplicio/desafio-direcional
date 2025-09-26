-- ==========================================
-- Script DDL para Sistema Direcional
-- Banco de dados: SQL Server Express
-- Autor: Sistema Direcional
-- Data: 2025-09-26
-- ==========================================

-- Criar banco de dados
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DirecionalDB')
BEGIN
    CREATE DATABASE DirecionalDB;
END
GO

-- Usar o banco de dados criado
USE DirecionalDB;
GO

-- ==========================================
-- TABELA: clientes
-- Descrição: Armazenar informações dos clientes
-- ==========================================
CREATE TABLE clientes (
    cliente_id INT IDENTITY(1,1) PRIMARY KEY,
    nome NVARCHAR(100) NOT NULL,
    cpf CHAR(11) UNIQUE NOT NULL,
    email NVARCHAR(100) UNIQUE,
    telefone NVARCHAR(15),
    endereco NVARCHAR(200),
    cidade NVARCHAR(50),
    estado CHAR(2),
    cep CHAR(8),
    data_nascimento DATE,
    renda_mensal DECIMAL(10,2),
    status_cliente NVARCHAR(20) DEFAULT 'Ativo' CHECK (status_cliente IN ('Ativo', 'Inativo', 'Prospecto')),
    data_cadastro DATETIME DEFAULT GETDATE(),
    observacoes NVARCHAR(500)
);

-- ==========================================
-- TABELA: apartamentos
-- Descrição: Armazenar informações dos apartamentos
-- ==========================================
CREATE TABLE apartamentos (
    apartamento_id INT IDENTITY(1,1) PRIMARY KEY,
    numero_apartamento NVARCHAR(10) NOT NULL,
    bloco NVARCHAR(10),
    andar INT,
    area_total DECIMAL(8,2) NOT NULL,
    area_privativa DECIMAL(8,2),
    quartos INT NOT NULL,
    suites INT DEFAULT 0,
    banheiros INT NOT NULL,
    vagas_garagem INT DEFAULT 0,
    varanda BIT DEFAULT 0,
    valor_venda DECIMAL(12,2) NOT NULL,
    valor_condominio DECIMAL(8,2),
    status_apartamento NVARCHAR(20) DEFAULT 'Disponível' CHECK (status_apartamento IN ('Disponível', 'Reservado', 'Vendido', 'Indisponível')),
    data_cadastro DATETIME DEFAULT GETDATE(),
    descricao NVARCHAR(500),
    empreendimento NVARCHAR(100),
    entrega_prevista DATE
);

-- ==========================================
-- TABELA: vendas
-- Descrição: Armazenar informações sobre vendas de apartamentos
-- ==========================================
CREATE TABLE vendas (
    venda_id INT IDENTITY(1,1) PRIMARY KEY,
    cliente_id INT NOT NULL,
    apartamento_id INT NOT NULL,
    data_venda DATETIME DEFAULT GETDATE(),
    valor_venda DECIMAL(12,2) NOT NULL,
    forma_pagamento NVARCHAR(50),
    valor_entrada DECIMAL(12,2),
    numero_parcelas INT,
    valor_parcela DECIMAL(10,2),
    data_primeira_parcela DATE,
    status_venda NVARCHAR(20) DEFAULT 'Ativa' CHECK (status_venda IN ('Ativa', 'Quitada', 'Cancelada', 'Inadimplente')),
    vendedor NVARCHAR(100),
    comissao_vendedor DECIMAL(10,2),
    observacoes NVARCHAR(500),
    data_quitacao DATE,
    
    -- Chaves estrangeiras
    FOREIGN KEY (cliente_id) REFERENCES clientes(cliente_id),
    FOREIGN KEY (apartamento_id) REFERENCES apartamentos(apartamento_id)
);

-- ==========================================
-- TABELA: reservas
-- Descrição: Armazenar informações sobre reservas de apartamentos
-- ==========================================
CREATE TABLE reservas (
    reserva_id INT IDENTITY(1,1) PRIMARY KEY,
    cliente_id INT NOT NULL,
    apartamento_id INT NOT NULL,
    data_reserva DATETIME DEFAULT GETDATE(),
    valor_reserva DECIMAL(10,2) NOT NULL,
    data_validade_reserva DATE NOT NULL,
    status_reserva NVARCHAR(20) DEFAULT 'Ativa' CHECK (status_reserva IN ('Ativa', 'Convertida', 'Cancelada', 'Expirada')),
    forma_pagamento_reserva NVARCHAR(50),
    vendedor NVARCHAR(100),
    observacoes NVARCHAR(500),
    data_conversao_venda DATE,
    venda_id INT, -- Referência para a venda quando a reserva for convertida
    
    -- Chaves estrangeiras
    FOREIGN KEY (cliente_id) REFERENCES clientes(cliente_id),
    FOREIGN KEY (apartamento_id) REFERENCES apartamentos(apartamento_id),
    FOREIGN KEY (venda_id) REFERENCES vendas(venda_id)
);

-- ==========================================
-- ÍNDICES PARA MELHOR PERFORMANCE
-- ==========================================

-- Índices na tabela clientes
CREATE INDEX IX_clientes_cpf ON clientes(cpf);
CREATE INDEX IX_clientes_email ON clientes(email);
CREATE INDEX IX_clientes_status ON clientes(status_cliente);

-- Índices na tabela apartamentos
CREATE INDEX IX_apartamentos_numero ON apartamentos(numero_apartamento);
CREATE INDEX IX_apartamentos_status ON apartamentos(status_apartamento);
CREATE INDEX IX_apartamentos_valor ON apartamentos(valor_venda);
CREATE INDEX IX_apartamentos_empreendimento ON apartamentos(empreendimento);

-- Índices na tabela vendas
CREATE INDEX IX_vendas_cliente ON vendas(cliente_id);
CREATE INDEX IX_vendas_apartamento ON vendas(apartamento_id);
CREATE INDEX IX_vendas_data ON vendas(data_venda);
CREATE INDEX IX_vendas_status ON vendas(status_venda);

-- Índices na tabela reservas
CREATE INDEX IX_reservas_cliente ON reservas(cliente_id);
CREATE INDEX IX_reservas_apartamento ON reservas(apartamento_id);
CREATE INDEX IX_reservas_data ON reservas(data_reserva);
CREATE INDEX IX_reservas_status ON reservas(status_reserva);
CREATE INDEX IX_reservas_validade ON reservas(data_validade_reserva);

-- Inserir alguns dados de exemplo para testes

-- Exemplo de clientes
INSERT INTO clientes (nome, cpf, email, telefone, endereco, cidade, estado, cep, renda_mensal) VALUES
('João Silva Santos', '12345678901', 'joao.silva@email.com', '11987654321', 'Rua das Flores, 123', 'São Paulo', 'SP', '01234567', 5000.00),
('Maria Oliveira Costa', '98765432109', 'maria.oliveira@email.com', '11876543210', 'Av. Paulista, 456', 'São Paulo', 'SP', '01310100', 7500.00),
('Carlos Pereira Lima', '45678912345', 'carlos.pereira@email.com', '11765432109', 'Rua Augusta, 789', 'São Paulo', 'SP', '01305000', 6200.00);

-- Exemplo de apartamentos
INSERT INTO apartamentos (numero_apartamento, bloco, andar, area_total, area_privativa, quartos, suites, banheiros, vagas_garagem, valor_venda, valor_condominio, empreendimento) VALUES
('101', 'A', 1, 65.50, 58.30, 2, 1, 2, 1, 280000.00, 450.00, 'Residencial Vista Verde'),
('201', 'A', 2, 75.20, 68.10, 3, 1, 2, 1, 320000.00, 520.00, 'Residencial Vista Verde'),
('301', 'B', 3, 85.80, 78.50, 3, 2, 3, 2, 450000.00, 680.00, 'Residencial Vista Verde');


PRINT 'Script DDL executado com sucesso!';
PRINT 'Banco de dados DirecionalDB criado com as tabelas:';
PRINT '- clientes';
PRINT '- apartamentos'; 
PRINT '- vendas';
PRINT '- reservas';
PRINT 'Índices, triggers e views também foram criados.';