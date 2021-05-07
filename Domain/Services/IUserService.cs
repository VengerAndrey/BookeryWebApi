using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Services
{
    public interface IUserService : IDataService<User>
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByUsername(string username);
    }
}
