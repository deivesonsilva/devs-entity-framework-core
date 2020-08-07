using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
//[us-rep]

namespace //[ns-rep]
{
    public interface IRepositoryBase<TEntity> where TEntity : Entity
    {
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, bool asNoTracking = true);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includes);
        Task<List<TEntity>> GetAllAsync();
        Task<List<TEntity>> GetAllAsync(int take);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> where, bool asNoTracking = true);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> where, bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includes);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> orderBy, bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includes);
        Task CreateAsync(TEntity entity);
        Task CreateAsync(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Update(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void Remove(Func<TEntity, bool> where);
        void Remove(IEnumerable<TEntity> entities);
    }
}