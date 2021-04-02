using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public interface IContainerRepository
    {
        Task<IEnumerable<Container>> ListContainersAsync();
        Task<Container> AddContainerAsync(Container container);
        Task<IEnumerable<Container>> DeleteContainersAsync();
        Task<Container> ListContainerAsync(Guid idContainer);
        Task<Container> DeleteContainerAsync(Guid idContainer);
    }
}
