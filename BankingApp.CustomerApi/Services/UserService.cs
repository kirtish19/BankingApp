namespace BankingApp.CustomerApi.Services
{
    public class UserService(ICustomerRepository customerRepository, IUserRepository userRepository) : IUserService
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly IUserRepository userRepository = userRepository;

        public Task RegisterUserAsync(PostUserRegisterationRequest request)
        {
            //TODO - call customer repo only when user type is customer but user repo is always called.
            throw new NotImplementedException();
        }
    }
}
