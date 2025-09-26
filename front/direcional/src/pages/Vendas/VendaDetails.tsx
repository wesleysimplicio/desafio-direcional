import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { 
  ArrowLeftIcon,
  PencilIcon,
  CurrencyDollarIcon,
  CalendarDaysIcon,
  UserIcon,
  BuildingOfficeIcon,
  DocumentTextIcon,
  CheckCircleIcon
} from '@heroicons/react/24/outline';
import { vendaService, Venda } from '../services/vendaService';
import { clienteService, Cliente } from '../services/clienteService';
import { apartamentoService, Apartamento } from '../services/apartamentoService';

const VendaDetails: React.FC = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [venda, setVenda] = useState<Venda | null>(null);
  const [cliente, setCliente] = useState<Cliente | null>(null);
  const [apartamento, setApartamento] = useState<Apartamento | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    if (id) {
      loadVenda(parseInt(id));
    }
  }, [id]);

  const loadVenda = async (vendaId: number) => {
    try {
      setIsLoading(true);
      const vendaData = await vendaService.getById(vendaId);
      setVenda(vendaData);

      const [clienteData, apartamentoData] = await Promise.all([
        clienteService.getById(vendaData.clienteId),
        apartamentoService.getById(vendaData.apartamentoId)
      ]);

      setCliente(clienteData);
      setApartamento(apartamentoData);
    } catch (error) {
      toast.error('Erro ao carregar dados da venda');
      navigate('/vendas');
    } finally {
      setIsLoading(false);
    }
  };

  const formatCurrency = (value?: number) => {
    if (!value) return 'R$ 0,00';
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return 'Não informado';
    return new Date(dateString).toLocaleDateString('pt-BR');
  };

  const getStatusBadge = (status: string) => {
    const badges = {
      'Pendente': 'bg-yellow-100 text-yellow-800 border-yellow-200',
      'Confirmada': 'bg-green-100 text-green-800 border-green-200',
      'Cancelada': 'bg-red-100 text-red-800 border-red-200',
      'Em Análise': 'bg-blue-100 text-blue-800 border-blue-200'
    };
    
    return badges[status as keyof typeof badges] || 'bg-gray-100 text-gray-800 border-gray-200';
  };

  const getTipoIcon = (tipo?: string) => {
    if (!tipo) return '🏠';
    if (tipo.includes('Casa')) return '🏠';
    if (tipo.includes('Cobertura')) return '🏢';
    return '🏠';
  };

  const calculateFinancingInfo = () => {
    if (!venda) return null;

    const valorTotal = venda.valorTotal || 0;
    const valorEntrada = venda.valorEntrada || 0;
    const valorFinanciado = valorTotal - valorEntrada;
    const porcentagemEntrada = valorTotal > 0 ? (valorEntrada / valorTotal) * 100 : 0;
    const porcentagemFinanciada = 100 - porcentagemEntrada;

    return {
      valorFinanciado,
      porcentagemEntrada,
      porcentagemFinanciada
    };
  };

  const financingInfo = calculateFinancingInfo();

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (!venda || !cliente || !apartamento) {
    return (
      <div className="text-center py-12">
        <CurrencyDollarIcon className="mx-auto h-12 w-12 text-gray-400" />
        <h3 className="mt-2 text-sm font-medium text-gray-900">Venda não encontrada</h3>
        <p className="mt-1 text-sm text-gray-500">A venda que você está procurando não existe.</p>
        <div className="mt-6">
          <button
            onClick={() => navigate('/vendas')}
            className="btn btn-primary"
          >
            Voltar para Lista
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-4">
          <button
            onClick={() => navigate('/vendas')}
            className="p-2 text-gray-400 hover:text-gray-600 rounded-md"
          >
            <ArrowLeftIcon className="h-6 w-6" />
          </button>
          <div className="flex items-center space-x-3">
            <CurrencyDollarIcon className="w-8 h-8 text-green-500" />
            <div>
              <h1 className="text-2xl font-bold text-gray-900">
                Venda #{venda.id}
              </h1>
              <p className="text-gray-600">{formatDate(venda.dataVenda)}</p>
            </div>
          </div>
        </div>
        
        <div className="flex space-x-3">
          <button
            onClick={() => navigate(`/vendas/${venda.id}/editar`)}
            className="btn btn-secondary"
          >
            <PencilIcon className="w-5 h-5 mr-2" />
            Editar
          </button>
        </div>
      </div>

      {/* Status Badge */}
      <div>
        <span className={`inline-flex px-3 py-1 text-sm font-semibold rounded-full border ${getStatusBadge(venda.statusVenda)}`}>
          {venda.statusVenda}
        </span>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Main Info */}
        <div className="lg:col-span-2 space-y-6">
          {/* Cliente Info */}
          <div className="card p-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
              <UserIcon className="w-5 h-5 mr-2" />
              Informações do Cliente
            </h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-3">
                <div>
                  <span className="text-sm font-medium text-gray-600">Nome:</span>
                  <p className="text-gray-900 font-semibold">{cliente.nome}</p>
                </div>
                
                <div>
                  <span className="text-sm font-medium text-gray-600">CPF:</span>
                  <p className="text-gray-900">{cliente.cpf}</p>
                </div>
                
                {cliente.email && (
                  <div>
                    <span className="text-sm font-medium text-gray-600">Email:</span>
                    <p className="text-gray-900">{cliente.email}</p>
                  </div>
                )}
              </div>
              
              <div className="space-y-3">
                {cliente.telefone && (
                  <div>
                    <span className="text-sm font-medium text-gray-600">Telefone:</span>
                    <p className="text-gray-900">{cliente.telefone}</p>
                  </div>
                )}
                
                {cliente.rendaMensal && (
                  <div>
                    <span className="text-sm font-medium text-gray-600">Renda Mensal:</span>
                    <p className="text-gray-900">{formatCurrency(cliente.rendaMensal)}</p>
                  </div>
                )}
                
                <div>
                  <span className="text-sm font-medium text-gray-600">Status:</span>
                  <p className="text-gray-900">{cliente.statusCliente}</p>
                </div>
              </div>
            </div>

            <div className="mt-4 pt-4 border-t">
              <button
                onClick={() => navigate(`/clientes/${cliente.id}/detalhes`)}
                className="btn btn-secondary btn-sm"
              >
                Ver Detalhes do Cliente
              </button>
            </div>
          </div>

          {/* Apartamento Info */}
          <div className="card p-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
              <BuildingOfficeIcon className="w-5 h-5 mr-2" />
              Informações do Apartamento
            </h3>
            
            <div className="flex items-center space-x-4 mb-4">
              <div className="text-3xl">{getTipoIcon(apartamento.tipoApartamento)}</div>
              <div>
                <h4 className="text-lg font-semibold text-gray-900">
                  {apartamento.bloco} - Apartamento {apartamento.numero}
                </h4>
                <p className="text-gray-600">{apartamento.tipoApartamento}</p>
              </div>
            </div>

            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div className="text-center p-3 bg-blue-50 rounded-lg">
                <div className="text-xl font-bold text-blue-600">{apartamento.quartos}</div>
                <div className="text-xs text-gray-600">Quartos</div>
              </div>
              
              <div className="text-center p-3 bg-green-50 rounded-lg">
                <div className="text-xl font-bold text-green-600">{apartamento.banheiros}</div>
                <div className="text-xs text-gray-600">Banheiros</div>
              </div>
              
              <div className="text-center p-3 bg-purple-50 rounded-lg">
                <div className="text-xl font-bold text-purple-600">{apartamento.vagasGaragem || 0}</div>
                <div className="text-xs text-gray-600">Garagem</div>
              </div>
              
              <div className="text-center p-3 bg-orange-50 rounded-lg">
                <div className="text-xl font-bold text-orange-600">
                  {apartamento.areaTotal ? `${apartamento.areaTotal}m²` : '-'}
                </div>
                <div className="text-xs text-gray-600">Área</div>
              </div>
            </div>

            <div className="mt-4 pt-4 border-t">
              <button
                onClick={() => navigate(`/apartamentos/${apartamento.id}/detalhes`)}
                className="btn btn-secondary btn-sm"
              >
                Ver Detalhes do Apartamento
              </button>
            </div>
          </div>

          {/* Observações */}
          {venda.observacoes && (
            <div className="card p-6">
              <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
                <DocumentTextIcon className="w-5 h-5 mr-2" />
                Observações
              </h3>
              <p className="text-gray-700 whitespace-pre-wrap">{venda.observacoes}</p>
            </div>
          )}
        </div>

        {/* Sidebar */}
        <div className="space-y-6">
          {/* Financial Summary */}
          <div className="card p-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
              <CurrencyDollarIcon className="w-5 h-5 mr-2" />
              Resumo Financeiro
            </h3>
            
            <div className="space-y-4">
              <div className="bg-gray-50 rounded-lg p-4">
                <div className="text-center">
                  <div className="text-2xl font-bold text-primary-600">
                    {formatCurrency(venda.valorTotal)}
                  </div>
                  <div className="text-sm text-gray-500">Valor Total</div>
                </div>
              </div>

              {venda.valorEntrada && (
                <div className="space-y-3">
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-gray-600">Entrada:</span>
                    <span className="font-semibold text-green-600">
                      {formatCurrency(venda.valorEntrada)}
                    </span>
                  </div>
                  {financingInfo && (
                    <div className="text-xs text-gray-500">
                      {financingInfo.porcentagemEntrada.toFixed(1)}% do valor total
                    </div>
                  )}
                </div>
              )}

              {venda.valorEntrada && financingInfo && (
                <div className="space-y-3">
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-gray-600">Valor Financiado:</span>
                    <span className="font-semibold text-blue-600">
                      {formatCurrency(financingInfo.valorFinanciado)}
                    </span>
                  </div>
                  <div className="text-xs text-gray-500">
                    {financingInfo.porcentagemFinanciada.toFixed(1)}% do valor total
                  </div>
                </div>
              )}

              {venda.numeroParcelasFinanciamento && venda.valorParcelasFinanciamento && (
                <div className="space-y-2 pt-3 border-t">
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-gray-600">Parcelas:</span>
                    <span className="font-semibold">{venda.numeroParcelasFinanciamento}x</span>
                  </div>
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-gray-600">Valor da Parcela:</span>
                    <span className="font-semibold text-orange-600">
                      {formatCurrency(venda.valorParcelasFinanciamento)}
                    </span>
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* Additional Info */}
          <div className="card p-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
              <CalendarDaysIcon className="w-5 h-5 mr-2" />
              Informações Adicionais
            </h3>
            
            <div className="space-y-3 text-sm">
              <div className="flex justify-between">
                <span className="text-gray-600">ID da Venda:</span>
                <span className="font-medium">#{venda.id}</span>
              </div>
              
              <div className="flex justify-between">
                <span className="text-gray-600">Data da Venda:</span>
                <span className="font-medium">{formatDate(venda.dataVenda)}</span>
              </div>
              
              <div className="flex justify-between">
                <span className="text-gray-600">Status:</span>
                <span className={`font-medium ${
                  venda.statusVenda === 'Confirmada' ? 'text-green-600' :
                  venda.statusVenda === 'Cancelada' ? 'text-red-600' :
                  venda.statusVenda === 'Em Análise' ? 'text-blue-600' :
                  'text-yellow-600'
                }`}>
                  {venda.statusVenda}
                </span>
              </div>
              
              {venda.dataCadastro && (
                <div className="flex justify-between">
                  <span className="text-gray-600">Cadastrada em:</span>
                  <span className="font-medium">{formatDate(venda.dataCadastro)}</span>
                </div>
              )}
              
              {venda.dataAtualizacao && (
                <div className="flex justify-between">
                  <span className="text-gray-600">Atualizada em:</span>
                  <span className="font-medium">{formatDate(venda.dataAtualizacao)}</span>
                </div>
              )}
            </div>
          </div>

          {/* Quick Actions */}
          <div className="card p-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4">Ações Rápidas</h3>
            
            <div className="space-y-3">
              <button
                onClick={() => navigate(`/vendas/${venda.id}/editar`)}
                className="w-full btn btn-secondary text-left"
              >
                <PencilIcon className="w-4 h-4 mr-2" />
                Editar Venda
              </button>
              
              <button
                onClick={() => navigate(`/clientes/${cliente.id}/detalhes`)}
                className="w-full btn btn-secondary text-left"
              >
                <UserIcon className="w-4 h-4 mr-2" />
                Ver Cliente
              </button>
              
              <button
                onClick={() => navigate(`/apartamentos/${apartamento.id}/detalhes`)}
                className="w-full btn btn-secondary text-left"
              >
                <BuildingOfficeIcon className="w-4 h-4 mr-2" />
                Ver Apartamento
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default VendaDetails;
