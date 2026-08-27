using BankingApp.Data.BankingDb.Tables;

namespace BankingApp.Data.BankingDb.Repository
{
    public class UnitOfWork(
     IUserRepository userRepository,
     ICustomerRepository customerRepository,
     ITransactionManager transactionManager)
     : IUnitOfWork
    {
        public IUserRepository Users { get; } = userRepository;
        public ICustomerRepository Customers { get; } = customerRepository;
        public ITransactionManager TransactionManager { get; } = transactionManager;
    }
}
