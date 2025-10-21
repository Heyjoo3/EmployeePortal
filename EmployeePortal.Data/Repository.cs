using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmployeePortal.Core.Data;
using EmployeePortal.Core.Repositories;

namespace EmployeePortal.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly EmployeePortalContext _context;

        public Repository(EmployeePortalContext context)
        {
            _context = context;
        }

        public async Task Add(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRange(IEnumerable<TEntity> entities)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate);
        }

        public Task<bool> Any(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().AsNoTracking().AnyAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async ValueTask<TEntity> GetById(Guid uid)
        {
            return await _context.Set<TEntity>().FindAsync(uid);
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public async Task<int> Commit()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CommitWithoutLogAsync()
        {
            return await _context.SaveChangesWithNoLogAsync();
        }

        public void SetEntityState(TEntity entity, EntityState state)
        {
            _context.Entry(entity).State = state;
        }

        public void SetEntityState<T>(T entity, EntityState state)
        {
            _context.Entry(entity).State = state;
        }

        public void SetEntityCurrentValues(TEntity targetEntity, TEntity sourceEntity)
        {
            _context.Entry(targetEntity).CurrentValues.SetValues(sourceEntity);
        }
        public void Update(TEntity entity, Expression<Func<TEntity, object>> primaryKeyExpression = null)
        {
            if (primaryKeyExpression == null)
                throw new ArgumentNullException(nameof(primaryKeyExpression), "Primary key expression cannot be null.");

            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                // Get the primary key value of the entity being updated
                var primaryKeyValue = primaryKeyExpression.Compile()(entity);

                // Detach the existing entity if it is already being tracked
                var existingEntity = _context.Set<TEntity>().Local.FirstOrDefault(e => primaryKeyExpression.Compile()(e).Equals(primaryKeyValue));
                if (existingEntity != null)
                {
                    _context.Entry(existingEntity).State = EntityState.Detached;
                }

                _context.Set<TEntity>().Attach(entity);
                entry.State = EntityState.Modified;
            }
        }

        public async Task Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = _context.Set<TEntity>().FirstOrDefault(predicate);

            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}