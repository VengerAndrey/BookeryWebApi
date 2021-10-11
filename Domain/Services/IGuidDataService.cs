using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IGuidDataService<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(Guid id);
        Task<T> Create(T entity);
        Task<T> Update(T entity);
        Task<bool> Delete(Guid id);
    }
}