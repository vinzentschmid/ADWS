using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repository
{
    public class EntityRepository<TEntity>(TimeScaleContext dbContext) : IEntityRepository<TEntity>
        where TEntity : Entity
    {
        private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<List<TEntity>> CreateAsync(List<TEntity> entity)
        {
            await _dbSet.AddRangeAsync(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }


        public async Task Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return (await _dbSet.FirstOrDefaultAsync(predicate) ?? null) ?? throw new InvalidOperationException();

        }

        public async Task<List<TEntity>> Get(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            _dbSet.Update(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

    }
}
