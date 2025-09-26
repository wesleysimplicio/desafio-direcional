import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { 
  ArrowLeftIcon,
  PencilIcon,
  BuildingOfficeIcon,
  MapPinIcon,
  CurrencyDollarIcon,
  HomeIcon,
  CalendarDaysIcon,
  CheckCircleIcon
} from '@heroicons/react/24/outline';
import { apartamentoService, Apartamento } from '../services/apartamentoService';

const ApartamentoDetails: React.FC = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [apartamento, setApartamento] = useState<Apartamento | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    if (id) {
      loadApartamento(parseInt(id));
    }
  }, [id]);

  const loadApartamento = async (apartamentoId: number) => {
    try {
      setIsLoading(true);
      const data = await apartamentoService.getById(apartamentoId);
      setApartamento(data);
    } catch (error) {
      toast.error('Erro ao carregar dados do apartamento');
      navigate('/apartamentos');
    } finally {
      setIsLoading(false);
    }
  };

  const formatCurrency = (value?: number) => {
    if (!value) return 'Não informado';
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
      'Disponível': 'bg-green-100 text-green-800 border-green-200',
      'Reservado': 'bg-yellow-100 text-yellow-800 border-yellow-200',
      'Vendido': 'bg-blue-100 text-blue-800 border-blue-200',
      'Indisponível': 'bg-red-100 text-red-800 border-red-200'
    };
    
    return badges[status as keyof typeof badges] || 'bg-gray-100 text-gray-800 border-gray-200';
  };

  const getTipoIcon = (tipo: string) => {
    if (tipo.includes('Casa')) return '🏠';
    if (tipo.includes('Cobertura')) return '🏢';
    return '🏠';
  };

  const handleReservar = () => {
    if (!apartamento) return;
    
    navigate('/vendas/nova', { 
      state: { 
        apartamentoId: apartamento.id,
        apartamentoInfo: `${apartamento.bloco} - Apt ${apartamento.numero}`
      }
    });
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (!apartamento) {
    return (
      <div className="text-center py-12">
        <BuildingOfficeIcon className="mx-auto h-12 w-12 text-gray-400" />
        <h3 className="mt-2 text-sm font-medium text-gray-900">Apartamento não encontrado</h3>
        <p className="mt-1 text-sm text-gray-500">O apartamento que você está procurando não existe.</p>
        <div className="mt-6">
          <button
            onClick={() => navigate('/apartamentos')}
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
            onClick={() => navigate('/apartamentos')}
            className="p-2 text-gray-400 hover:text-gray-600 rounded-md"
          >
            <ArrowLeftIcon className="h-6 w-6" />
          </button>
          <div className="flex items-center space-x-3">
            <div className="text-3xl">{getTipoIcon(apartamento.tipoApartamento)}</div>
            <div>
              <h1 className="text-2xl font-bold text-gray-900">
                {apartamento.bloco} - Apartamento {apartamento.numero}
              </h1>
              <p className="text-gray-600">{apartamento.tipoApartamento}</p>
            </div>
          </div>
        </div>
        
        <div className="flex space-x-3">
          <button
            onClick={() => navigate(`/apartamentos/${apartamento.id}/editar`)}
            className="btn btn-secondary"
          >
            <PencilIcon className="w-5 h-5 mr-2" />
            Editar
          </button>
          
          {apartamento.statusApartamento === 'Disponível' && (
            <button
              onClick={handleReservar}
              className="btn btn-primary"
            >
              <CheckCircleIcon className="w-5 h-5 mr-2" />
              Reservar
            </button>
          )}
        </div>
      </div>

      {/* Status Badge */}
      <div>
        <span className={`inline-flex px-3 py-1 text-sm font-semibold rounded-full border ${getStatusBadge(apartamento.statusApartamento)}`}>
          {apartamento.statusApartamento}
        </span>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Main Info */}
        <div className="lg:col-span-2 space-y-6">
          {/* Basic Info */}
          <div className="card p-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
              <HomeIcon className="w-5 h-5 mr-2" />
              Informações Básicas
            </h3>
            
            <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
              <div className="text-center p-4 bg-blue-50 rounded-lg">
                <div className="text-2xl font-bold text-blue-600">{apartamento.quartos}</div>
                <div className="text-sm text-gray-600">Quartos</div>
              </div>
              
              <div className="text-center p-4 bg-green-50 rounded-lg">
                <div className="text-2xl font-bold text-green-600">{apartamento.banheiros}</div>
                <div className="text-sm text-gray-600">Banheiros</div>
              </div>
              
              <div className="text-center p-4 bg-purple-50 rounded-lg">
                <div className="text-2xl font-bold text-purple-600">{apartamento.vagasGaragem || 0}</div>
                <div className="text-sm text-gray-600">Garagem</div>
              </div>
              
              <div className="text-center p-4 bg-orange-50 rounded-lg">
                <div className="text-2xl font-bold text-orange-600">
                  {apartamento.areaTotal ? `${apartamento.areaTotal}m²` : '-'}
                </div>
                <div className="text-sm text-gray-600">Área Total</div>
              </div>
            </div>

            {apartamento.areaPrivativa && (
              <div className="mt-4 p-4 bg-gray-50 rounded-lg">
                <div className="text-sm text-gray-600">Área Privativa</div>
                <div className="text-lg font-semibold text-gray-900">{apartamento.areaPrivativa}m²</div>
              </div>
            )}
          </div>

          {/* Location */}
          <div className="card p-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
              <MapPinIcon className="w-5 h-5 mr-2" />
              Localização
            </h3>
            
            <div className="space-y-3">
              {apartamento.endereco && (
                <div>
                  <span className="text-sm font-medium text-gray-600">Endereço: </span>
                  <span className="text-gray-900">{apartamento.endereco}</span>
                </div>
              )}
              
              <div className="flex space-x-4">
                {apartamento.cidade && (
                  <div>
                    <span className="text-sm font-medium text-gray-600">Cidade: </span>
                    <span className="text-gray-900">{apartamento.cidade}</span>
                  </div>
                )}
                
                {apartamento.estado && (
                  <div>
                    <span className="text-sm font-medium text-gray-600">Estado: </span>
                    <span className="text-gray-900">{apartamento.estado}</span>
                  </div>
                )}
                
                {apartamento.cep && (
                  <div>
                    <span className="text-sm font-medium text-gray-600">CEP: </span>
                    <span className="text-gray-900">{apartamento.cep}</span>
                  </div>
                )}
              </div>
            </div>
          </div>

          {/* Description */}
          {apartamento.descricao && (
            <div className="card p-6">
              <h3 className="text-lg font-medium text-gray-900 mb-4">Descrição</h3>
              <p className="text-gray-700 whitespace-pre-wrap">{apartamento.descricao}</p>
            </div>
          )}

          {/* Characteristics */}
          {apartamento.caracteristicas && (
            <div className="card p-6">
              <h3 className="text-lg font-medium text-gray-900 mb-4">Características</h3>
              <p className="text-gray-700 whitespace-pre-wrap">{apartamento.caracteristicas}</p>
            </div>
          )}
        </div>

        {/* Sidebar */}
        <div className="space-y-6">
          {/* Price */}
          <div className="card p-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4 flex items-center">
              <CurrencyDollarIcon className="w-5 h-5 mr-2" />
              Preço
            </h3>
            
            <div className="text-center">
              <div className="text-3xl font-bold text-primary-600">
                {formatCurrency(apartamento.preco)}
              </div>
              {apartamento.areaTotal && apartamento.preco && (
                <div className="text-sm text-gray-500 mt-1">
                  {formatCurrency(apartamento.preco / apartamento.areaTotal)}/m²
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
                <span className="text-gray-600">ID:</span>
                <span className="font-medium">#{apartamento.id}</span>
              </div>
              
              <div className="flex justify-between">
                <span className="text-gray-600">Cadastrado em:</span>
                <span className="font-medium">{formatDate(apartamento.dataCadastro)}</span>
              </div>
              
              {apartamento.dataAtualizacao && (
                <div className="flex justify-between">
                  <span className="text-gray-600">Atualizado em:</span>
                  <span className="font-medium">{formatDate(apartamento.dataAtualizacao)}</span>
                </div>
              )}
            </div>
          </div>

          {/* Quick Actions */}
          <div className="card p-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4">Ações Rápidas</h3>
            
            <div className="space-y-3">
              <button
                onClick={() => navigate(`/apartamentos/${apartamento.id}/editar`)}
                className="w-full btn btn-secondary text-left"
              >
                <PencilIcon className="w-4 h-4 mr-2" />
                Editar Apartamento
              </button>
              
              {apartamento.statusApartamento === 'Disponível' && (
                <button
                  onClick={handleReservar}
                  className="w-full btn btn-primary text-left"
                >
                  <CheckCircleIcon className="w-4 h-4 mr-2" />
                  Iniciar Venda
                </button>
              )}
              
              <button
                onClick={() => navigate('/vendas', { 
                  state: { apartamentoFilter: apartamento.id }
                })}
                className="w-full btn btn-secondary text-left"
              >
                <CalendarDaysIcon className="w-4 h-4 mr-2" />
                Ver Histórico de Vendas
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ApartamentoDetails;
