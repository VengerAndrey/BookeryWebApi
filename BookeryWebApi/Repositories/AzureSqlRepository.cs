using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookeryWebApi.Common;
using BookeryWebApi.Entities;

namespace BookeryWebApi.Repositories
{
    public class AzureSqlRepository : IDataRepository
    {
        private readonly DatabaseContext _context;
        private readonly object _lock = new object();

        public AzureSqlRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContainerEntity>> ListContainersAsync()
        {
            return await Task.Run(() => _context.Containers);
        }

        public async Task<ContainerEntity> AddContainerAsync(ContainerEntity containerEntity)
        {
            await _context.Containers.AddAsync(containerEntity);
            lock (_lock)
            {
                _context.SaveChanges();
            }

            return containerEntity;
        }

        public async Task<IEnumerable<ContainerEntity>> DeleteContainersAsync()
        {
            return await Task.Run(() =>
            {
                var deleted = new List<ContainerEntity>();

                foreach (var containerEntity in _context.Containers)
                {
                    lock (_lock)
                    {
                        _context.Remove(containerEntity);
                    }

                    deleted.Add(containerEntity);
                }
                lock (_lock)
                {
                    _context.SaveChanges();
                }

                return deleted;
            });
        }

        public async Task<ContainerEntity> ListContainerAsync(Guid idContainer)
        {
            return await Task.Run(() => _context.Containers.FirstOrDefault(x => x.Id == idContainer));
        }

        public async Task<IEnumerable<BlobEntity>> ListBlobsAsync(Guid idContainer)
        {
            return await Task.Run(() => _context.Blobs.AsParallel().Where(x => x.IdContainer == idContainer));
        }

        public async Task<BlobEntity> AddBlobAsync(BlobEntity blobEntity)
        {
            await _context.Blobs.AddAsync(blobEntity);

            lock (_lock)
            {
                _context.SaveChanges();
            }

            return blobEntity;
        }

        public async Task<IEnumerable<BlobEntity>> DeleteBlobsAsync(Guid idContainer)
        {
            return await Task.Run(() =>
            {
                var deleted = new List<BlobEntity>();

                foreach (var blobEntity in _context.Blobs.Where(x => x.IdContainer == idContainer))
                {
                    lock (_lock)
                    {
                        _context.Remove(blobEntity);
                    }

                    deleted.Add(blobEntity);
                }
                lock (_lock)
                {
                    _context.SaveChanges();
                }

                return deleted;
            });
        }

        public async Task<ContainerEntity> DeleteContainerAsync(Guid idContainer)
        {
            return await Task.Run(() =>
            {
                var containerEntity = _context.Containers.FirstOrDefault(x => x.Id == idContainer);

                if (containerEntity is null)
                    return null;

                lock (_lock)
                {
                    _context.Remove(containerEntity);
                    _context.SaveChanges();
                }

                return containerEntity;
            });
        }

        public async Task<BlobEntity> ListBlobAsync(Guid idBlob)
        {
            return await Task.Run(() => _context.Blobs.FirstOrDefault(x => x.Id == idBlob));
        }

        public async Task<BlobEntity> PutBlobAsync(BlobEntity blobEntity)
        {
            return await Task.Run(() =>
            {
                var oldBlobEntity = _context.Blobs.FirstOrDefault(x => x.Id == blobEntity.Id);

                if (oldBlobEntity is null)
                    return null;

                lock (_lock)
                {
                    _context.Blobs.Remove(oldBlobEntity);
                    _context.Blobs.Add(blobEntity);
                    _context.SaveChanges();
                }

                return blobEntity;
            });
        }

        public async Task<BlobEntity> DeleteBlobAsync(Guid idBlob)
        {
            return await Task.Run(() =>
            {
                var blobEntity = _context.Blobs.FirstOrDefault(x => x.Id == idBlob);

                if (blobEntity is null)
                    return null;

                lock (_lock)
                {
                    _context.Remove(blobEntity);
                    _context.SaveChanges();
                }

                return blobEntity;
            });
        }
    }
}
