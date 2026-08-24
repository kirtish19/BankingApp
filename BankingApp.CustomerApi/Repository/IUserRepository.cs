using BankingApp.Data.Tables;

namespace BankingApp.CustomerApi.Repository
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
    }
}
