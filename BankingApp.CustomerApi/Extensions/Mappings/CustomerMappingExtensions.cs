namespace BankingApp.CustomerApi.Extensions.Mappings
{
    public static class CustomerMappingExtensions
    {
        public static CustomerDto ToCustomerDto(this PostUserRegisterationRequest request)
        {
            return new CustomerDto
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName!,
                LastName = request.LastName!,
                Email = request.Email!,
                MobileNumber = request.MobileNumber!,
                EmploymentType = request.EmploymentType!.Value,
                AnnualIncome = (decimal)request.AnnualIncome!,
                CreditScore = (int)request.CreditScore!,
                DateOfBirth = (DateTime)request.DateOfBirth!,
                Status = CustomerStatus.New,
                CreatedDate = DateTime.Now
            };
        }

        public static Customer ToCustomer(this CustomerDto request, Guid userId)
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                MobileNumber = request.MobileNumber,
                EmploymentType = request.EmploymentType,
                AnnualIncome = request.AnnualIncome,
                CreditScore = request.CreditScore,
                DateOfBirth = request.DateOfBirth,
                Status = CustomerStatus.New,
                CreatedDate = DateTime.Now
            };
        }
    }
}
