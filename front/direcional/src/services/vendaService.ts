import api from './api';
import { Cliente } from './clienteService';
import { Apartamento } from './apartamentoService';

export interface Venda {
  vendaId: number;
  clienteId: number;
  apartamentoId: number;
  dataVenda: string;
  valorVenda: number;
  formaPagamento?: string;
  valorEntrada?: number;
  numeroParcelas?: number;
  valorParcela?: number;
  dataPrimeiraParcela?: string;
  statusVenda: string;
  vendedor?: string;
  comissaoVendedor?: number;
  observacoes?: string;
  dataQuitacao?: string;
  cliente?: Cliente;
  apartamento?: Apartamento;
}

export interface CreateVendaData {
  clienteId: number;
  apartamentoId: number;
  valorVenda: number;
  formaPagamento?: string;
  valorEntrada?: number;
  numeroParcelas?: number;
  valorParcela?: number;
  dataPrimeiraParcela?: string;
  vendedor?: string;
  comissaoVendedor?: number;
  observacoes?: string;
}

export interface UpdateVendaData {
  valorVenda: number;
  formaPagamento?: string;
  valorEntrada?: number;
  numeroParcelas?: number;
  valorParcela?: number;
  dataPrimeiraParcela?: string;
  statusVenda: string;
  vendedor?: string;
  comissaoVendedor?: number;
  observacoes?: string;
  dataQuitacao?: string;
}

export const vendaService = {
  async getAll(): Promise<Venda[]> {
    const response = await api.get('/vendas');
    return response.data;
  },

  async getById(id: number): Promise<Venda> {
    const response = await api.get(`/vendas/${id}`);
    return response.data;
  },

  async getByClienteId(clienteId: number): Promise<Venda[]> {
    const response = await api.get(`/vendas/cliente/${clienteId}`);
    return response.data;
  },

  async getByStatus(status: string): Promise<Venda[]> {
    const response = await api.get(`/vendas/status/${status}`);
    return response.data;
  },

  async create(data: CreateVendaData): Promise<Venda> {
    const response = await api.post('/vendas', data);
    return response.data;
  },

  async update(id: number, data: UpdateVendaData): Promise<Venda> {
    const response = await api.put(`/vendas/${id}`, data);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/vendas/${id}`);
  }
};
