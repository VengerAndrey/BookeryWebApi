using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Services
{
    public interface IShareService
    {
        Task<IEnumerable<Share>> GetAll();
        Task<Share> Get(Guid id);
        Task<Share> Create(Share entity);
        Task<Share> Update(Share entity);
        Task<bool> Delete(Guid id);
    }
}
