using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
//[us-rep]

namespace //[ns-rep]
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : Entity
    {
        protected readonly RepositoryContext _context;
        private readonly DbSet<TEntity> Data;

        public RepositoryBase(RepositoryContext context)
        {
            _context = context;
            Data = context.Set<TEntity>();
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, bool asNoTracking = true)
        {
            return asNoTracking
                ? await Data.AsNoTracking().SingleOrDefaultAsync(where)
                : await Data.SingleOrDefaultAsync(where);
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            return asNoTracking
                ? await includes.Aggregate(Data.AsNoTracking().Where(where), (current, expression) => current.Include(expression)).SingleOrDefaultAsync()
                : await includes.Aggregate(Data.Where(where), (current, expression) => current.Include(expression)).SingleOrDefaultAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await Data.ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsync(int take)
        {
            return await Data.Take(take).ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> where, bool asNoTracking = true)
        {
            return asNoTracking
                ? await Data.AsNoTracking().Where(where).ToListAsync()
                : await Data.Where(where).ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> where, bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            return asNoTracking
                ? await includes.Aggregate(Data.AsNoTracking().Where(where), (current, expression) => current.Include(expression)).ToListAsync()
                : await includes.Aggregate(Data.Where(where), (current, expression) => current.Include(expression)).ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> orderBy, bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            return asNoTracking
                ? await includes.Aggregate(Data.AsNoTracking().Where(where), (current, expression) => current.Include(expression)).OrderBy(orderBy).ToListAsync()
                : await includes.Aggregate(Data.Where(where), (current, expression) => current.Include(expression)).OrderBy(orderBy).ToListAsync();
        }

        public virtual async Task CreateAsync(TEntity entity) => await Data.AddAsync(entity);

        public virtual async Task CreateAsync(IEnumerable<TEntity> entities) => await Data.AddRangeAsync(entities);

        public virtual void Update(TEntity entity) => Data.Update(entity);

        public virtual void Update(IEnumerable<TEntity> entities) => Data.UpdateRange(entities);

        public virtual void Remove(TEntity entity) => Data.Remove(entity);

        public virtual void Remove(Func<TEntity, bool> where) => Data.RemoveRange(Data.ToList().Where(where));

        public virtual void Remove(IEnumerable<TEntity> entities) => Data.RemoveRange(entities);
    }
}
