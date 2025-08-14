using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WTB.API.Data.Context;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Implementación genérica del repositorio usando Entity Framework Core
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly WTBDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(WTBDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        #region Operaciones de Lectura

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.CountAsync();
            
            return await _dbSet.CountAsync(predicate);
        }

        #endregion

        #region Operaciones de Escritura

        public virtual async Task<T> AddAsync(T entity)
        {
            var entry = await _dbSet.AddAsync(entity);
            return entry.Entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            var entry = _dbSet.Update(entity);
            return await Task.FromResult(entry.Entity);
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await Task.CompletedTask;
        }

        #endregion

        #region Operaciones de Consulta con Paginación

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool ascending = true)
        {
            var query = _dbSet.AsQueryable();

            // Aplicar filtro si existe
            if (predicate != null)
                query = query.Where(predicate);

            // Obtener el total antes de paginar
            var totalCount = await query.CountAsync();

            // Aplicar ordenamiento si existe
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Aplicar paginación
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        #endregion

        #region Operaciones de Consulta con Includes

        public virtual async Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public virtual async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        #endregion

        #region Operaciones de Persistencia

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #endregion

        #region Métodos de Utilidad

        /// <summary>
        /// Obtiene el DbSet para consultas más complejas
        /// </summary>
        protected IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        /// <summary>
        /// Aplica includes a una consulta existente
        /// </summary>
        protected IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        #endregion
    }
}
