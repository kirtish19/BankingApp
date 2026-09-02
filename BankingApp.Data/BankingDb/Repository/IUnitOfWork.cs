namespace BankingApp.Data.BankingDb.Repository
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        ITransactionManager TransactionManager { get; }
        ILoanApplicationRepository LoanApplicationRepository { get; }
    }
}
