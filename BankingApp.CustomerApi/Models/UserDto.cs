namespace BankingApp.CustomerApi.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public byte[] LoginPasswordHash { get; set; } = null!;
        public byte[] LoginPasswordSalt { get; set; } = null!;
        public byte[] ProfilePasswordHash { get; set; } = null!;
        public byte[] ProfilePasswordSalt { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public UserType UserType { get; set; }
    }
}
