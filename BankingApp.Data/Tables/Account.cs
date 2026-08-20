using BankingApp.Shared.Constants.Enums;

namespace BankingApp.Data.Tables
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string AccountType { get; set; } = null!; // not sure if we need this as we are only working with loan accounts.
        public AccountStatus Status { get; set; }
        public decimal Balance { get; set; }
        public DateTime OpenDate { get; set; }
        public Customer Customer { get; set; } = null!;
    }
}
