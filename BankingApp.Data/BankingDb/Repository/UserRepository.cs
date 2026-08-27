using BankingApp.Data.BankingDb.Tables;

namespace BankingApp.Data.BankingDb.Repository
{
    public class UserRepository(BankingDbContext dbContext) : IUserRepository
    {
        private readonly BankingDbContext _dbContext = dbContext;

        public async Task AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }

        public async Task<User> GetUserByCustomerId(Guid id)
        {
            var user = await _dbContext.Users
                .Include(x => x.Customer)
                .FirstAsync(x => x.Customer!.Id  == id);
            return user;
        }
    }
}