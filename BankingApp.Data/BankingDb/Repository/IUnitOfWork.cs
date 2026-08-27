namespace BankingApp.Data.BankingDb.Repository
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ICustomerRepository Customers { get; }
        ITransactionManager TransactionManager { get; }
    }
}
