using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        ValueTask<TEntity> GetById(Guid uid);

        Task<IEnumerable<TEntity>> GetAll();

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        Task<bool> Any(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task Add(TEntity entity);

        Task AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);

        Task<int> Commit();

        Task<int> CommitWithoutLogAsync();

        void SetEntityState(TEntity entity, EntityState state);
        
        void SetEntityState<T>(T entity, EntityState state);

        void SetEntityCurrentValues(TEntity entity, TEntity state);

        void Update(TEntity entity, Expression<Func<TEntity, object>> primaryKeyExpression = null);

        Task Delete(Expression<Func<TEntity, bool>> predicate);

    }
}
