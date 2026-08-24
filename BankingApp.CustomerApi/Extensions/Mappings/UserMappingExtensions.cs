using BankingApp.Data.Tables;

namespace BankingApp.CustomerApi.Extensions.Mappings
{
    public static class UserMappingExtensions
    {
        public static User ToUser(this PostUserRegisterationRequest request)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                UserName = request.Email,
                IsActive = false,
                CreateDate = DateTime.UtcNow,
                UserType = request.UserType
            };
        }
    }
}
