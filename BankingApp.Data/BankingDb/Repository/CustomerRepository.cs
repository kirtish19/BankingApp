namespace BankingApp.Data.BankingDb.Repository
{
    public class CustomerRepository : EntityRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(BankingDbContext dbContext) : base(dbContext)
        {

        }
    }
}