using BankingApp.Shared.Constants.Enums;

namespace BankingApp.Data.BankingDb.Tables
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public AccountStatus Status { get; set; }
        public decimal Balance { get; set; }
        public DateTime OpenDate { get; set; }
        public Customer Customer { get; set; } = null!;
        public ICollection<Loan>? Loans { get; set; }
    }
}
