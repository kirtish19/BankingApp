namespace BankingApp.LoanApi.Extensions.Mappings
{
    public static class LoanApplicationMappingExtensions
    {
        public static LoanApplicationsDto ToLoanApplicationsDto(this PostLoanApplicationRequest request)
        {
            return new LoanApplicationsDto
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                LoanType = request.LoanType,
                LoanAmount = request.LoanAmount,
                TenureMonths = request.TenureMonths,
                Status = LoanStatus.Submitted,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
        }

        public static LoanApplications ToLoanApplication(this LoanApplicationsDto request)
        {
            return new LoanApplications
            {
                Id = request.Id,
                CustomerId = request.CustomerId,
                LoanType = request.LoanType,
                LoanAmount = request.LoanAmount,
                TenureMonths = request.TenureMonths,
                Status = request.Status,
                CreatedDate = request.CreatedDate,
                UpdatedDate = request.UpdatedDate
            };
        }

        public static LoanApplicationsDto ToLoanApplicationDto(this LoanApplications request)
        {
            return new LoanApplicationsDto
            {
                Id = request.Id,
                CustomerId = request.CustomerId,
                LoanType = request.LoanType,
                LoanAmount = request.LoanAmount,
                TenureMonths = request.TenureMonths,
                Status = request.Status,
                CreatedDate = request.CreatedDate,
                UpdatedDate = request.UpdatedDate,
                InterestRate = request.InterestRate,
                MonthlyEMI = request.MonthlyEMI,
                ReviewComments = request.ReviewComments,
                RiskAssesmentScore = request.RiskAssesmentScore,
            };
        }

        public static IEnumerable<LoanApplicationsDto> ToLoanApplicationDtoList(this IEnumerable<LoanApplications> loanApplications)
        {
            return loanApplications.Select(x => x.ToLoanApplicationDto());
        }
    }
}
