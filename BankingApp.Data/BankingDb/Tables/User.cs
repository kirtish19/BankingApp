namespace BankingApp.Data.BankingDb.Tables
{
    [Index(nameof(UserName), IsUnique = true)]
    public class User
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
        public Customer? Customer { get; set; }
    }
}
