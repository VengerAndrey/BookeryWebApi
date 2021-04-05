using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookeryWebApi.Dtos;

namespace BookeryWebApi.Repositories
{
    public interface IContainerRepository
    {
        Task<IEnumerable<ContainerDto>> ListContainersAsync();
        Task<ContainerDto> AddContainerAsync(ContainerDto containerDto);
        Task<IEnumerable<ContainerDto>> DeleteContainersAsync();
        Task<ContainerDto> ListContainerAsync(Guid idContainer);
        Task<ContainerDto> DeleteContainerAsync(Guid idContainer);
    }
}
