namespace BankingApp.Data.BankingDb.Repository
{
    public interface ITransactionManager
    {
        Task BeginTransactionAsync();

        Task<int> SaveChangesAsync();

        Task CommitAsync();

        Task RollbackAsync();
    }
}