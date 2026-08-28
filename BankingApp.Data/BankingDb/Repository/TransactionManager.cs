namespace BankingApp.Data.BankingDb.Repository
{
    public class TransactionManager(BankingDbContext dbContext) : ITransactionManager
    {
        private readonly BankingDbContext _dbContext = dbContext;

        private IDbContextTransaction? _transaction;

        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException(
                    "Transaction has not been started.");
            }

            await _transaction.CommitAsync();

            await _transaction.DisposeAsync();

            _transaction = null;
        }

        public async Task RollbackAsync()
        {
            if (_transaction is null)
            {
                return;
            }

            await _transaction.RollbackAsync();

            await _transaction.DisposeAsync();

            _transaction = null;
        }
    }
}