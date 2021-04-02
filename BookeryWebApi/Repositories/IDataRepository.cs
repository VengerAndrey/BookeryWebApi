using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public interface IDataRepository
    {
        public Task<IEnumerable<Container>> ListContainersAsync();
        public Task<Container> AddContainerAsync(Container container);
        public Task<IEnumerable<Container>> DeleteContainersAsync();
        public Task<IEnumerable<BlobDto>> ListBlobsAsync(Guid idContainer);
        public Task<BlobDto> AddBlobAsync(BlobDto blobDto);
        public Task<IEnumerable<BlobDto>> DeleteBlobsAsync(Guid idContainer);
        public Task<Container> DeleteContainerAsync(Guid idContainer);
        public Task<BlobDto> ListBlobAsync(Guid idBlob);
        public Task<BlobDto> PutBlobAsync(BlobDto blobDto);
        public Task<BlobDto> DeleteBlobAsync(Guid idBlob);
    }
}
