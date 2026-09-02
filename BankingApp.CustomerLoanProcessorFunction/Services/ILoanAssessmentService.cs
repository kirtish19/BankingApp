namespace BankingApp.CustomerLoanProcessorFunction.Services
{
    public interface ILoanAssessmentService
    {
        public Task ProcessLoanApplication(LoanApplicationMessage message);
    }
}
