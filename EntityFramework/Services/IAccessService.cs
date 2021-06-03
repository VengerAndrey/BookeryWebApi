using System;
using System.Threading.Tasks;

namespace EntityFramework.Services
{
    public interface IAccessService
    {
        Task<bool> AccessById(string email, Guid id);
    }
}