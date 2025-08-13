using Microsoft.EntityFrameworkCore.Storage;
using WTB.API.Data.Context;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Implementación del patrón Unit of Work que coordina múltiples repositorios
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WTBDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction? _currentTransaction;
        private bool _disposed = false;

        public UnitOfWork(WTBDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Obtiene un repositorio genérico para una entidad específica
        /// </summary>
        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<T>(_context);
            }

            return (IRepository<T>)_repositories[type];
        }

        /// <summary>
        /// Guarda todos los cambios pendientes en la base de datos
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log del error
                throw new InvalidOperationException("Error al guardar cambios en la base de datos", ex);
            }
        }

        /// <summary>
        /// Inicia una transacción de base de datos
        /// </summary>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("Ya existe una transacción activa");
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync();
            return _currentTransaction;
        }

        /// <summary>
        /// Confirma la transacción actual
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_currentTransaction == null)
                {
                    throw new InvalidOperationException("No hay transacción activa para confirmar");
                }

                await _currentTransaction.CommitAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
            catch (Exception ex)
            {
                await RollbackTransactionAsync();
                throw new InvalidOperationException("Error al confirmar la transacción", ex);
            }
        }

        /// <summary>
        /// Revierte la transacción actual
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync();
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al revertir la transacción", ex);
            }
        }

        /// <summary>
        /// Ejecuta una operación dentro de una transacción
        /// </summary>
        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
        {
            using var transaction = await BeginTransactionAsync();
            try
            {
                var result = await operation();
                await CommitTransactionAsync();
                return result;
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// Ejecuta una operación dentro de una transacción (sin retorno)
        /// </summary>
        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            using var transaction = await BeginTransactionAsync();
            try
            {
                await operation();
                await CommitTransactionAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// Libera los recursos utilizados
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos utilizados
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _currentTransaction?.Dispose();
                // No liberamos _context ya que no lo creamos
                _disposed = true;
            }
        }
    }
}
