// Common types used across the application

export interface BaseEntity {
  id: number;
  dataCadastro?: string;
  dataAtualizacao?: string;
}

// API Response types
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}

// Authentication types
export interface LoginCredentials {
  email: string;
  password: string;
}

export interface AuthUser {
  id: number;
  nome: string;
  email: string;
  role?: string;
}

export interface AuthResponse {
  token: string;
  user: AuthUser;
  expiresAt: string;
}

// Filter and Search types
export interface SearchFilters {
  searchTerm?: string;
  status?: string;
  dateRange?: {
    start: Date;
    end: Date;
  };
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

// Form validation types
export interface ValidationError {
  field: string;
  message: string;
}

// Modal types
export interface ConfirmationModalProps {
  isOpen: boolean;
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  onConfirm: () => void;
  onCancel: () => void;
}

// Status types
export type ClienteStatus = 'Ativo' | 'Inativo' | 'Prospecto';
export type ApartamentoStatus = 'Disponível' | 'Reservado' | 'Vendido' | 'Indisponível';
export type VendaStatus = 'Pendente' | 'Confirmada' | 'Cancelada' | 'Em Análise';

// Navigation state types
export interface NavigationState {
  apartamentoId?: number;
  apartamentoInfo?: string;
  clienteId?: number;
  vendaId?: number;
  apartamentoFilter?: number;
  returnUrl?: string;
}
