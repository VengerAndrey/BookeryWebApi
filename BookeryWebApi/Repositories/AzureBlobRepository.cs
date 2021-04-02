using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public class AzureBlobRepository : IBlobRepository
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobRepository(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<IEnumerable<BlobDto>> ListBlobsAsync(Guid idContainer)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobDtos = new List<BlobDto>();

            await foreach (var page in blobContainerClient.GetBlobsAsync(BlobTraits.Metadata).AsPages())
            {
                foreach (var blobItem in page.Values)
                {
                    blobDtos.Add(new BlobDto { 
                        Id = Guid.Parse(blobItem.Name), 
                        Name = blobItem.Metadata["name"], 
                        IdContainer = idContainer
                    });
                }
            }

            return blobDtos;
        }

        public async Task<BlobDto> AddBlobAsync(Blob blob)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blob.IdContainer.ToString());

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobClient = blobContainerClient.GetBlobClient(blob.Id.ToString());

            var blobMetadata = new Dictionary<string, string> {{"name", blob.Name}};
            var blobContent = new MemoryStream(Convert.FromBase64String(blob.ContentBase64));

            await blobClient.UploadAsync(blobContent, metadata: blobMetadata);

            return new BlobDto { Id = blob.Id, Name = blob.Name, IdContainer = blob.IdContainer };
        }

        public async Task<IEnumerable<BlobDto>> DeleteBlobsAsync(Guid idContainer)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var toDelete = await ListBlobsAsync(idContainer);
            var deleted = new List<BlobDto>();

            foreach (var blobDto in toDelete)
            {
                await blobContainerClient.DeleteBlobAsync(blobDto.Id.ToString());
                deleted.Add(blobDto);
            }

            return deleted;
        }

        public async Task<BlobDto> ListBlobAsync(Guid idContainer, Guid idBlob)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobClient = blobContainerClient.GetBlobClient(idBlob.ToString());

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            return new BlobDto
            {
                Id = idBlob, 
                Name = (await blobClient.GetPropertiesAsync()).Value.Metadata["name"],
                IdContainer = idContainer
            };
        }

        public async Task<Blob> GetBlobAsync(Guid idContainer, Guid idBlob)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobClient = blobContainerClient.GetBlobClient(idBlob.ToString());

            var blobDownloadInfo = await blobClient.DownloadAsync();
            byte[] content;
            await using (var memoryStream = new MemoryStream())
            {
                await blobDownloadInfo.Value.Content.CopyToAsync(memoryStream);
                content = memoryStream.ToArray();
            }

            return new Blob
            {
                Id = idBlob,
                Name = (await blobClient.GetPropertiesAsync()).Value.Metadata["name"],
                IdContainer = idContainer,
                ContentBase64 = Convert.ToBase64String(content)
            };
        }

        public async Task<BlobDto> PutBlobAsync(Guid idContainer, Guid idBlob, BlobUploadDto blobUploadDto)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobClient = blobContainerClient.GetBlobClient(idBlob.ToString());

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var blobMetadata = new Dictionary<string, string> { { "name", blobUploadDto.Name } };
            var blobContent = new MemoryStream(Convert.FromBase64String(blobUploadDto.ContentBase64));

            await blobClient.UploadAsync(blobContent, metadata: blobMetadata);

            return new BlobDto
            {
                Id = idBlob,
                Name = blobUploadDto.Name,
                IdContainer = idContainer
            };
        }

        public async Task<BlobDto> DeleteBlobAsync(Guid idContainer, Guid idBlob)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobClient = blobContainerClient.GetBlobClient(idBlob.ToString());

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var blobDto = new BlobDto
            {
                Id = idBlob,
                Name = (await blobClient.GetPropertiesAsync()).Value.Metadata["name"],
                IdContainer = idContainer
            };

            await blobClient.DeleteAsync();

            return blobDto;
        }
    }
}
