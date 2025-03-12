using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class HypertableRepository<T>(TimeScaleContext dbContext) : IHypertableRepository<T>
        where T : HyperEntity
    {
        private readonly DbSet<T> _dbSet = dbContext.Set<T>();

        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task CreateAsync(List<T> entity)
        {
            await _dbSet.AddRangeAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task<T> GetLastValue(int dp, DateTimeOffset from, DateTimeOffset to)
        {
            return await _dbSet
                .Where(e => e.FK_Datapoint == dp && e.Time >= from && e.Time <= to)
                .OrderByDescending(e => e.Time)
                .FirstOrDefaultAsync() ?? throw new InvalidOperationException();
            
        }


        public async Task<List<T>> GetValues(int dp, DateTimeOffset from, DateTimeOffset to)
        {
            
            return await _dbSet
                .Where(e => e.FK_Datapoint == dp && e.Time >= from && e.Time <= to)
                .OrderBy(e => e.Time)
                .ToListAsync();
        }



        public async Task<T> GetLastValue(int dp)
        {
            return await _dbSet
                .Where(e => e.FK_Datapoint == dp)
                .OrderByDescending(e => e.Time)
                .FirstOrDefaultAsync() ?? throw new InvalidOperationException();
        }

    }
}
