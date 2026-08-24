namespace BankingApp.CustomerApi.Repository
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ICustomerRepository Customers { get; }
        Task<int> SaveChangesAsync();
    }
}
