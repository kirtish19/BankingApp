namespace BankingApp.CustomerApi.Extensions.Mappings
{
    public static class UserMappingExtensions
    {
        public static UserDto ToUserDto(this PostUserRegisterationRequest request)
        {
            var hmac = new HMACSHA512();
            var user = new UserDto
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                IsActive = request.UserType == UserType.Staff ? true : false,
                CreateDate = DateTime.UtcNow,
                UserType = request.UserType,
                LoginPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.LoginPassword)),
                LoginPasswordSalt = hmac.Key
            };
            hmac = new HMACSHA512();
            user.ProfilePasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.ProfilePassword));
            user.ProfilePasswordSalt = hmac.Key;

            return user;
        }

        public static User ToUser(this UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                UserName = userDto.UserName,
                IsActive = userDto.IsActive,
                CreateDate = userDto.CreateDate,
                UserType = userDto.UserType,
                LoginPasswordHash = userDto.LoginPasswordHash,
                LoginPasswordSalt = userDto.LoginPasswordSalt,
                ProfilePasswordHash = userDto.ProfilePasswordHash,
                ProfilePasswordSalt = userDto.ProfilePasswordSalt
            };
        }
    }
}