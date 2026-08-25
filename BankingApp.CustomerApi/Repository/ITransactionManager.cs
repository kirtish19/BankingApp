namespace BankingApp.CustomerApi.Repository
{
    public interface ITransactionManager
    {
        Task BeginTransactionAsync();

        Task<int> SaveChangesAsync();

        Task CommitAsync();

        Task RollbackAsync();
    }
}