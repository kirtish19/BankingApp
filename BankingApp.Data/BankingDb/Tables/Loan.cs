using BankingApp.Shared.Constants.Enums;

namespace BankingApp.Data.BankingDb.Tables
{
    public class Loan
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public LoanStatus Status { get; set; }
        public int TenureMonths { get; set; }
        public LoanType LoanType { get; set; }
        public decimal OutstandingAmount { get; set; }
        public Account Account { get; set; } = null!;
    }
}
