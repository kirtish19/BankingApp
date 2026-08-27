using System.Linq.Expressions;

namespace BankingApp.Data.BankingDb.Repository
{
    public class EntityRepository<T>(BankingDbContext dbContext) : IEntityRepository<T> where T : class
    {
        protected readonly BankingDbContext _dbContext = dbContext;
        protected readonly DbSet<T> _dbSet = dbContext.Set<T>();

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate)
        {
            return await _dbSet
                .Where(predicate)
                .ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
