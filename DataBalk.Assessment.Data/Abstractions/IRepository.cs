using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace DataBalk.Assessment.Data.Abstractions
{
    public interface IRepository
    {
        Task<IEnumerable<TEntity>> Get<TEntity>(Expression<Func<TEntity, bool>>? filter,   
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
            where TEntity : class;

        Task<TEntity> Insert<TEntity>(TEntity entity) where TEntity : class, IEntity;

        Task<TEntity> GetById<TEntity>(long id,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null) where TEntity : class, IEntity;

        Task<TEntity> Update<TEntity>(TEntity entity) where TEntity : class, IEntity;

        Task Delete<TEntity>(TEntity entity) where TEntity : class;        

    }
}
