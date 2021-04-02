using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public class AzureContainerRepository : IContainerRepository
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureContainerRepository(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<IEnumerable<Container>> ListContainersAsync()
        {
            var containers = new List<Container>();
            await foreach (var page in _blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata).AsPages())
            {
                foreach (var blobContainerItem in page.Values)
                {
                    containers.Add(new Container
                    {
                        Id = Guid.Parse(blobContainerItem.Name),
                        Name = blobContainerItem.Properties.Metadata["name"]
                    });
                }
            }
            return containers;
        }

        public async Task<Container> AddContainerAsync(Container container)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(container.Id.ToString());

            await _blobServiceClient.CreateBlobContainerAsync(container.Id.ToString());
            await blobContainerClient.SetMetadataAsync(new Dictionary<string, string> { { "name", container.Name } });

            return container;
        }

        public async Task<IEnumerable<Container>> DeleteContainersAsync()
        {
            var toDelete = await ListContainersAsync();
            var deleted = new List<Container>();

            foreach (var container in toDelete)
            {
                await _blobServiceClient.DeleteBlobContainerAsync(container.Id.ToString());
                deleted.Add(container);
            }

            return deleted;
        }

        public async Task<Container> ListContainerAsync(Guid idContainer)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (await blobContainerClient.ExistsAsync())
            {
                var container = new Container
                {
                    Id = idContainer,
                    Name = (await blobContainerClient.GetPropertiesAsync()).Value.Metadata["name"]
                };
                return container;
            }

            return null;
        }

        public async Task<Container> DeleteContainerAsync(Guid idContainer)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (await blobContainerClient.ExistsAsync())
            {
                var container = new Container
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
