using BankingApp.Data;
using BankingApp.Data.BankingDb.Tables;

namespace BankingApp.CustomerApi.Repository
{
    public class UnitOfWork(
     IEntityRepository<User> userRepository,
     IEntityRepository<Customer> customerRepository,
     ITransactionManager transactionManager)
     : IUnitOfWork
    {
        public IEntityRepository<User> Users { get; } = userRepository;
        public IEntityRepository<Customer> Customers { get; } = customerRepository;
        public ITransactionManager TransactionManager { get; } = transactionManager;
    }
}
