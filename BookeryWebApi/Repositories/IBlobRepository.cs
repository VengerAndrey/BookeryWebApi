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
        Task AddContainerAsync(Container container);
        Task<IEnumerable<BlobDto>> ListBlobsAsync(Container container);
        Task<IEnumerable<Blob>> GetBlobsAsync(Container container);
        Task AddBlobAsync(Container container, Blob blob);
    }
}
