using BankingApp.Data.BankingDb.Tables;

namespace BankingApp.Data.BankingDb.Repository
{
    public interface IUserRepository:IEntityRepository<User>
    {
        Task<User> GetUserByCustomerId(Guid id);
    }
}
