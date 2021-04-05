using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookeryWebApi.Entities;

namespace BookeryWebApi.Repositories
{
    public interface IDataRepository
    {
        public Task<IEnumerable<ContainerEntity>> ListContainersAsync();
        public Task<ContainerEntity> AddContainerAsync(ContainerEntity containerEntity);
        public Task<IEnumerable<ContainerEntity>> DeleteContainersAsync();
        public Task<ContainerEntity> ListContainerAsync(Guid idContainer);
        public Task<IEnumerable<BlobEntity>> ListBlobsAsync(Guid idContainer);
        public Task<BlobEntity> AddBlobAsync(BlobEntity blobEntity);
        public Task<IEnumerable<BlobEntity>> DeleteBlobsAsync(Guid idContainer);
        public Task<ContainerEntity> DeleteContainerAsync(Guid idContainer);
        public Task<BlobEntity> ListBlobAsync(Guid idBlob);
        public Task<BlobEntity> PutBlobAsync(BlobEntity blobEntity);
        public Task<BlobEntity> DeleteBlobAsync(Guid idBlob);
    }
}
