using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Services
{
    public interface INodeService : IDataService<Node>
    {
        Task<Node> GetWithSub(int id);
    }
}
