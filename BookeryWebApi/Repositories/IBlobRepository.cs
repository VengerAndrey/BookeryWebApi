using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public interface IBlobRepository
    {
        Task<IEnumerable<Container>> ListContainersAsync();
        Task<Container> AddContainerAsync(ContainerCreateDto containerCreateDto);
        Task<IEnumerable<Container>> DeleteContainersAsync();
        Task<Container> ListContainerAsync(Guid idContainer);
        Task<Container> DeleteContainerAsync(Guid idContainer);
        Task<IEnumerable<BlobDto>> ListBlobsAsync(Guid idContainer);
        Task<Blob> GetBlobAsync(BlobDownloadDto blobDownloadDto);
        Task<IEnumerable<Blob>> GetBlobsAsync(Guid idContainer);
        Task<BlobDto> AddBlobAsync(BlobCreateDto blobCreateDto);
    }
}
