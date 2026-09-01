using BankingApp.LoanApi.Models;

namespace BankingApp.LoanApi.Services
{
    public class LoanService : ILoanService
    {
        public Task LoanApplicationSubmitAsync(PostLoanApplicationRequest request)
        {
            //check customer exists or not, throw error if not exists
            //store loan details in sql db
            //upload loan documents to blob storage
            //send a message to loan queue for further processing by azure function
            throw new NotImplementedException();
        }
    }
}
