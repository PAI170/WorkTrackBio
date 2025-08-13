using Microsoft.EntityFrameworkCore.Storage;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Interfaz para el patrón Unit of Work que coordina múltiples repositorios
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Obtiene un repositorio genérico para una entidad específica
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <returns>Repositorio genérico</returns>
        IRepository<T> Repository<T>() where T : class;

        /// <summary>
        /// Guarda todos los cambios pendientes en la base de datos
        /// </summary>
        /// <returns>Número de registros afectados</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Inicia una transacción de base de datos
        /// </summary>
        /// <returns>Transacción de base de datos</returns>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        /// Confirma la transacción actual
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Revierte la transacción actual
        /// </summary>
        Task RollbackTransactionAsync();

        /// <summary>
        /// Ejecuta una operación dentro de una transacción
        /// </summary>
        /// <param name="operation">Operación a ejecutar</param>
        /// <returns>Resultado de la operación</returns>
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);

        /// <summary>
        /// Ejecuta una operación dentro de una transacción (sin retorno)
        /// </summary>
        /// <param name="operation">Operación a ejecutar</param>
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
