namespace BankingApp.CustomerApi.Models
{
    public class CustomerDto
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
    }
}
