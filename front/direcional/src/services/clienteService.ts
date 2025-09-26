import api from './api';

export interface Cliente {
  clienteId: number;
  nome: string;
  cpf: string;
  email?: string;
  telefone?: string;
  endereco?: string;
  cidade?: string;
  estado?: string;
  cep?: string;
  dataNascimento?: string;
  rendaMensal?: number;
  statusCliente: string;
  dataCadastro: string;
  observacoes?: string;
}

export interface CreateClienteData {
  nome: string;
  cpf: string;
  email?: string;
  telefone?: string;
  endereco?: string;
  cidade?: string;
  estado?: string;
  cep?: string;
  dataNascimento?: string;
  rendaMensal?: number;
  observacoes?: string;
}

export interface UpdateClienteData {
  nome: string;
  email?: string;
  telefone?: string;
  endereco?: string;
  cidade?: string;
  estado?: string;
  cep?: string;
  dataNascimento?: string;
  rendaMensal?: number;
  statusCliente: string;
  observacoes?: string;
}

export const clienteService = {
  async getAll(): Promise<Cliente[]> {
    const response = await api.get('/clientes');
    return response.data;
  },

  async getById(id: number): Promise<Cliente> {
    const response = await api.get(`/clientes/${id}`);
    return response.data;
  },

  async create(data: CreateClienteData): Promise<Cliente> {
    const response = await api.post('/clientes', data);
    return response.data;
  },

  async update(id: number, data: UpdateClienteData): Promise<Cliente> {
    const response = await api.put(`/clientes/${id}`, data);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/clientes/${id}`);
  }
};
