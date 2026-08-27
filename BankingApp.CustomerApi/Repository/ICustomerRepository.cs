using BankingApp.Data.BankingDb.Tables;

namespace BankingApp.CustomerApi.Repository
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
    }
}
