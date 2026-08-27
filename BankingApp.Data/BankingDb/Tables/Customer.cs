using BankingApp.Shared.Constants.Enums;

namespace BankingApp.Data.BankingDb.Tables
{
    //TODO How will we store Docs
    public class Customer
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public CustomerStatus Status { get; set; } = CustomerStatus.New;
        public EmploymentType EmploymentType { get; set; }
        public decimal AnnualIncome { get; set; }
        public int CreditScore { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime DateOfBirth { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Account> Accounts { get; set; } = [];
    }
}
