using BankingApp.CustomerApi.Extensions.Mappings;

namespace BankingApp.CustomerApi.Services
{
    public class UserService(IUnitOfWork unitOfWork) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task RegisterUserAsync(PostUserRegisterationRequest request)
        {
            //TODO - call customer repo only when user type is customer but user repo is always called.
            // upload the documents to blog storage
            // push a message to ASB topic.
            // User is always created
            var user = request.ToUser();

            await _unitOfWork.Users.AddAsync(user);

            // Customer is created only for Customer registration
            if (request.UserType == UserType.Customer)
            {
                var customer = request.ToCustomer(user.Id);
                await _unitOfWork.Customers.AddAsync(customer);
            }

            // Commit everything together
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
