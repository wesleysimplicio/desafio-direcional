import api from './api';

export interface Apartamento {
  apartamentoId: number;
  numeroApartamento: string;
  bloco?: string;
  andar?: number;
  areaTotal: number;
  areaPrivativa?: number;
  quartos: number;
  suites: number;
  banheiros: number;
  vagasGaragem: number;
  varanda: boolean;
  valorVenda: number;
  valorCondominio?: number;
  statusApartamento: string;
  dataCadastro: string;
  descricao?: string;
  empreendimento?: string;
  entregaPrevista?: string;
}

export interface CreateApartamentoData {
  numeroApartamento: string;
  bloco?: string;
  andar?: number;
  areaTotal: number;
  areaPrivativa?: number;
  quartos: number;
  suites: number;
  banheiros: number;
  vagasGaragem: number;
  varanda: boolean;
  valorVenda: number;
  valorCondominio?: number;
  descricao?: string;
  empreendimento?: string;
  entregaPrevista?: string;
}

export interface UpdateApartamentoData {
  numeroApartamento: string;
  bloco?: string;
  andar?: number;
  areaTotal: number;
  areaPrivativa?: number;
  quartos: number;
  suites: number;
  banheiros: number;
  vagasGaragem: number;
  varanda: boolean;
  valorVenda: number;
  valorCondominio?: number;
  statusApartamento: string;
  descricao?: string;
  empreendimento?: string;
  entregaPrevista?: string;
}

export const apartamentoService = {
  async getAll(): Promise<Apartamento[]> {
    const response = await api.get('/apartamentos');
    return response.data;
  },

  async getById(id: number): Promise<Apartamento> {
    const response = await api.get(`/apartamentos/${id}`);
    return response.data;
  },

  async getByStatus(status: string): Promise<Apartamento[]> {
    const response = await api.get(`/apartamentos/status/${status}`);
    return response.data;
  },

  async create(data: CreateApartamentoData): Promise<Apartamento> {
    const response = await api.post('/apartamentos', data);
    return response.data;
  },

  async update(id: number, data: UpdateApartamentoData): Promise<Apartamento> {
    const response = await api.put(`/apartamentos/${id}`, data);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/apartamentos/${id}`);
  }
};
