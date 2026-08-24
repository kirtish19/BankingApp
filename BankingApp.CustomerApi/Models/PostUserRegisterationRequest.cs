namespace BankingApp.CustomerApi.Models
{
    public class PostUserRegisterationRequest
    {
        public string UserName { get; set; } = null!;
        public string LoginPassword { get; set; } = null!;
        public string ProfilePassword { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public EmploymentType EmploymentType { get; set; }
        public UserType UserType { get; set; }
        public decimal AnnualIncome { get; set; }
        public int CreditScore { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IFormFileCollection? KycDocuments { get; set; }
    }
}
