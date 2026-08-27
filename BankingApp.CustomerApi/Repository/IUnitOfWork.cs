using BankingApp.Data.BankingDb.Tables;

namespace BankingApp.CustomerApi.Repository
{
    public interface IUnitOfWork
    {
        IEntityRepository<User> Users { get; }
        IEntityRepository<Customer> Customers { get; }
        ITransactionManager TransactionManager { get; }
    }
}
