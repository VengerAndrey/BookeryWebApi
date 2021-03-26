using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public interface IDataRepository
    {
        public Task<IEnumerable<Container>> ListContainers();
        public Task<Container> AddContainer(Container container);
        public Task<IEnumerable<Container>> DeleteContainers();
        public Task<IEnumerable<BlobDto>> ListBlobs(Guid idContainer);
        public Task<BlobDto> AddBlob(BlobDto blobDto);
        public Task<IEnumerable<BlobDto>> DeleteBlobs(Guid idContainer);
    }
}
