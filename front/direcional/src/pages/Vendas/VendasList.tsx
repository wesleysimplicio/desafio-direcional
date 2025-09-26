import React, { useEffect, useState, useCallback } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { toast } from 'react-toastify';
import { 
  PlusIcon, 
  MagnifyingGlassIcon,
  PencilIcon,
  TrashIcon,
  EyeIcon,
  CurrencyDollarIcon,
  CalendarDaysIcon,
  UserIcon,
  BuildingOfficeIcon,
  CheckCircleIcon
} from '@heroicons/react/24/outline';
import { vendaService, Venda } from '../services/vendaService';
import { clienteService, Cliente } from '../services/clienteService';
import { apartamentoService, Apartamento } from '../services/apartamentoService';

const VendasList: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [vendas, setVendas] = useState<Venda[]>([]);
  const [filteredVendas, setFilteredVendas] = useState<Venda[]>([]);
  const [clientes, setClientes] = useState<{ [key: number]: Cliente }>({});
  const [apartamentos, setApartamentos] = useState<{ [key: number]: Apartamento }>({});
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [dateFilter, setDateFilter] = useState<string>('all');
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [vendaToDelete, setVendaToDelete] = useState<Venda | null>(null);

  // Check if there's a filter from navigation state
  const apartamentoFilter = location.state?.apartamentoFilter;

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    filterVendas();
  }, [vendas, searchTerm, statusFilter, dateFilter]);

  const loadData = async () => {
    try {
      setIsLoading(true);
      
      // Load vendas
      const vendasData = await vendaService.getAll();
      setVendas(vendasData);

      // Load related clientes and apartamentos
      const clienteIds = Array.from(new Set(vendasData.map(v => v.clienteId)));
      const apartamentoIds = Array.from(new Set(vendasData.map(v => v.apartamentoId)));

      const [clientesData, apartamentosData] = await Promise.all([
        Promise.all(clienteIds.map(id => clienteService.getById(id))),
        Promise.all(apartamentoIds.map(id => apartamentoService.getById(id)))
      ]);

      const clientesMap = clientesData.reduce((acc, cliente) => {
        acc[cliente.id] = cliente;
        return acc;
      }, {} as { [key: number]: Cliente });

      const apartamentosMap = apartamentosData.reduce((acc, apartamento) => {
        acc[apartamento.id] = apartamento;
        return acc;
      }, {} as { [key: number]: Apartamento });

      setClientes(clientesMap);
      setApartamentos(apartamentosMap);

    } catch (error) {
      toast.error('Erro ao carregar vendas');
    } finally {
      setIsLoading(false);
    }
  };

  const filterVendas = useCallback(() => {
    let filtered = vendas;

    // Apply apartamento filter from navigation if exists
    if (apartamentoFilter) {
      filtered = filtered.filter(venda => venda.apartamentoId === apartamentoFilter);
    }

    // Filter by search term
    if (searchTerm) {
      filtered = filtered.filter(venda => {
        const cliente = clientes[venda.clienteId];
        const apartamento = apartamentos[venda.apartamentoId];
        
        return (
          cliente?.nome.toLowerCase().includes(searchTerm.toLowerCase()) ||
          cliente?.cpf.includes(searchTerm) ||
          apartamento?.numero.toLowerCase().includes(searchTerm.toLowerCase()) ||
          apartamento?.bloco.toLowerCase().includes(searchTerm.toLowerCase()) ||
          venda.id.toString().includes(searchTerm)
        );
      });
    }

    // Filter by status
    if (statusFilter !== 'all') {
      filtered = filtered.filter(venda => venda.statusVenda === statusFilter);
    }

    // Filter by date period
    if (dateFilter !== 'all') {
      const now = new Date();
      const filterDate = new Date();
      
      switch (dateFilter) {
        case 'today':
          filterDate.setHours(0, 0, 0, 0);
          break;
        case 'week':
          filterDate.setDate(now.getDate() - 7);
          break;
        case 'month':
          filterDate.setMonth(now.getMonth() - 1);
          break;
        case 'year':
          filterDate.setFullYear(now.getFullYear() - 1);
          break;
      }

      if (dateFilter !== 'all') {
        filtered = filtered.filter(venda => 
          new Date(venda.dataVenda) >= filterDate
        );
      }
    }

    setFilteredVendas(filtered);
  }, [vendas, searchTerm, statusFilter, dateFilter, clientes, apartamentos, apartamentoFilter]);

  const handleDelete = async () => {
    if (!vendaToDelete) return;

    try {
      await vendaService.delete(vendaToDelete.id);
      toast.success('Venda excluída com sucesso!');
      loadData();
    } catch (error: any) {
      const message = error.response?.data?.message || 'Erro ao excluir venda';
      toast.error(message);
    } finally {
      setDeleteModalOpen(false);
      setVendaToDelete(null);
    }
  };

  const formatCurrency = (value?: number) => {
    if (!value) return 'R$ 0,00';
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('pt-BR');
  };

  const getStatusBadge = (status: string) => {
    const badges = {
      'Pendente': 'bg-yellow-100 text-yellow-800',
      'Confirmada': 'bg-green-100 text-green-800',
      'Cancelada': 'bg-red-100 text-red-800',
      'Em Análise': 'bg-blue-100 text-blue-800'
    };
    
    return badges[status as keyof typeof badges] || 'bg-gray-100 text-gray-800';
  };

  const calculateTotalValue = () => {
    return filteredVendas.reduce((sum, venda) => sum + (venda.valorTotal || 0), 0);
  };

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
          <h1 className="text-2xl font-bold text-gray-900">Vendas</h1>
          <p className="text-gray-600">Gerencie as vendas de apartamentos</p>
        </div>
        <button
          onClick={() => navigate('/vendas/nova')}
          className="btn btn-primary mt-4 sm:mt-0"
        >
          <PlusIcon className="w-5 h-5 mr-2" />
          Nova Venda
        </button>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="card p-4">
          <div className="flex items-center">
            <CurrencyDollarIcon className="w-8 h-8 text-blue-500" />
            <div className="ml-3">
              <p className="text-sm font-medium text-gray-600">Total Vendas</p>
              <p className="text-2xl font-bold text-gray-900">{vendas.length}</p>
            </div>
          </div>
        </div>
        
        <div className="card p-4">
          <div className="flex items-center">
            <CheckCircleIcon className="w-8 h-8 text-green-500" />
            <div className="ml-3">
              <p className="text-sm font-medium text-gray-600">Confirmadas</p>
              <p className="text-2xl font-bold text-gray-900">
                {vendas.filter(venda => venda.statusVenda === 'Confirmada').length}
              </p>
            </div>
          </div>
        </div>
        
        <div className="card p-4">
          <div className="flex items-center">
            <CalendarDaysIcon className="w-8 h-8 text-yellow-500" />
            <div className="ml-3">
              <p className="text-sm font-medium text-gray-600">Pendentes</p>
              <p className="text-2xl font-bold text-gray-900">
                {vendas.filter(venda => venda.statusVenda === 'Pendente').length}
              </p>
            </div>
          </div>
        </div>
        
        <div className="card p-4">
          <div className="flex items-center">
            <CurrencyDollarIcon className="w-8 h-8 text-purple-500" />
            <div className="ml-3">
              <p className="text-sm font-medium text-gray-600">Valor Total</p>
              <p className="text-lg font-bold text-gray-900">
                {formatCurrency(calculateTotalValue())}
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
                placeholder="Buscar por cliente, apartamento ou ID da venda..."
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
              <option value="Pendente">Pendente</option>
              <option value="Confirmada">Confirmada</option>
              <option value="Em Análise">Em Análise</option>
              <option value="Cancelada">Cancelada</option>
            </select>
          </div>

          <div className="sm:w-48">
            <select
              className="form-input"
              value={dateFilter}
              onChange={(e) => setDateFilter(e.target.value)}
            >
              <option value="all">Todos os Períodos</option>
              <option value="today">Hoje</option>
              <option value="week">Última Semana</option>
              <option value="month">Último Mês</option>
              <option value="year">Último Ano</option>
            </select>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="card overflow-hidden">
        {filteredVendas.length === 0 ? (
          <div className="p-8 text-center">
            <CurrencyDollarIcon className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">Nenhuma venda encontrada</h3>
            <p className="mt-1 text-sm text-gray-500">
              {vendas.length === 0 
                ? 'Comece registrando uma nova venda.'
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
                    Venda
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Cliente
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Apartamento
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Valor
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
                {filteredVendas.map((venda) => {
                  const cliente = clientes[venda.clienteId];
                  const apartamento = apartamentos[venda.apartamentoId];
                  
                  return (
                    <tr key={venda.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div>
                          <div className="text-sm font-medium text-gray-900">
                            Venda #{venda.id}
                          </div>
                          <div className="text-sm text-gray-500">
                            {formatDate(venda.dataVenda)}
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <UserIcon className="w-5 h-5 text-gray-400 mr-2" />
                          <div>
                            <div className="text-sm font-medium text-gray-900">
                              {cliente?.nome || 'Cliente não encontrado'}
                            </div>
                            <div className="text-sm text-gray-500">
                              {cliente?.cpf || '-'}
                            </div>
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <BuildingOfficeIcon className="w-5 h-5 text-gray-400 mr-2" />
                          <div>
                            <div className="text-sm font-medium text-gray-900">
                              {apartamento ? `${apartamento.bloco} - Apt ${apartamento.numero}` : 'Apartamento não encontrado'}
                            </div>
                            <div className="text-sm text-gray-500">
                              {apartamento?.tipoApartamento || '-'}
                            </div>
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                        {formatCurrency(venda.valorTotal)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusBadge(venda.statusVenda)}`}>
                          {venda.statusVenda}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                        <div className="flex justify-end space-x-2">
                          <button
                            onClick={() => navigate(`/vendas/${venda.id}/detalhes`)}
                            className="text-blue-600 hover:text-blue-900"
                            title="Ver detalhes"
                          >
                            <EyeIcon className="w-4 h-4" />
                          </button>
                          <button
                            onClick={() => navigate(`/vendas/${venda.id}/editar`)}
                            className="text-indigo-600 hover:text-indigo-900"
                            title="Editar"
                          >
                            <PencilIcon className="w-4 h-4" />
                          </button>
                          <button
                            onClick={() => {
                              setVendaToDelete(venda);
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
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Delete Modal */}
      {deleteModalOpen && vendaToDelete && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
          <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
            <div className="mt-3 text-center">
              <TrashIcon className="mx-auto h-16 w-16 text-red-600" />
              <h3 className="text-lg font-medium text-gray-900 mt-4">Excluir Venda</h3>
              <div className="mt-2 px-7 py-3">
                <p className="text-sm text-gray-500">
                  Tem certeza que deseja excluir a venda <strong>#{vendaToDelete.id}</strong>?
                  Esta ação não pode ser desfeita.
                </p>
              </div>
              <div className="flex justify-center space-x-4 mt-4">
                <button
                  onClick={() => {
                    setDeleteModalOpen(false);
                    setVendaToDelete(null);
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

export default VendasList;
