using BankingApp.Data.BankingDb.Repository;
using BankingApp.Data.DocumentDb.Repository;

namespace BankingApp.Data
{
    public class UnitOfWork(IServiceProvider serviceProvider) : IUnitOfWork
    {

        private readonly IServiceProvider _serviceProvider = serviceProvider;
        ITransactionManager? _transactionManager;
        IUserRepository? _userRepository;
        ICustomerRepository? _customerRepository;
        ILoanApplicationRepository? loanApplicationRepository;
        IKycDocumentsRepository? kycDocumentsRepository;

        ILoanDocumentRepository? loanDocumentRepository;

        public IUserRepository UserRepository => _userRepository ??= (IUserRepository)_serviceProvider.GetRequiredService(typeof(IUserRepository));
        public ICustomerRepository CustomerRepository => _customerRepository ??= (ICustomerRepository)_serviceProvider.GetRequiredService(typeof(ICustomerRepository));
        public ITransactionManager TransactionManager => _transactionManager ??= (ITransactionManager)_serviceProvider.GetRequiredService(typeof(ITransactionManager));
        public ILoanApplicationRepository LoanApplicationRepository => loanApplicationRepository ??= (ILoanApplicationRepository)_serviceProvider.GetRequiredService(typeof(ILoanApplicationRepository));
        public IKycDocumentsRepository KycDocumentsRepository => kycDocumentsRepository ??= (IKycDocumentsRepository)_serviceProvider.GetRequiredService(typeof(IKycDocumentsRepository));
        public ILoanDocumentRepository LoanDocumentRepository => loanDocumentRepository ??= (ILoanDocumentRepository)_serviceProvider.GetRequiredService(typeof(ILoanDocumentRepository));
    }
}
