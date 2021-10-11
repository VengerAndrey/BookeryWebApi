using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;

namespace WebApi.Services.Database
{
    public interface IUserNodeService
    {
        Task<IEnumerable<UserNode>> GetAll();
        Task<UserNode> Create(UserNode entity);
        Task<UserNode> Update(UserNode entity);
        Task<bool> Delete(UserNode entity);
    }
}
