import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { 
  UsersIcon, 
  BuildingOfficeIcon, 
  CurrencyDollarIcon,
  TrendingUpIcon,
  TrendingDownIcon
} from '@heroicons/react/24/outline';
import { clienteService } from '../services/clienteService';
import { apartamentoService } from '../services/apartamentoService';
import { vendaService } from '../services/vendaService';
import { useAuth } from '../contexts/AuthContext';

interface DashboardStats {
  totalClientes: number;
  totalApartamentos: number;
  apartamentosDisponiveis: number;
  apartamentosVendidos: number;
  totalVendas: number;
  vendasAtivas: number;
  valorTotalVendas: number;
}

const Dashboard: React.FC = () => {
  const { user } = useAuth();
  const [stats, setStats] = useState<DashboardStats>({
    totalClientes: 0,
    totalApartamentos: 0,
    apartamentosDisponiveis: 0,
    apartamentosVendidos: 0,
    totalVendas: 0,
    vendasAtivas: 0,
    valorTotalVendas: 0
  });
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadDashboardData = async () => {
      try {
        setIsLoading(true);

        const [clientes, apartamentos, vendas] = await Promise.all([
          clienteService.getAll(),
          apartamentoService.getAll(),
          vendaService.getAll()
        ]);

        const apartamentosDisponiveis = apartamentos.filter(apt => apt.statusApartamento === 'Disponível').length;
        const apartamentosVendidos = apartamentos.filter(apt => apt.statusApartamento === 'Vendido').length;
        const vendasAtivas = vendas.filter(venda => venda.statusVenda === 'Ativa').length;
        const valorTotalVendas = vendas.reduce((total, venda) => total + venda.valorVenda, 0);

        setStats({
          totalClientes: clientes.length,
          totalApartamentos: apartamentos.length,
          apartamentosDisponiveis,
          apartamentosVendidos,
          totalVendas: vendas.length,
          vendasAtivas,
          valorTotalVendas
        });
      } catch (error) {
        console.error('Erro ao carregar dados do dashboard:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadDashboardData();
  }, []);

  const cards = [
    {
      title: 'Total de Clientes',
      value: stats.totalClientes,
      icon: UsersIcon,
      color: 'text-blue-600',
      bgColor: 'bg-blue-50',
      link: '/clientes'
    },
    {
      title: 'Total de Apartamentos',
      value: stats.totalApartamentos,
      icon: BuildingOfficeIcon,
      color: 'text-green-600',
      bgColor: 'bg-green-50',
      link: '/apartamentos'
    },
    {
      title: 'Apartamentos Disponíveis',
      value: stats.apartamentosDisponiveis,
      icon: TrendingUpIcon,
      color: 'text-emerald-600',
      bgColor: 'bg-emerald-50',
      link: '/apartamentos'
    },
    {
      title: 'Total de Vendas',
      value: stats.totalVendas,
      icon: CurrencyDollarIcon,
      color: 'text-purple-600',
      bgColor: 'bg-purple-50',
      link: '/vendas'
    }
  ];

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
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
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600">Bem-vindo ao sistema, {user?.username}!</p>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {cards.map((card) => {
          const Icon = card.icon;
          return (
            <Link 
              key={card.title}
              to={card.link}
              className="card p-6 hover:shadow-lg transition-shadow cursor-pointer"
            >
              <div className="flex items-center">
                <div className={`p-2 rounded-lg ${card.bgColor}`}>
                  <Icon className={`h-8 w-8 ${card.color}`} />
                </div>
                <div className="ml-4">
                  <p className="text-sm font-medium text-gray-600">{card.title}</p>
                  <p className="text-2xl font-bold text-gray-900">{card.value}</p>
                </div>
              </div>
            </Link>
          );
        })}
      </div>

      {/* Detailed Stats */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Apartamentos */}
        <div className="card p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Situação dos Apartamentos</h3>
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <span className="flex items-center">
                <div className="w-3 h-3 bg-green-500 rounded-full mr-2"></div>
                Disponíveis
              </span>
              <span className="font-medium">{stats.apartamentosDisponiveis}</span>
            </div>
            <div className="flex items-center justify-between">
              <span className="flex items-center">
                <div className="w-3 h-3 bg-red-500 rounded-full mr-2"></div>
                Vendidos
              </span>
              <span className="font-medium">{stats.apartamentosVendidos}</span>
            </div>
            <div className="flex items-center justify-between">
              <span className="flex items-center">
                <div className="w-3 h-3 bg-yellow-500 rounded-full mr-2"></div>
                Outros Status
              </span>
              <span className="font-medium">
                {stats.totalApartamentos - stats.apartamentosDisponiveis - stats.apartamentosVendidos}
              </span>
            </div>
          </div>
        </div>

        {/* Vendas */}
        <div className="card p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Resumo de Vendas</h3>
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <span className="text-gray-600">Total de Vendas</span>
              <span className="font-medium">{stats.totalVendas}</span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-gray-600">Vendas Ativas</span>
              <span className="font-medium text-green-600">{stats.vendasAtivas}</span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-gray-600">Valor Total</span>
              <span className="font-medium text-blue-600">
                {formatCurrency(stats.valorTotalVendas)}
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="card p-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">Ações Rápidas</h3>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Link
            to="/clientes/novo"
            className="flex items-center p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
          >
            <UsersIcon className="h-8 w-8 text-blue-600 mr-3" />
            <span className="font-medium text-blue-700">Cadastrar Cliente</span>
          </Link>
          <Link
            to="/apartamentos/novo"
            className="flex items-center p-4 bg-green-50 rounded-lg hover:bg-green-100 transition-colors"
          >
            <BuildingOfficeIcon className="h-8 w-8 text-green-600 mr-3" />
            <span className="font-medium text-green-700">Cadastrar Apartamento</span>
          </Link>
          <Link
            to="/vendas/nova"
            className="flex items-center p-4 bg-purple-50 rounded-lg hover:bg-purple-100 transition-colors"
          >
            <CurrencyDollarIcon className="h-8 w-8 text-purple-600 mr-3" />
            <span className="font-medium text-purple-700">Nova Venda</span>
          </Link>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
