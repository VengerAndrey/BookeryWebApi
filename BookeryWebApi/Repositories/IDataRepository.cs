using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public interface IDataRepository
    {
        Task<IEnumerable<Container>> ListContainersAsync();
        Task AddContainerAsync(Container container);
        Task AddContainersAsync(List<Container> containers);
    }
}
