import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';
import { ArrowLeftIcon } from '@heroicons/react/24/outline';
import { apartamentoService, CreateApartamentoData, UpdateApartamentoData } from '../services/apartamentoService';

interface ApartamentoFormData extends CreateApartamentoData {
  statusApartamento?: string;
}

const ApartamentoForm: React.FC = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEditing = !!id;
  
  const [isLoading, setIsLoading] = useState(false);
  const [loadingData, setLoadingData] = useState(isEditing);

  const { 
    register, 
    handleSubmit, 
    formState: { errors }, 
    reset,
    watch
  } = useForm<ApartamentoFormData>();

  const tipoApartamento = watch('tipoApartamento');

  useEffect(() => {
    if (isEditing && id) {
      loadApartamento(parseInt(id));
    }
  }, [id, isEditing]);

  const loadApartamento = async (apartamentoId: number) => {
    try {
      setLoadingData(true);
      const apartamento = await apartamentoService.getById(apartamentoId);
      
      const formData: ApartamentoFormData = {
        numero: apartamento.numero,
        bloco: apartamento.bloco,
        tipoApartamento: apartamento.tipoApartamento,
        quartos: apartamento.quartos,
        banheiros: apartamento.banheiros,
        areaTotal: apartamento.areaTotal || undefined,
        areaPrivativa: apartamento.areaPrivativa || undefined,
        vagasGaragem: apartamento.vagasGaragem || undefined,
        preco: apartamento.preco || undefined,
        endereco: apartamento.endereco || '',
        cidade: apartamento.cidade || '',
        estado: apartamento.estado || '',
        cep: apartamento.cep || '',
        statusApartamento: apartamento.statusApartamento,
        descricao: apartamento.descricao || '',
        caracteristicas: apartamento.caracteristicas || ''
      };
      
      reset(formData);
    } catch (error) {
      toast.error('Erro ao carregar dados do apartamento');
      navigate('/apartamentos');
    } finally {
      setLoadingData(false);
    }
  };

  const onSubmit = async (data: ApartamentoFormData) => {
    try {
      setIsLoading(true);
      
      if (isEditing && id) {
        const updateData: UpdateApartamentoData = {
          numero: data.numero,
          bloco: data.bloco,
          tipoApartamento: data.tipoApartamento,
          quartos: data.quartos,
          banheiros: data.banheiros,
          areaTotal: data.areaTotal,
          areaPrivativa: data.areaPrivativa,
          vagasGaragem: data.vagasGaragem,
          preco: data.preco,
          endereco: data.endereco || undefined,
          cidade: data.cidade || undefined,
          estado: data.estado || undefined,
          cep: data.cep || undefined,
          statusApartamento: data.statusApartamento || 'Disponível',
          descricao: data.descricao || undefined,
          caracteristicas: data.caracteristicas || undefined
        };
        
        await apartamentoService.update(parseInt(id), updateData);
        toast.success('Apartamento atualizado com sucesso!');
      } else {
        const createData: CreateApartamentoData = {
          numero: data.numero,
          bloco: data.bloco,
          tipoApartamento: data.tipoApartamento,
          quartos: data.quartos,
          banheiros: data.banheiros,
          areaTotal: data.areaTotal,
          areaPrivativa: data.areaPrivativa,
          vagasGaragem: data.vagasGaragem,
          preco: data.preco,
          endereco: data.endereco || undefined,
          cidade: data.cidade || undefined,
          estado: data.estado || undefined,
          cep: data.cep || undefined,
          descricao: data.descricao || undefined,
          caracteristicas: data.caracteristicas || undefined
        };
        
        await apartamentoService.create(createData);
        toast.success('Apartamento cadastrado com sucesso!');
      }
      
      navigate('/apartamentos');
    } catch (error: any) {
      const message = error.response?.data?.message || 'Erro ao salvar apartamento';
      toast.error(message);
    } finally {
      setIsLoading(false);
    }
  };

  const formatCEP = (value: string) => {
    return value
      .replace(/\D/g, '')
      .replace(/(\d{5})(\d)/, '$1-$2')
      .replace(/(-\d{3})\d+?$/, '$1');
  };

  const tiposApartamento = [
    'Apartamento 1 Quarto',
    'Apartamento 2 Quartos',
    'Apartamento 3 Quartos',
    'Apartamento 4 Quartos',
    'Cobertura Duplex',
    'Cobertura Linear',
    'Studio',
    'Loft',
    'Casa Térrea',
    'Casa Sobrado'
  ];

  if (loadingData) {
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
          onClick={() => navigate('/apartamentos')}
          className="p-2 text-gray-400 hover:text-gray-600 rounded-md"
        >
          <ArrowLeftIcon className="h-6 w-6" />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEditing ? 'Editar Apartamento' : 'Novo Apartamento'}
          </h1>
          <p className="text-gray-600">
            {isEditing ? 'Altere as informações do apartamento' : 'Cadastre um novo apartamento no catálogo'}
          </p>
        </div>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <div className="card p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Informações Básicas</h3>
          
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="form-label">Número *</label>
              <input
                type="text"
                className={`form-input ${errors.numero ? 'border-red-500' : ''}`}
                {...register('numero', { 
                  required: 'Número é obrigatório',
                  minLength: { value: 1, message: 'Número deve ter pelo menos 1 caractere' }
                })}
              />
              {errors.numero && (
                <p className="mt-1 text-sm text-red-600">{errors.numero.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Bloco *</label>
              <input
                type="text"
                className={`form-input ${errors.bloco ? 'border-red-500' : ''}`}
                {...register('bloco', { 
                  required: 'Bloco é obrigatório',
                  minLength: { value: 1, message: 'Bloco deve ter pelo menos 1 caractere' }
                })}
              />
              {errors.bloco && (
                <p className="mt-1 text-sm text-red-600">{errors.bloco.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Tipo de Apartamento *</label>
              <select 
                className={`form-input ${errors.tipoApartamento ? 'border-red-500' : ''}`}
                {...register('tipoApartamento', { required: 'Tipo é obrigatório' })}
              >
                <option value="">Selecione o tipo...</option>
                {tiposApartamento.map(tipo => (
                  <option key={tipo} value={tipo}>{tipo}</option>
                ))}
              </select>
              {errors.tipoApartamento && (
                <p className="mt-1 text-sm text-red-600">{errors.tipoApartamento.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Quartos *</label>
              <input
                type="number"
                min="0"
                className={`form-input ${errors.quartos ? 'border-red-500' : ''}`}
                {...register('quartos', { 
                  required: 'Número de quartos é obrigatório',
                  min: { value: 0, message: 'Número de quartos deve ser maior ou igual a 0' },
                  valueAsNumber: true
                })}
              />
              {errors.quartos && (
                <p className="mt-1 text-sm text-red-600">{errors.quartos.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Banheiros *</label>
              <input
                type="number"
                min="1"
                className={`form-input ${errors.banheiros ? 'border-red-500' : ''}`}
                {...register('banheiros', { 
                  required: 'Número de banheiros é obrigatório',
                  min: { value: 1, message: 'Deve ter pelo menos 1 banheiro' },
                  valueAsNumber: true
                })}
              />
              {errors.banheiros && (
                <p className="mt-1 text-sm text-red-600">{errors.banheiros.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Vagas de Garagem</label>
              <input
                type="number"
                min="0"
                className="form-input"
                {...register('vagasGaragem', {
                  min: { value: 0, message: 'Vagas deve ser maior ou igual a 0' },
                  valueAsNumber: true
                })}
              />
              {errors.vagasGaragem && (
                <p className="mt-1 text-sm text-red-600">{errors.vagasGaragem.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Área Total (m²)</label>
              <input
                type="number"
                step="0.01"
                min="0"
                className="form-input"
                {...register('areaTotal', {
                  min: { value: 0, message: 'Área deve ser maior que 0' },
                  valueAsNumber: true
                })}
              />
              {errors.areaTotal && (
                <p className="mt-1 text-sm text-red-600">{errors.areaTotal.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Área Privativa (m²)</label>
              <input
                type="number"
                step="0.01"
                min="0"
                className="form-input"
                {...register('areaPrivativa', {
                  min: { value: 0, message: 'Área deve ser maior que 0' },
                  valueAsNumber: true
                })}
              />
              {errors.areaPrivativa && (
                <p className="mt-1 text-sm text-red-600">{errors.areaPrivativa.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Preço (R$)</label>
              <input
                type="number"
                step="0.01"
                min="0"
                className="form-input"
                {...register('preco', {
                  min: { value: 0, message: 'Preço deve ser maior que 0' },
                  valueAsNumber: true
                })}
              />
              {errors.preco && (
                <p className="mt-1 text-sm text-red-600">{errors.preco.message}</p>
              )}
            </div>

            {isEditing && (
              <div>
                <label className="form-label">Status</label>
                <select className="form-input" {...register('statusApartamento')}>
                  <option value="Disponível">Disponível</option>
                  <option value="Reservado">Reservado</option>
                  <option value="Vendido">Vendido</option>
                  <option value="Indisponível">Indisponível</option>
                </select>
              </div>
            )}
          </div>
        </div>

        <div className="card p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Localização</h3>
          
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div className="lg:col-span-2">
              <label className="form-label">Endereço Completo</label>
              <input
                type="text"
                className="form-input"
                placeholder="Rua, número, complemento"
                {...register('endereco')}
              />
            </div>

            <div>
              <label className="form-label">CEP</label>
              <input
                type="text"
                maxLength={9}
                className="form-input"
                {...register('cep')}
                onChange={(e) => {
                  e.target.value = formatCEP(e.target.value);
                }}
              />
            </div>

            <div>
              <label className="form-label">Cidade</label>
              <input
                type="text"
                className="form-input"
                {...register('cidade')}
              />
            </div>

            <div>
              <label className="form-label">Estado</label>
              <select className="form-input" {...register('estado')}>
                <option value="">Selecione...</option>
                <option value="AC">Acre</option>
                <option value="AL">Alagoas</option>
                <option value="AP">Amapá</option>
                <option value="AM">Amazonas</option>
                <option value="BA">Bahia</option>
                <option value="CE">Ceará</option>
                <option value="DF">Distrito Federal</option>
                <option value="ES">Espírito Santo</option>
                <option value="GO">Goiás</option>
                <option value="MA">Maranhão</option>
                <option value="MT">Mato Grosso</option>
                <option value="MS">Mato Grosso do Sul</option>
                <option value="MG">Minas Gerais</option>
                <option value="PA">Pará</option>
                <option value="PB">Paraíba</option>
                <option value="PR">Paraná</option>
                <option value="PE">Pernambuco</option>
                <option value="PI">Piauí</option>
                <option value="RJ">Rio de Janeiro</option>
                <option value="RN">Rio Grande do Norte</option>
                <option value="RS">Rio Grande do Sul</option>
                <option value="RO">Rondônia</option>
                <option value="RR">Roraima</option>
                <option value="SC">Santa Catarina</option>
                <option value="SP">São Paulo</option>
                <option value="SE">Sergipe</option>
                <option value="TO">Tocantins</option>
              </select>
            </div>
          </div>
        </div>

        <div className="card p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Descrição e Características</h3>
          
          <div className="space-y-4">
            <div>
              <label className="form-label">Descrição</label>
              <textarea
                rows={4}
                className="form-input"
                placeholder="Descrição detalhada do apartamento..."
                {...register('descricao')}
              />
            </div>

            <div>
              <label className="form-label">Características Especiais</label>
              <textarea
                rows={3}
                className="form-input"
                placeholder="Sacada, churrasqueira, piscina, academia, etc..."
                {...register('caracteristicas')}
              />
            </div>
          </div>
        </div>

        {/* Actions */}
        <div className="flex justify-end space-x-4">
          <button
            type="button"
            onClick={() => navigate('/apartamentos')}
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
            {isEditing ? 'Atualizar' : 'Cadastrar'} Apartamento
          </button>
        </div>
      </form>
    </div>
  );
};

export default ApartamentoForm;
