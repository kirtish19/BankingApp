namespace BankingApp.LoanApi.Models
{
    public class LoanApplicationsDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public LoanType LoanType { get; set; }
        public decimal LoanAmount { get; set; }
        public int TenureMonths { get; set; }
        public decimal? MonthlyEMI { get; set; }
        public decimal? InterestRate { get; set; }
        public LoanStatus Status { get; set; }
        public RiskAssesment? RiskAssesmentScore { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
