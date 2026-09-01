using BankingApp.Shared.Constants.Enums;

namespace BankingApp.LoanApi.Models
{
    public class PostLoanApplicationRequest
    {
        public Guid CustomerId { get; set; }
        public LoanType LoanType { get; set; }
        public decimal LoanAmount { get; set; }
        public int TenureMonths { get; set; }
        public IFormFile SalarySlip { get; set; } = null!;
        public IFormFile BankStatement { get; set; } = null!;
        public IFormFile EmploymentLetter { get; set; } = null!;
    }
}