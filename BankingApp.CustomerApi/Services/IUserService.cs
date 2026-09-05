namespace BankingApp.CustomerApi.Services
{
    public interface IUserService
    {
        public Task RegisterUserAsync(PostUserRegisterationRequest request);
        public Task<LoginResponseDto> LoginUserAsync(PostLoginRequest request);
        public Task<string> GetTokenAsync(User user);
    }
}
