using BankingApp.Data;
using BankingApp.Data.Tables;

namespace BankingApp.CustomerApi.Repository
{
    public class CustomerRepository(BankingDbContext dbContext) : ICustomerRepository
    {
        private readonly BankingDbContext _dbContext = dbContext;

        public async Task AddAsync(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
        }
    }
}