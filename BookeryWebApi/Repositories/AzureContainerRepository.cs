using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BookeryWebApi.Dtos;

namespace BookeryWebApi.Repositories
{
    public class AzureContainerRepository : IContainerRepository
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureContainerRepository(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<IEnumerable<ContainerDto>> ListContainersAsync()
        {
            var containers = new List<ContainerDto>();
            await foreach (var page in _blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata).AsPages())
            {
                foreach (var blobContainerItem in page.Values)
                {
                    containers.Add(new ContainerDto
                    {
                        Id = Guid.Parse(blobContainerItem.Name),
                        Name = blobContainerItem.Properties.Metadata["name"]
                    });
                }
            }
            return containers;
        }

        public async Task<ContainerDto> AddContainerAsync(ContainerDto containerDto)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerDto.Id.ToString());

            await _blobServiceClient.CreateBlobContainerAsync(containerDto.Id.ToString());
            await blobContainerClient.SetMetadataAsync(new Dictionary<string, string> { { "name", containerDto.Name } });

            return containerDto;
        }

        public async Task<IEnumerable<ContainerDto>> DeleteContainersAsync()
        {
            var toDelete = await ListContainersAsync();
            var deleted = new List<ContainerDto>();

            foreach (var container in toDelete)
            {
                await _blobServiceClient.DeleteBlobContainerAsync(container.Id.ToString());
                deleted.Add(container);
            }

            return deleted;
        }

        public async Task<ContainerDto> ListContainerAsync(Guid idContainer)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (await blobContainerClient.ExistsAsync())
            {
                var container = new ContainerDto
                {
                    Id = idContainer,
                    Name = (await blobContainerClient.GetPropertiesAsync()).Value.Metadata["name"]
                };
                return container;
            }

            return null;
        }

        public async Task<ContainerDto> DeleteContainerAsync(Guid idContainer)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (await blobContainerClient.ExistsAsync())
            {
                var container = new ContainerDto
                {
                    Id = idContainer,
                    Name = (await blobContainerClient.GetPropertiesAsync()).Value.Metadata["name"]
                };

                await _blobServiceClient.DeleteBlobContainerAsync(idContainer.ToString());

                return container;
            }

            return null;
        }
    }
}
