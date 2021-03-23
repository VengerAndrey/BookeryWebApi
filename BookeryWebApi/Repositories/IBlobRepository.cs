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
        Task AddContainerAsync(ContainerCreateDto containerCreateDto);
        Task<IEnumerable<BlobDto>> ListBlobsAsync(Guid idContainer);
        Task<IEnumerable<Blob>> GetBlobsAsync(Guid idContainer);
        Task AddBlobAsync(BlobCreateDto blobCreateDto);
    }
}
