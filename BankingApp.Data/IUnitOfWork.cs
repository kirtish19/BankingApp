using BankingApp.Data.BankingDb.Repository;
using BankingApp.Data.DocumentDb.Repository;

namespace BankingApp.Data
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        ITransactionManager TransactionManager { get; }
        ILoanApplicationRepository LoanApplicationRepository { get; }
        IKycDocumentsRepository KycDocumentsRepository { get; }
        ILoanDocumentRepository LoanDocumentRepository { get; }
    }
}
