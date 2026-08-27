using BankingApp.Data.BankingDb.Tables;

namespace BankingApp.CustomerApi.Repository
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
    }
}
