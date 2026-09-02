namespace BankingApp.Data.BankingDb.Repository
{
    public class UnitOfWork(IServiceProvider serviceProvider) : IUnitOfWork
    {

        private readonly IServiceProvider _serviceProvider = serviceProvider;
        ITransactionManager? _transactionManager;
        IUserRepository? _userRepository;
        ICustomerRepository? _customerRepository;
        ILoanApplicationRepository? loanApplicationRepository;

        public IUserRepository UserRepository => _userRepository ??= (IUserRepository)_serviceProvider.GetRequiredService(typeof(IUserRepository));
        public ICustomerRepository CustomerRepository => _customerRepository ??= (ICustomerRepository)_serviceProvider.GetRequiredService(typeof(ICustomerRepository));
        public ITransactionManager TransactionManager => _transactionManager ??= (ITransactionManager)_serviceProvider.GetRequiredService(typeof(ITransactionManager));
        public ILoanApplicationRepository LoanApplicationRepository => loanApplicationRepository ??= (ILoanApplicationRepository)_serviceProvider.GetRequiredService(typeof(ILoanApplicationRepository));
    }
}
