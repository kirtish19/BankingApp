namespace BankingApp.Data.BankingDb.Repository
{
    public class UserRepository : EntityRepository<User>, IUserRepository
    {

        public UserRepository(BankingDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<User> GetUserByCustomerId(Guid id)
        {
            var user = await _dbContext.Users
                .Include(x => x.Customer)
                .FirstAsync(x => x.Customer!.Id == id);
            return user;
        }

        public async Task<User> GetUserByUserName(string username)
        {
            var user = await _dbContext.Users
                .Include(x => x.Customer)
                .FirstAsync(x => x.UserName == username);
            return user;
        }
    }
}