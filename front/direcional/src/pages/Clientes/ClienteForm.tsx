import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';
import { ArrowLeftIcon } from '@heroicons/react/24/outline';
import { clienteService, CreateClienteData, UpdateClienteData, Cliente } from '../services/clienteService';

interface ClienteFormData extends CreateClienteData {
  statusCliente?: string;
}

const ClienteForm: React.FC = () => {
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
  } = useForm<ClienteFormData>();

  useEffect(() => {
    if (isEditing && id) {
      loadCliente(parseInt(id));
    }
  }, [id, isEditing]);

  const loadCliente = async (clienteId: number) => {
    try {
      setLoadingData(true);
      const cliente = await clienteService.getById(clienteId);
      
      // Format data for form
      const formData: ClienteFormData = {
        nome: cliente.nome,
        cpf: cliente.cpf,
        email: cliente.email || '',
        telefone: cliente.telefone || '',
        endereco: cliente.endereco || '',
        cidade: cliente.cidade || '',
        estado: cliente.estado || '',
        cep: cliente.cep || '',
        dataNascimento: cliente.dataNascimento ? cliente.dataNascimento.split('T')[0] : '',
        rendaMensal: cliente.rendaMensal || 0,
        statusCliente: cliente.statusCliente,
        observacoes: cliente.observacoes || ''
      };
      
      reset(formData);
    } catch (error) {
      toast.error('Erro ao carregar dados do cliente');
      navigate('/clientes');
    } finally {
      setLoadingData(false);
    }
  };

  const onSubmit = async (data: ClienteFormData) => {
    try {
      setIsLoading(true);
      
      if (isEditing && id) {
        const updateData: UpdateClienteData = {
          nome: data.nome,
          email: data.email || undefined,
          telefone: data.telefone || undefined,
          endereco: data.endereco || undefined,
          cidade: data.cidade || undefined,
          estado: data.estado || undefined,
          cep: data.cep || undefined,
          dataNascimento: data.dataNascimento || undefined,
          rendaMensal: data.rendaMensal || undefined,
          statusCliente: data.statusCliente || 'Ativo',
          observacoes: data.observacoes || undefined
        };
        
        await clienteService.update(parseInt(id), updateData);
        toast.success('Cliente atualizado com sucesso!');
      } else {
        const createData: CreateClienteData = {
          nome: data.nome,
          cpf: data.cpf,
          email: data.email || undefined,
          telefone: data.telefone || undefined,
          endereco: data.endereco || undefined,
          cidade: data.cidade || undefined,
          estado: data.estado || undefined,
          cep: data.cep || undefined,
          dataNascimento: data.dataNascimento || undefined,
          rendaMensal: data.rendaMensal || undefined,
          observacoes: data.observacoes || undefined
        };
        
        await clienteService.create(createData);
        toast.success('Cliente cadastrado com sucesso!');
      }
      
      navigate('/clientes');
    } catch (error: any) {
      const message = error.response?.data?.message || 'Erro ao salvar cliente';
      toast.error(message);
    } finally {
      setIsLoading(false);
    }
  };

  const formatCPF = (value: string) => {
    return value
      .replace(/\D/g, '')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d{1,2})/, '$1-$2')
      .replace(/(-\d{2})\d+?$/, '$1');
  };

  const formatCEP = (value: string) => {
    return value
      .replace(/\D/g, '')
      .replace(/(\d{5})(\d)/, '$1-$2')
      .replace(/(-\d{3})\d+?$/, '$1');
  };

  const formatPhone = (value: string) => {
    return value
      .replace(/\D/g, '')
      .replace(/(\d{2})(\d)/, '($1) $2')
      .replace(/(\d{4,5})(\d{4})/, '$1-$2')
      .replace(/(-\d{4})\d+?$/, '$1');
  };

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
          onClick={() => navigate('/clientes')}
          className="p-2 text-gray-400 hover:text-gray-600 rounded-md"
        >
          <ArrowLeftIcon className="h-6 w-6" />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEditing ? 'Editar Cliente' : 'Novo Cliente'}
          </h1>
          <p className="text-gray-600">
            {isEditing ? 'Altere as informações do cliente' : 'Cadastre um novo cliente no sistema'}
          </p>
        </div>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <div className="card p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Informações Pessoais</h3>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="md:col-span-2">
              <label className="form-label">Nome Completo *</label>
              <input
                type="text"
                className={`form-input ${errors.nome ? 'border-red-500' : ''}`}
                {...register('nome', { 
                  required: 'Nome é obrigatório',
                  minLength: { value: 2, message: 'Nome deve ter pelo menos 2 caracteres' }
                })}
              />
              {errors.nome && (
                <p className="mt-1 text-sm text-red-600">{errors.nome.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">CPF *</label>
              <input
                type="text"
                maxLength={14}
                className={`form-input ${errors.cpf ? 'border-red-500' : ''}`}
                disabled={isEditing}
                {...register('cpf', { 
                  required: 'CPF é obrigatório',
                  pattern: {
                    value: /^\d{3}\.\d{3}\.\d{3}-\d{2}$/,
                    message: 'CPF deve ter formato válido (000.000.000-00)'
                  }
                })}
                onChange={(e) => {
                  e.target.value = formatCPF(e.target.value);
                }}
              />
              {errors.cpf && (
                <p className="mt-1 text-sm text-red-600">{errors.cpf.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Data de Nascimento</label>
              <input
                type="date"
                className="form-input"
                {...register('dataNascimento')}
              />
            </div>

            <div>
              <label className="form-label">Email</label>
              <input
                type="email"
                className={`form-input ${errors.email ? 'border-red-500' : ''}`}
                {...register('email', {
                  pattern: {
                    value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                    message: 'Email deve ter formato válido'
                  }
                })}
              />
              {errors.email && (
                <p className="mt-1 text-sm text-red-600">{errors.email.message}</p>
              )}
            </div>

            <div>
              <label className="form-label">Telefone</label>
              <input
                type="tel"
                className="form-input"
                {...register('telefone')}
                onChange={(e) => {
                  e.target.value = formatPhone(e.target.value);
                }}
              />
            </div>

            <div>
              <label className="form-label">Renda Mensal</label>
              <input
                type="number"
                step="0.01"
                min="0"
                className="form-input"
                {...register('rendaMensal', {
                  min: { value: 0, message: 'Renda deve ser maior ou igual a zero' }
                })}
              />
              {errors.rendaMensal && (
                <p className="mt-1 text-sm text-red-600">{errors.rendaMensal.message}</p>
              )}
            </div>

            {isEditing && (
              <div>
                <label className="form-label">Status</label>
                <select className="form-input" {...register('statusCliente')}>
                  <option value="Ativo">Ativo</option>
                  <option value="Inativo">Inativo</option>
                  <option value="Prospecto">Prospecto</option>
                </select>
              </div>
            )}
          </div>
        </div>

        <div className="card p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Endereço</h3>
          
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div className="lg:col-span-2">
              <label className="form-label">Endereço</label>
              <input
                type="text"
                className="form-input"
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
          <h3 className="text-lg font-medium text-gray-900 mb-4">Observações</h3>
          <textarea
            rows={4}
            className="form-input"
            placeholder="Observações adicionais sobre o cliente..."
            {...register('observacoes')}
          />
        </div>

        {/* Actions */}
        <div className="flex justify-end space-x-4">
          <button
            type="button"
            onClick={() => navigate('/clientes')}
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
            {isEditing ? 'Atualizar' : 'Cadastrar'} Cliente
          </button>
        </div>
      </form>
    </div>
  );
};

export default ClienteForm;
