# Direcional - Frontend React

Interface web para o sistema de gerenciamento de vendas de apartamentos da Direcional.

## 🚀 Tecnologias

- **React 19.1.1** - Framework frontend
- **TypeScript** - Tipagem estática
- **Vite** - Build tool e dev server
- **Tailwind CSS** - Framework de estilos
- **React Router DOM** - Roteamento
- **Axios** - Cliente HTTP
- **React Hook Form** - Gerenciamento de formulários
- **React Toastify** - Notificações
- **Heroicons** - Ícones

## 📁 Estrutura do Projeto

```
src/
├── components/         # Componentes reutilizáveis
│   ├── Layout.tsx     # Layout principal com sidebar
│   └── ProtectedRoute.tsx # Proteção de rotas
├── contexts/          # Contextos React
│   └── AuthContext.tsx # Gerenciamento de autenticação
├── pages/             # Páginas da aplicação
│   ├── Apartamentos/  # Módulo de apartamentos
│   ├── Clientes/      # Módulo de clientes
│   ├── Vendas/        # Módulo de vendas
│   ├── Dashboard.tsx  # Dashboard principal
│   └── Login.tsx      # Página de login
├── services/          # Serviços de API
│   ├── api.ts         # Cliente Axios configurado
│   ├── authService.ts # Autenticação
│   ├── clienteService.ts # CRUD clientes
│   ├── apartamentoService.ts # CRUD apartamentos
│   └── vendaService.ts # CRUD vendas
├── types/             # Definições de tipos
│   └── common.ts      # Tipos comuns
├── App.tsx            # Componente principal com rotas
└── main.tsx           # Ponto de entrada
```

## 🛠️ Configuração e Desenvolvimento

### Pré-requisitos

- Node.js 18+
- npm ou yarn

### Instalação

```bash
# Instalar dependências
npm install

# Iniciar servidor de desenvolvimento
npm run dev
```

### Scripts Disponíveis

```bash
npm run dev          # Servidor de desenvolvimento
npm run build        # Build para produção
npm run preview      # Preview do build
npm run lint         # Verificar código
```

## 🔧 Configuração da API

Configure a URL da API no arquivo `.env`:

```env
VITE_API_BASE_URL=http://localhost:5000/api
```

## 📖 Funcionalidades

### 🏠 Dashboard
- Visão geral do sistema
- Estatísticas de vendas
- Apartamentos disponíveis
- Ações rápidas

### 👥 Gerenciamento de Clientes
- Lista com busca e filtros
- Cadastro e edição de clientes
- Validação de CPF
- Histórico de interações

### 🏢 Catálogo de Apartamentos
- Listagem com filtros por tipo e status
- Cadastro detalhado de imóveis
- Gerenciamento de disponibilidade
- Visualização de detalhes

### 💰 Controle de Vendas
- Registro de vendas
- Cálculo automático de financiamento
- Acompanhamento de status
- Relatórios detalhados

## 🎨 Interface e UX

### Design System
- **Cores**: Paleta baseada em azul (primary) com variações
- **Tipografia**: Sistema de tamanhos consistente
- **Componentes**: Cards, botões, formulários padronizados
- **Layout**: Sidebar responsiva com navegação intuitiva

### Responsividade
- Layout adaptável para desktop, tablet e mobile
- Sidebar colapsível em telas menores
- Tabelas com scroll horizontal quando necessário
- Formulários otimizados para touch

## 🔐 Autenticação

O sistema utiliza JWT (JSON Web Token) para autenticação:

- Login com email e senha
- Token armazenado no localStorage
- Renovação automática quando possível
- Logout automático ao expirar

## 📱 Navegação

### Rotas Principais

- `/dashboard` - Dashboard principal
- `/clientes` - Lista de clientes
- `/clientes/novo` - Novo cliente
- `/clientes/:id/editar` - Editar cliente
- `/apartamentos` - Catálogo de apartamentos
- `/apartamentos/novo` - Novo apartamento
- `/apartamentos/:id/detalhes` - Detalhes do apartamento
- `/vendas` - Lista de vendas
- `/vendas/nova` - Nova venda
- `/vendas/:id/detalhes` - Detalhes da venda

### Proteção de Rotas

Todas as rotas são protegidas e requerem autenticação, exceto:
- `/login` - Página de login

## 🧪 Qualidade de Código

### Padrões Adotados

- **TypeScript** - Tipagem forte em todos os componentes
- **React Hook Form** - Validação de formulários
- **Axios Interceptors** - Tratamento global de erros
- **Error Boundaries** - Captura de erros em componentes
- **Loading States** - Feedback visual durante carregamento

### Tratamento de Erros

- Notificações toast para feedback ao usuário
- Validação de formulários em tempo real
- Tratamento de erros de API centralizado
- Fallbacks para estados de erro

## 🚀 Deploy e Produção

### Build para Produção

```bash
npm run build
```

O build gera os arquivos otimizados na pasta `dist/`.

### Variáveis de Ambiente

```env
VITE_API_BASE_URL=https://api.direcional.com.br/api
VITE_APP_TITLE=Direcional - Sistema de Vendas
```

## 🤝 Integração com Backend

Esta aplicação consome a API .NET 9 desenvolvida paralelamente:

- **Base URL**: Configurável via variável de ambiente
- **Autenticação**: JWT Bearer Token
- **Formato**: JSON REST API
- **CORS**: Configurado para permitir requisições

### Endpoints Utilizados

- `POST /auth/login` - Autenticação
- `GET|POST|PUT|DELETE /clientes` - CRUD clientes
- `GET|POST|PUT|DELETE /apartamentos` - CRUD apartamentos  
- `GET|POST|PUT|DELETE /vendas` - CRUD vendas

## 📝 Notas de Desenvolvimento

### Estado da Aplicação

- ✅ Estrutura base implementada
- ✅ Autenticação funcional
- ✅ CRUD completo para todas entidades
- ✅ Interface responsiva
- ✅ Validação de formulários
- ✅ Navegação entre módulos
- ✅ Notificações de feedback

### Próximos Passos

- [ ] Implementar detalhes específicos do cliente
- [ ] Adicionar relatórios e dashboards avançados  
- [ ] Implementar upload de documentos
- [ ] Adicionar filtros avançados
- [ ] Implementar exportação de dados

---

💡 **Dica**: Este frontend foi desenvolvido para integrar perfeitamente com a API .NET 9 do projeto Direcional. Certifique-se de que ambos estejam executando para funcionalidade completa.
