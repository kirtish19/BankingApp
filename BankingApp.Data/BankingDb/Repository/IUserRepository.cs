using BankingApp.Data.BankingDb.Tables;

namespace BankingApp.Data.BankingDb.Repository
{
    public interface IUserRepository
    {
        Task AddAsync(User user);

        Task<User> GetUserByCustomerId(Guid id);
    }
}
