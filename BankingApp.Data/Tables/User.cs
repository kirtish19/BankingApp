using BankingApp.Shared.Constants.Enums;

namespace BankingApp.Data.Tables
{
    [Index(nameof(UserName), IsUnique = true)]
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string LoginPassword { get; set; } = null!;
        public string ProfilePassword { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public UserType UserType { get; set; }
        public Customer? Customer { get; set; }
    }
}
