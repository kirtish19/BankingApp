using BankingApp.Data.BankingDb.Tables;

namespace BankingApp.CustomerApi.Extensions.Mappings
{
    public static class CustomerMappingExtensions
    {
        public static Customer ToCustomer(this PostUserRegisterationRequest request, Guid userId)
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
