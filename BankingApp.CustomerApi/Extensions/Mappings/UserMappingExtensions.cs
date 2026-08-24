using BankingApp.Data.Tables;
using System.Security.Cryptography;
using System.Text;

namespace BankingApp.CustomerApi.Extensions.Mappings
{
    public static class UserMappingExtensions
    {
        public static User ToUser(this PostUserRegisterationRequest request)
        {
            var hmac = new HMACSHA512();
            var user = new User();

            user.Id = Guid.NewGuid();
            user.UserName = request.UserName;
            user.IsActive = request.UserType == UserType.Staff ? true : false;
            user.CreateDate = DateTime.UtcNow;
            user.UserType = request.UserType;
            user.LoginPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.LoginPassword));
            user.LoginPasswordSalt = hmac.Key;
            hmac = new HMACSHA512();
            user.ProfilePasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.ProfilePassword));
            user.ProfilePasswordSalt = hmac.Key;

            return user;
        }
    }
}