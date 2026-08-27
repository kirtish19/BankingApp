using BankingApp.Data.BankingDb;
using BankingApp.Data.BankingDb.Tables;

namespace BankingApp.CustomerApi.Repository
{
    public class UserRepository(BankingDbContext dbContext) : IUserRepository
    {
        private readonly BankingDbContext _dbContext = dbContext;

        public async Task AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }
    }
}