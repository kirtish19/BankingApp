using BankingApp.Data;

namespace BankingApp.CustomerApi.Repository
{
    public class UnitOfWork(
        BankingDbContext dbContext,
        IUserRepository userRepository,
        ICustomerRepository customerRepository) : IUnitOfWork
    {
        private readonly BankingDbContext _dbContext = dbContext;

        public IUserRepository Users { get; } = userRepository;

        public ICustomerRepository Customers { get; } = customerRepository;

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
