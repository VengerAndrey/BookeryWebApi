using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookeryWebApi.Common;
using BookeryWebApi.Entities;
using BookeryWebApi.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Container>> ListContainers()
        {
            return await Task.Run(() =>
            {
                var containers = new List<Container>();

                foreach (var containerEntity in _context.Containers)
                {
                    containers.Add(new Container
                    {
                        Id = containerEntity.Id,
                        Name = containerEntity.Name
                    });
                }

                return containers;
            });
        }

        public async Task<Container> AddContainer(Container container)
        {
            var containerEntity = new ContainerEntity
            {
                Id = container.Id,
                Name = container.Name
            };

            await _context.Containers.AddAsync(containerEntity);
            lock (_lock)
            {
                _context.SaveChanges();
            }

            return container;
        }

        public async Task<IEnumerable<Container>> DeleteContainers()
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
                        Name = containerEntity.Name

                    });
                }
                lock (_lock)
                {
                    _context.SaveChanges();
                }

                return deleted;
            });
        }

        public async Task<IEnumerable<BlobDto>> ListBlobs(Guid idContainer)
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

        public async Task<BlobDto> AddBlob(BlobDto blobDto)
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

        public async Task<IEnumerable<BlobDto>> DeleteBlobs(Guid idContainer)
        {
            return await Task.Run(() =>
            {
                var deleted = new List<BlobDto>();

                foreach (var blobEntity in _context.Blobs)
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
    }
}
