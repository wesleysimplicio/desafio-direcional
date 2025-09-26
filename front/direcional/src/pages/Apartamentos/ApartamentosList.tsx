import React, { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { 
  PlusIcon, 
  MagnifyingGlassIcon,
  PencilIcon,
  TrashIcon,
  EyeIcon,
  BuildingOfficeIcon,
  MapPinIcon,
  CurrencyDollarIcon,
  CheckIcon
} from '@heroicons/react/24/outline';
import { apartamentoService, Apartamento } from '../services/apartamentoService';

const ApartamentosList: React.FC = () => {
  const navigate = useNavigate();
  const [apartamentos, setApartamentos] = useState<Apartamento[]>([]);
  const [filteredApartamentos, setFilteredApartamentos] = useState<Apartamento[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [tipoFilter, setTipoFilter] = useState<string>('all');
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [apartamentoToDelete, setApartamentoToDelete] = useState<Apartamento | null>(null);

  useEffect(() => {
    loadApartamentos();
  }, []);

  useEffect(() => {
    filterApartamentos();
  }, [apartamentos, searchTerm, statusFilter, tipoFilter]);

  const loadApartamentos = async () => {
    try {
      setIsLoading(true);
      const data = await apartamentoService.getAll();
      setApartamentos(data);
    } catch (error) {
      toast.error('Erro ao carregar apartamentos');
    } finally {
      setIsLoading(false);
    }
  };

  const filterApartamentos = useCallback(() => {
    let filtered = apartamentos;

    if (searchTerm) {
      filtered = filtered.filter(apt => 
        apt.numero.toLowerCase().includes(searchTerm.toLowerCase()) ||
        apt.bloco.toLowerCase().includes(searchTerm.toLowerCase()) ||
        apt.tipoApartamento.toLowerCase().includes(searchTerm.toLowerCase()) ||
        (apt.endereco && apt.endereco.toLowerCase().includes(searchTerm.toLowerCase()))
      );
    }

    if (statusFilter !== 'all') {
      filtered = filtered.filter(apt => apt.statusApartamento === statusFilter);
    }

    if (tipoFilter !== 'all') {
      filtered = filtered.filter(apt => apt.tipoApartamento === tipoFilter);
    }

    setFilteredApartamentos(filtered);
  }, [apartamentos, searchTerm, statusFilter, tipoFilter]);

  const handleDelete = async () => {
    if (!apartamentoToDelete) return;

    try {
      await apartamentoService.delete(apartamentoToDelete.id);
      toast.success('Apartamento excluído com sucesso!');
      loadApartamentos();
    } catch (error: any) {
      const message = error.response?.data?.message || 'Erro ao excluir apartamento';
      toast.error(message);
    } finally {
      setDeleteModalOpen(false);
      setApartamentoToDelete(null);
    }
  };

  const formatCurrency = (value?: number) => {
    if (!value) return 'R$ 0,00';
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  };

  const getStatusBadge = (status: string) => {
    const badges = {
      'Disponível': 'bg-green-100 text-green-800',
      'Reservado': 'bg-yellow-100 text-yellow-800',
      'Vendido': 'bg-blue-100 text-blue-800',
      'Indisponível': 'bg-red-100 text-red-800'
    };
    
    return badges[status as keyof typeof badges] || 'bg-gray-100 text-gray-800';
  };

  const getTipoIcon = (tipo: string) => {
    if (tipo.includes('Casa')) return '🏠';
    if (tipo.includes('Cobertura')) return '🏢';
    return '🏠';
  };

  const uniqueTypes = Array.from(new Set(apartamentos.map(apt => apt.tipoApartamento)));

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Apartamentos</h1>
          <p className="text-gray-600">Gerencie o catálogo de imóveis</p>
        </div>
        <button
          onClick={() => navigate('/apartamentos/novo')}
          className="btn btn-primary mt-4 sm:mt-0"
        >
          <PlusIcon className="w-5 h-5 mr-2" />
          Novo Apartamento
        </button>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="card p-4">
          <div className="flex items-center">
            <BuildingOfficeIcon className="w-8 h-8 text-blue-500" />
            <div className="ml-3">
              <p className="text-sm font-medium text-gray-600">Total</p>
              <p className="text-2xl font-bold text-gray-900">{apartamentos.length}</p>
            </div>
          </div>
        </div>
        
        <div className="card p-4">
          <div className="flex items-center">
            <CheckIcon className="w-8 h-8 text-green-500" />
            <div className="ml-3">
              <p className="text-sm font-medium text-gray-600">Disponíveis</p>
              <p className="text-2xl font-bold text-gray-900">
                {apartamentos.filter(apt => apt.statusApartamento === 'Disponível').length}
              </p>
            </div>
          </div>
        </div>
        
        <div className="card p-4">
          <div className="flex items-center">
            <MapPinIcon className="w-8 h-8 text-yellow-500" />
            <div className="ml-3">
              <p className="text-sm font-medium text-gray-600">Reservados</p>
              <p className="text-2xl font-bold text-gray-900">
                {apartamentos.filter(apt => apt.statusApartamento === 'Reservado').length}
              </p>
            </div>
          </div>
        </div>
        
        <div className="card p-4">
          <div className="flex items-center">
            <CurrencyDollarIcon className="w-8 h-8 text-blue-500" />
            <div className="ml-3">
              <p className="text-sm font-medium text-gray-600">Vendidos</p>
              <p className="text-2xl font-bold text-gray-900">
                {apartamentos.filter(apt => apt.statusApartamento === 'Vendido').length}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Filters */}
      <div className="card p-4">
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="flex-1">
            <div className="relative">
              <MagnifyingGlassIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
              <input
                type="text"
                placeholder="Buscar por número, bloco, tipo ou endereço..."
                className="form-input pl-10"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
          </div>
          
          <div className="sm:w-48">
            <select
              className="form-input"
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
            >
              <option value="all">Todos os Status</option>
              <option value="Disponível">Disponível</option>
              <option value="Reservado">Reservado</option>
              <option value="Vendido">Vendido</option>
              <option value="Indisponível">Indisponível</option>
            </select>
          </div>

          <div className="sm:w-48">
            <select
              className="form-input"
              value={tipoFilter}
              onChange={(e) => setTipoFilter(e.target.value)}
            >
              <option value="all">Todos os Tipos</option>
              {uniqueTypes.map(tipo => (
                <option key={tipo} value={tipo}>{tipo}</option>
              ))}
            </select>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="card overflow-hidden">
        {filteredApartamentos.length === 0 ? (
          <div className="p-8 text-center">
            <BuildingOfficeIcon className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">Nenhum apartamento encontrado</h3>
            <p className="mt-1 text-sm text-gray-500">
              {apartamentos.length === 0 
                ? 'Comece criando um novo apartamento.'
                : 'Tente ajustar os filtros de busca.'
              }
            </p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Apartamento
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Tipo
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Área
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Preço
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Ações
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredApartamentos.map((apartamento) => (
                  <tr key={apartamento.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <div className="text-2xl mr-3">{getTipoIcon(apartamento.tipoApartamento)}</div>
                        <div>
                          <div className="text-sm font-medium text-gray-900">
                            {apartamento.bloco} - Apt {apartamento.numero}
                          </div>
                          <div className="text-sm text-gray-500">
                            {apartamento.quartos}Q • {apartamento.banheiros}B
                            {apartamento.vagasGaragem ? ` • ${apartamento.vagasGaragem}G` : ''}
                          </div>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {apartamento.tipoApartamento}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {apartamento.areaTotal ? `${apartamento.areaTotal}m²` : '-'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      {formatCurrency(apartamento.preco)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusBadge(apartamento.statusApartamento)}`}>
                        {apartamento.statusApartamento}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex justify-end space-x-2">
                        <button
                          onClick={() => navigate(`/apartamentos/${apartamento.id}/detalhes`)}
                          className="text-blue-600 hover:text-blue-900"
                          title="Ver detalhes"
                        >
                          <EyeIcon className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => navigate(`/apartamentos/${apartamento.id}/editar`)}
                          className="text-indigo-600 hover:text-indigo-900"
                          title="Editar"
                        >
                          <PencilIcon className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => {
                            setApartamentoToDelete(apartamento);
                            setDeleteModalOpen(true);
                          }}
                          className="text-red-600 hover:text-red-900"
                          title="Excluir"
                        >
                          <TrashIcon className="w-4 h-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Delete Modal */}
      {deleteModalOpen && apartamentoToDelete && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
          <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
            <div className="mt-3 text-center">
              <TrashIcon className="mx-auto h-16 w-16 text-red-600" />
              <h3 className="text-lg font-medium text-gray-900 mt-4">Excluir Apartamento</h3>
              <div className="mt-2 px-7 py-3">
                <p className="text-sm text-gray-500">
                  Tem certeza que deseja excluir o apartamento <strong>{apartamentoToDelete.bloco} - Apt {apartamentoToDelete.numero}</strong>?
                  Esta ação não pode ser desfeita.
                </p>
              </div>
              <div className="flex justify-center space-x-4 mt-4">
                <button
                  onClick={() => {
                    setDeleteModalOpen(false);
                    setApartamentoToDelete(null);
                  }}
                  className="btn btn-secondary"
                >
                  Cancelar
                </button>
                <button
                  onClick={handleDelete}
                  className="btn bg-red-600 text-white hover:bg-red-700"
                >
                  Excluir
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ApartamentosList;
