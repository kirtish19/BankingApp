using BankingApp.Data.Tables;

namespace BankingApp.CustomerApi.Repository
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
    }
}
