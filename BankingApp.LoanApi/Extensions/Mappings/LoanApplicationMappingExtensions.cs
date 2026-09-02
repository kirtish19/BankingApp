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
    }
}
