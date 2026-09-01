namespace BankingApp.CustomerApi.Services
{
    public interface IUserService
    {
        public Task RegisterUserAsync(PostUserRegisterationRequest request);
        public Task<bool> LoginUserAsync(PostLoginRequest request);
    }
}
