import React, { useEffect, useState } from 'react';
import { useNavigate, useParams, useLocation } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';
import { ArrowLeftIcon, MagnifyingGlassIcon } from '@heroicons/react/24/outline';
import { vendaService, CreateVendaData, UpdateVendaData } from '../services/vendaService';
import { clienteService, Cliente } from '../services/clienteService';
import { apartamentoService, Apartamento } from '../services/apartamentoService';

interface VendaFormData extends CreateVendaData {
  statusVenda?: string;
}

const VendaForm: React.FC = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const location = useLocation();
  const isEditing = !!id;
  
  const [isLoading, setIsLoading] = useState(false);
  const [loadingData, setLoadingData] = useState(isEditing);
  const [clientes, setClientes] = useState<Cliente[]>([]);
  const [apartamentos, setApartamentos] = useState<Apartamento[]>([]);
  const [loadingClientes, setLoadingClientes] = useState(true);
  const [loadingApartamentos, setLoadingApartamentos] = useState(true);

  const { 
    register, 
    handleSubmit, 
    formState: { errors }, 
    reset,
    watch,
    setValue
  } = useForm<VendaFormData>();

  const selectedApartamentoId = watch('apartamentoId');

  const preSelectedApartamento = location.state?.apartamentoId;
  const apartamentoInfo = location.state?.apartamentoInfo;

  useEffect(() => {
    loadClientes();
    loadApartamentos();
    
    if (isEditing && id) {
      loadVenda(parseInt(id));
    } else if (preSelectedApartamento) {
      setValue('apartamentoId', preSelectedApartamento);
    }
  }, [id, isEditing, preSelectedApartamento]);

  const loadClientes = async () => {
    try {
      setLoadingClientes(true);
      const data = await clienteService.getAll();
      setClientes(data);
    } catch (error) {
      toast.error('Erro ao carregar clientes');
    } finally {
      setLoadingClientes(false);
    }
  };

  const loadApartamentos = async () => {
    try {
      setLoadingApartamentos(true);
      const data = await apartamentoService.getAll();
      const availableApartamentos = isEditing 
        ? data 
        : data.filter(apt => apt.statusApartamento === 'Disponível');
      setApartamentos(availableApartamentos);
    } catch (error) {
      toast.error('Erro ao carregar apartamentos');
    } finally {
      setLoadingApartamentos(false);
    }
  };

  const loadVenda = async (vendaId: number) => {
    try {
      setLoadingData(true);
      const venda = await vendaService.getById(vendaId);
      
      const formData: VendaFormData = {
        clienteId: venda.clienteId,
        apartamentoId: venda.apartamentoId,
        valorTotal: venda.valorTotal || undefined,
        valorEntrada: venda.valorEntrada || undefined,
        numeroParcelasFinanciamento: venda.numeroParcelasFinanciamento || undefined,
        valorParcelasFinanciamento: venda.valorParcelasFinanciamento || undefined,
        dataVenda: venda.dataVenda ? venda.dataVenda.split('T')[0] : new Date().toISOString().split('T')[0],
        statusVenda: venda.statusVenda,
        observacoes: venda.observacoes || ''
      };
      
      reset(formData);
    } catch (error) {
      toast.error('Erro ao carregar dados da venda');
      navigate('/vendas');
    } finally {
      setLoadingData(false);
    }
  };

  const onSubmit = async (data: VendaFormData) => {
    try {
      setIsLoading(true);
      
      if (isEditing && id) {
        const updateData: UpdateVendaData = {
          clienteId: data.clienteId,
          apartamentoId: data.apartamentoId,
          valorTotal: data.valorTotal,
          valorEntrada: data.valorEntrada,
          numeroParcelasFinanciamento: data.numeroParcelasFinanciamento,
          valorParcelasFinanciamento: data.valorParcelasFinanciamento,
          dataVenda: data.dataVenda,
          statusVenda: data.statusVenda || 'Pendente',
          observacoes: data.observacoes || undefined
        };
        
        await vendaService.update(parseInt(id), updateData);
        toast.success('Venda atualizada com sucesso!');
      } else {
        const createData: CreateVendaData = {
          clienteId: data.clienteId,
          apartamentoId: data.apartamentoId,
          valorTotal: data.valorTotal,
          valorEntrada: data.valorEntrada,
          numeroParcelasFinanciamento: data.numeroParcelasFinanciamento,
          valorParcelasFinanciamento: data.valorParcelasFinanciamento,
          dataVenda: data.dataVenda,
          observacoes: data.observacoes || undefined
        };
        
        await vendaService.create(createData);
        toast.success('Venda cadastrada com sucesso!');
      }
      
      navigate('/vendas');
    } catch (error: any) {
      const message = error.response?.data?.message || 'Erro ao salvar venda';
      toast.error(message);
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

  const getSelectedApartamento = () => {
    return apartamentos.find(apt => apt.id === selectedApartamentoId);
  };

  const calculateFinancing = () => {
    const apartamento = getSelectedApartamento();
    const valorTotal = apartamento?.preco || 0;
    const valorEntrada = watch('valorEntrada') || 0;
    const numeroParcelas = watch('numeroParcelasFinanciamento') || 0;
    
    if (valorTotal && valorEntrada && numeroParcelas) {
      const valorFinanciado = valorTotal - valorEntrada;
      const valorParcela = valorFinanciado / numeroParcelas;
      setValue('valorParcelasFinanciamento', Math.round(valorParcela * 100) / 100);
    }
  };

  if (loadingData || loadingClientes || loadingApartamentos) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center space-x-4">
        <button
          onClick={() => navigate('/vendas')}
          className="p-2 text-gray-400 hover:text-gray-600 rounded-md"
        >
          <ArrowLeftIcon className="h-6 w-6" />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEditing ? 'Editar Venda' : 'Nova Venda'}
          </h1>
          <p className="text-gray-600">
            {isEditing ? 'Altere as informações da venda' : 'Registre uma nova venda de apartamento'}
          </p>
          {apartamentoInfo && (
            <p className="text-sm text-blue-600 mt-1">
              Apartamento pré-selecionado: {apartamentoInfo}
            </p>
          )}
        </div>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <div className="card p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Informações da Venda</h3>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="form-label">Cliente *</label>
              <select 
                className={`form-input ${errors.clienteId ? 'border-red-500' : ''}`}
                {...register('clienteId', { 
                  required: 'Cliente é obrigatório',
                  valueAsNumber: true
                })}
              >
                <option value="">Selecione um cliente...</option>
                {clientes.map(cliente => (
                  <option key={cliente.id} value={cliente.id}>
                    {cliente.nome} - {cliente.cpf}
                  </option>
                ))}
              </select>
              {errors.clienteId && (
                <p className="mt-1 text-sm text-red-600">{errors.clienteId.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Apartamento *</label>
              <select 
                className={`form-input ${errors.apartamentoId ? 'border-red-500' : ''}`}
                {...register('apartamentoId', { 
                  required: 'Apartamento é obrigatório',
                  valueAsNumber: true
                })}
                onChange={(e) => {
                  const value = parseInt(e.target.value);
                  setValue('apartamentoId', value);
                  
                  const apartamento = apartamentos.find(apt => apt.id === value);
                  if (apartamento?.preco) {
                    setValue('valorTotal', apartamento.preco);
                  }
                }}
              >
                <option value="">Selecione um apartamento...</option>
                {apartamentos.map(apartamento => (
                  <option key={apartamento.id} value={apartamento.id}>
                    {apartamento.bloco} - Apt {apartamento.numero} - {apartamento.tipoApartamento}
                    {apartamento.preco ? ` - ${formatCurrency(apartamento.preco)}` : ''}
                  </option>
                ))}
              </select>
              {errors.apartamentoId && (
                <p className="mt-1 text-sm text-red-600">{errors.apartamentoId.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Data da Venda *</label>
              <input
                type="date"
                className={`form-input ${errors.dataVenda ? 'border-red-500' : ''}`}
                {...register('dataVenda', { required: 'Data da venda é obrigatória' })}
              />
              {errors.dataVenda && (
                <p className="mt-1 text-sm text-red-600">{errors.dataVenda.message}</p>
              )}
            </div>

            {isEditing && (
              <div>
                <label className="form-label">Status</label>
                <select className="form-input" {...register('statusVenda')}>
                  <option value="Pendente">Pendente</option>
                  <option value="Confirmada">Confirmada</option>
                  <option value="Em Análise">Em Análise</option>
                  <option value="Cancelada">Cancelada</option>
                </select>
              </div>
            )}
          </div>
        </div>

        {/* Apartment Details */}
        {selectedApartamentoId && (
          <div className="card p-6">
            <h3 className="text-lg font-medium text-gray-900 mb-4">Detalhes do Apartamento Selecionado</h3>
            
            {(() => {
              const apartamento = getSelectedApartamento();
              if (!apartamento) return null;
              
              return (
                <div className="bg-blue-50 rounded-lg p-4">
                  <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                    <div>
                      <span className="text-sm font-medium text-gray-600">Apartamento:</span>
                      <p className="font-semibold">{apartamento.bloco} - Apt {apartamento.numero}</p>
                    </div>
                    <div>
                      <span className="text-sm font-medium text-gray-600">Tipo:</span>
                      <p className="font-semibold">{apartamento.tipoApartamento}</p>
                    </div>
                    <div>
                      <span className="text-sm font-medium text-gray-600">Quartos:</span>
                      <p className="font-semibold">{apartamento.quartos}Q • {apartamento.banheiros}B</p>
                    </div>
                    <div>
                      <span className="text-sm font-medium text-gray-600">Preço:</span>
                      <p className="font-semibold text-primary-600">{formatCurrency(apartamento.preco)}</p>
                    </div>
                  </div>
                </div>
              );
            })()}
          </div>
        )}

        <div className="card p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Valores e Financiamento</h3>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="form-label">Valor Total (R$)</label>
              <input
                type="number"
                step="0.01"
                min="0"
                className="form-input"
                {...register('valorTotal', {
                  min: { value: 0, message: 'Valor deve ser maior que 0' },
                  valueAsNumber: true
                })}
                onChange={(e) => {
                  const value = parseFloat(e.target.value) || 0;
                  setValue('valorTotal', value);
                  calculateFinancing();
                }}
              />
              {errors.valorTotal && (
                <p className="mt-1 text-sm text-red-600">{errors.valorTotal.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Valor de Entrada (R$)</label>
              <input
                type="number"
                step="0.01"
                min="0"
                className="form-input"
                {...register('valorEntrada', {
                  min: { value: 0, message: 'Valor deve ser maior ou igual a 0' },
                  valueAsNumber: true
                })}
                onChange={(e) => {
                  const value = parseFloat(e.target.value) || 0;
                  setValue('valorEntrada', value);
                  calculateFinancing();
                }}
              />
              {errors.valorEntrada && (
                <p className="mt-1 text-sm text-red-600">{errors.valorEntrada.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Número de Parcelas do Financiamento</label>
              <input
                type="number"
                min="1"
                max="420"
                className="form-input"
                {...register('numeroParcelasFinanciamento', {
                  min: { value: 1, message: 'Número de parcelas deve ser maior que 0' },
                  max: { value: 420, message: 'Número de parcelas não pode exceder 420' },
                  valueAsNumber: true
                })}
                onChange={(e) => {
                  const value = parseInt(e.target.value) || 0;
                  setValue('numeroParcelasFinanciamento', value);
                  calculateFinancing();
                }}
              />
              {errors.numeroParcelasFinanciamento && (
                <p className="mt-1 text-sm text-red-600">{errors.numeroParcelasFinanciamento.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Valor das Parcelas do Financiamento (R$)</label>
              <input
                type="number"
                step="0.01"
                min="0"
                className="form-input bg-gray-50"
                readOnly
                {...register('valorParcelasFinanciamento', {
                  min: { value: 0, message: 'Valor deve ser maior ou igual a 0' },
                  valueAsNumber: true
                })}
              />
              <p className="mt-1 text-xs text-gray-500">
                Calculado automaticamente com base no valor total, entrada e número de parcelas
              </p>
              {errors.valorParcelasFinanciamento && (
                <p className="mt-1 text-sm text-red-600">{errors.valorParcelasFinanciamento.message}</p>
              )}
            </div>
          </div>
        </div>

        <div className="card p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Observações</h3>
          <textarea
            rows={4}
            className="form-input"
            placeholder="Observações adicionais sobre a venda..."
            {...register('observacoes')}
          />
        </div>

        {/* Actions */}
        <div className="flex justify-end space-x-4">
          <button
            type="button"
            onClick={() => navigate('/vendas')}
            className="btn btn-secondary"
            disabled={isLoading}
          >
            Cancelar
          </button>
          <button
            type="submit"
            className="btn btn-primary"
            disabled={isLoading}
          >
            {isLoading ? (
              <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin mr-2" />
            ) : null}
            {isEditing ? 'Atualizar' : 'Registrar'} Venda
          </button>
        </div>
      </form>
    </div>
  );
};

export default VendaForm;
