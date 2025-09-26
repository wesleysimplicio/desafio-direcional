import React, { useState } from 'react';
import { Navigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { useAuth } from '../contexts/AuthContext';
import { BuildingOfficeIcon, EyeIcon, EyeSlashIcon } from '@heroicons/react/24/outline';
import { LoginData } from '../services/authService';

const Login: React.FC = () => {
  const { login, isAuthenticated, isLoading } = useAuth();
  const [showPassword, setShowPassword] = useState(false);
  const { register, handleSubmit, formState: { errors } } = useForm<LoginData>();

  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  const onSubmit = async (data: LoginData) => {
    try {
      await login(data);
    } catch (error) {
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <div>
          <div className="flex justify-center">
            <BuildingOfficeIcon className="h-16 w-16 text-primary-600" />
          </div>
          <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
            Sistema Direcional
          </h2>
          <p className="mt-2 text-center text-sm text-gray-600">
            Faça login para acessar o sistema
          </p>
        </div>

        <form className="mt-8 space-y-6" onSubmit={handleSubmit(onSubmit)}>
          <div className="space-y-4">
            <div>
              <label htmlFor="username" className="form-label">
                Usuário
              </label>
              <input
                id="username"
                type="text"
                autoComplete="username"
                className={`form-input ${errors.username ? 'border-red-500' : ''}`}
                placeholder="Digite seu usuário"
                {...register('username', {
                  required: 'Usuário é obrigatório',
                  minLength: {
                    value: 3,
                    message: 'Usuário deve ter pelo menos 3 caracteres'
                  }
                })}
              />
              {errors.username && (
                <p className="mt-1 text-sm text-red-600">{errors.username.message}</p>
              )}
            </div>

            <div>
              <label htmlFor="password" className="form-label">
                Senha
              </label>
              <div className="relative">
                <input
                  id="password"
                  type={showPassword ? 'text' : 'password'}
                  autoComplete="current-password"
                  className={`form-input pr-10 ${errors.password ? 'border-red-500' : ''}`}
                  placeholder="Digite sua senha"
                  {...register('password', {
                    required: 'Senha é obrigatória',
                    minLength: {
                      value: 6,
                      message: 'Senha deve ter pelo menos 6 caracteres'
                    }
                  })}
                />
                <button
                  type="button"
                  className="absolute inset-y-0 right-0 pr-3 flex items-center"
                  onClick={() => setShowPassword(!showPassword)}
                >
                  {showPassword ? (
                    <EyeSlashIcon className="h-5 w-5 text-gray-400" />
                  ) : (
                    <EyeIcon className="h-5 w-5 text-gray-400" />
                  )}
                </button>
              </div>
              {errors.password && (
                <p className="mt-1 text-sm text-red-600">{errors.password.message}</p>
              )}
            </div>
          </div>

          <div>
            <button
              type="submit"
              disabled={isLoading}
              className="group relative w-full flex justify-center py-3 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? (
                <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
              ) : (
                'Entrar'
              )}
            </button>
          </div>

          <div className="mt-6">
            <div className="relative">
              <div className="absolute inset-0 flex items-center">
                <div className="w-full border-t border-gray-300" />
              </div>
              <div className="relative flex justify-center text-sm">
                <span className="px-2 bg-gray-50 text-gray-500">Credenciais de teste</span>
              </div>
            </div>
            <div className="mt-4 text-center text-sm text-gray-600">
              <p><strong>Admin:</strong> admin / admin123</p>
              <p><strong>Usuário:</strong> user / admin123</p>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
};

export default Login;
