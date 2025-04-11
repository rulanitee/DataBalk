using DataBalk.Assessment.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace DataBalk.Assessment.Data
{
    public class Repository(DataBalkAssessmentContext context) : IRepository
    {

        private readonly DataBalkAssessmentContext _context = context;

        public async Task<IEnumerable<TEntity>> Get<TEntity>(Expression<Func<TEntity, bool>>? filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null) where TEntity : class
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (include != null) query = include(query);

            if (filter != null) query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task<TEntity> GetById<TEntity>(long id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include) where TEntity : class, IEntity
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (include != null) query = include(query);

            query = query.Where(x => x.Id == id);

            TEntity? entity = await query.FirstOrDefaultAsync();

            if (entity != null)
            {
                return entity;
            }

            throw new Exception("Entity not found");
        }

        public async Task<TEntity> Insert<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            var entities = _context.Set<TEntity>();
            entities.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> Update<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            var entities = _context.Set<TEntity>();
            entities.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete<TEntity>(TEntity entity) where TEntity : class
        {
            var entities = _context.Set<TEntity>();
            entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

    }
}
