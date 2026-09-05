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
                Id = request.Id,
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                MobileNumber = request.MobileNumber,
                EmploymentType = request.EmploymentType,
                AnnualIncome = request.AnnualIncome,
                CreditScore = request.CreditScore,
                DateOfBirth = request.DateOfBirth,
                Status = request.Status,
                CreatedDate = request.CreatedDate
            };
        }

        public static CustomerDto ToCustomerDto(this Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                MobileNumber = customer.MobileNumber,
                EmploymentType = customer.EmploymentType,
                AnnualIncome = customer.AnnualIncome,
                CreditScore = customer.CreditScore,
                DateOfBirth = customer.DateOfBirth,
                Status = customer.Status,
                CreatedDate = customer.CreatedDate
            };
        }
    }
}
