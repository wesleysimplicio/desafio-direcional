-- Inserir usuário padrão para testes (senha: admin123)
-- Hash BCrypt para "admin123": $2a$11$5HhGzrBGL7c8kv3LJ8ZVMe4B9X.IpTbO4EohOUr0zCRptCxKYGJOm

INSERT INTO users (Username, Email, PasswordHash, Role, CreatedAt, IsActive) VALUES
('admin', 'admin@direcional.com', '$2a$11$5HhGzrBGL7c8kv3LJ8ZVMe4B9X.IpTbO4EohOUr0zCRptCxKYGJOm', 'Admin', GETDATE(), 1),
('user', 'user@direcional.com', '$2a$11$5HhGzrBGL7c8kv3LJ8ZVMe4B9X.IpTbO4EohOUr0zCRptCxKYGJOm', 'User', GETDATE(), 1);

PRINT 'Usuários padrão criados:';
PRINT '- admin/admin123 (Role: Admin)';
PRINT '- user/admin123 (Role: User)';
