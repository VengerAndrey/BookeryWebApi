using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookeryWebApi.Common;
using BookeryWebApi.Entities;
using BookeryWebApi.Models;

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

        public async Task<IEnumerable<Container>> ListContainersAsync()
        {
            return await Task.Run(() =>
            {
                var containers = new List<Container>();

                foreach (var containerEntity in _context.Containers)
                {
                    containers.Add(new Container
                    {
                        Id = containerEntity.Id,
                        Name = containerEntity.Name,
                        OwnerLogin = containerEntity.OwnerLogin
                    });
                }

                return containers;
            });
        }

        public async Task<Container> AddContainerAsync(Container container)
        {
            var containerEntity = new ContainerEntity
            {
                Id = container.Id,
                Name = container.Name,
                OwnerLogin = container.OwnerLogin
            };

            await _context.Containers.AddAsync(containerEntity);
            lock (_lock)
            {
                _context.SaveChanges();
            }

            return container;
        }

        public async Task<IEnumerable<Container>> DeleteContainersAsync()
        {
            return await Task.Run(() =>
            {
                var deleted = new List<Container>();

                foreach (var containerEntity in _context.Containers)
                {
                    lock (_lock)
                    {
                        _context.Remove(containerEntity);
                    }

                    deleted.Add(new Container
                    {
                        Id = containerEntity.Id,
                        Name = containerEntity.Name,
                        OwnerLogin = containerEntity.OwnerLogin

                    });
                }
                lock (_lock)
                {
                    _context.SaveChanges();
                }

                return deleted;
            });
        }

        public async Task<Container> ListContainerAsync(Guid idContainer)
        {
            return await Task.Run(() =>
            {
                var containerEntity = _context.Containers.FirstOrDefault(x => x.Id == idContainer);

                if (containerEntity is null)
                    return null;

                return new Container {Id = idContainer, Name = containerEntity.Name, OwnerLogin = containerEntity.OwnerLogin};
            });
        }

        public async Task<IEnumerable<BlobDto>> ListBlobsAsync(Guid idContainer)
        {
            return await Task.Run(() =>
            {
                var blobDtos = from x in _context.Blobs.AsParallel()
                    where x.IdContainer == idContainer
                    select new BlobDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IdContainer = idContainer
                    };

                return blobDtos;
            });
        }

        public async Task<BlobDto> AddBlobAsync(BlobDto blobDto)
        {
            await _context.Blobs.AddAsync(new BlobEntity
            {
                Id = blobDto.Id,
                Name = blobDto.Name,
                IdContainer = blobDto.IdContainer
            });

            lock (_lock)
            {
                _context.SaveChanges();
            }

            return blobDto;
        }

        public async Task<IEnumerable<BlobDto>> DeleteBlobsAsync(Guid idContainer)
        {
            return await Task.Run(() =>
            {
                var deleted = new List<BlobDto>();

                foreach (var blobEntity in _context.Blobs.Where(x => x.IdContainer == idContainer))
                {
                    lock (_lock)
                    {
                        _context.Remove(blobEntity);
                    }

                    deleted.Add(new BlobDto()
                    {
                        Id = blobEntity.Id,
                        Name = blobEntity.Name,
                        IdContainer = blobEntity.IdContainer
                    });
                }
                lock (_lock)
                {
                    _context.SaveChanges();
                }

                return deleted;
            });
        }

        public async Task<Container> DeleteContainerAsync(Guid idContainer)
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

                return new Container {Id = containerEntity.Id, Name = containerEntity.Name};
            });
        }

        public async Task<BlobDto> ListBlobAsync(Guid idBlob)
        {
            return await Task.Run(() =>
            {
                var blobEntity = _context.Blobs.FirstOrDefault(x => x.Id == idBlob);

                if (blobEntity is null)
                    return null;

                return new BlobDto {Id = blobEntity.Id, Name = blobEntity.Name, IdContainer = blobEntity.IdContainer};
            });
        }

        public async Task<BlobDto> PutBlobAsync(BlobDto blobDto)
        {
            return await Task.Run(() =>
            {
                _context.Blobs.Update(new BlobEntity
                    {Id = blobDto.Id, Name = blobDto.Name, IdContainer = blobDto.IdContainer});

                lock (_lock)
                {
                    _context.SaveChanges();
                }

                return blobDto;
            });
        }

        public async Task<BlobDto> DeleteBlobAsync(Guid idBlob)
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

                return new BlobDto {Id = blobEntity.Id, Name = blobEntity.Name, IdContainer = blobEntity.IdContainer};
            });
        }
    }
}
