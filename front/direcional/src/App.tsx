import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Layout from './components/Layout';

// Auth Pages
import Login from './pages/Login';

// Dashboard
import Dashboard from './pages/Dashboard';

// Cliente Pages  
import ClientesList from './pages/Clientes/ClientesList';
import ClienteForm from './pages/Clientes/ClienteForm';

// Apartamento Pages
import ApartamentosList from './pages/Apartamentos/ApartamentosList';
import ApartamentoForm from './pages/Apartamentos/ApartamentoForm';
import ApartamentoDetails from './pages/Apartamentos/ApartamentoDetails';

// Venda Pages
import VendasList from './pages/Vendas/VendasList';
import VendaForm from './pages/Vendas/VendaForm';
import VendaDetails from './pages/Vendas/VendaDetails';

// Styles
import 'react-toastify/dist/ReactToastify.css';
import './App.css';

const App: React.FC = () => {
  return (
    <AuthProvider>
      <Router>
        <div className="App">
          <Routes>
            {/* Public Routes */}
            <Route path="/login" element={<Login />} />
            
            {/* Protected Routes */}
            <Route path="/" element={
              <ProtectedRoute>
                <Layout />
              </ProtectedRoute>
            }>
              {/* Dashboard */}
              <Route index element={<Navigate to="/dashboard" replace />} />
              <Route path="dashboard" element={<Dashboard />} />
              
              {/* Clientes Routes */}
              <Route path="clientes" element={<ClientesList />} />
              <Route path="clientes/novo" element={<ClienteForm />} />
              <Route path="clientes/:id/editar" element={<ClienteForm />} />
              <Route path="clientes/:id/detalhes" element={
                <div className="p-6">
                  <div className="text-center">
                    <h2 className="text-xl font-semibold text-gray-900">Detalhes do Cliente</h2>
                    <p className="text-gray-600 mt-2">Esta página será implementada em breve.</p>
                  </div>
                </div>
              } />
              
              {/* Apartamentos Routes */}
              <Route path="apartamentos" element={<ApartamentosList />} />
              <Route path="apartamentos/novo" element={<ApartamentoForm />} />
              <Route path="apartamentos/:id/editar" element={<ApartamentoForm />} />
              <Route path="apartamentos/:id/detalhes" element={<ApartamentoDetails />} />
              
              {/* Vendas Routes */}
              <Route path="vendas" element={<VendasList />} />
              <Route path="vendas/nova" element={<VendaForm />} />
              <Route path="vendas/:id/editar" element={<VendaForm />} />
              <Route path="vendas/:id/detalhes" element={<VendaDetails />} />
              
              {/* Fallback for undefined routes within protected area */}
              <Route path="*" element={
                <div className="p-6">
                  <div className="text-center">
                    <h2 className="text-xl font-semibold text-gray-900">Página não encontrada</h2>
                    <p className="text-gray-600 mt-2">A página que você está procurando não existe.</p>
                    <button 
                      onClick={() => window.history.back()}
                      className="btn btn-primary mt-4"
                    >
                      Voltar
                    </button>
                  </div>
                </div>
              } />
            </Route>
            
            {/* Redirect root to dashboard */}
            <Route path="*" element={<Navigate to="/dashboard" replace />} />
          </Routes>

          {/* Toast Notifications */}
          <ToastContainer
            position="top-right"
            autoClose={5000}
            hideProgressBar={false}
            newestOnTop={false}
            closeOnClick
            rtl={false}
            pauseOnFocusLoss
            draggable
            pauseOnHover
            theme="light"
          />
        </div>
      </Router>
    </AuthProvider>
  );
};

export default App;
