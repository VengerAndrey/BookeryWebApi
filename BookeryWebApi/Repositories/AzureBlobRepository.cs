using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BookeryWebApi.Common;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public class AzureBlobRepository : IBlobRepository
    {
        private readonly BlobServiceClient _blobServiceClient;
        private const string RootContainer = "blobs";

        public AzureBlobRepository(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<BlobDto> AddBlobAsync(Blob blob)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(RootContainer);

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobClient = blobContainerClient.GetBlobClient(blob.Id.ToString());

            var blobMetadata = new Dictionary<string, string>
            {
                {MetadataKeys.Name, blob.Name},
                {MetadataKeys.IdContainer, blob.IdContainer.ToString()}
            };
            var blobContent = new MemoryStream(Convert.FromBase64String(blob.ContentBase64));

            await blobClient.UploadAsync(blobContent, metadata: blobMetadata);

            return new BlobDto { Id = blob.Id, Name = blob.Name, IdContainer = blob.IdContainer };
        }

        public async Task<Blob> GetBlobAsync(Guid idBlob)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(RootContainer);

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

            var metadata = (await blobClient.GetPropertiesAsync()).Value.Metadata;

            return new Blob
            {
                Id = idBlob,
                Name = metadata[MetadataKeys.Name],
                IdContainer = Guid.Parse(metadata[MetadataKeys.IdContainer]),
                ContentBase64 = Convert.ToBase64String(content)
            };
        }

        public async Task<BlobDto> PutBlobAsync(Guid idBlob, BlobUploadDto blobUploadDto)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(RootContainer);

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobClient = blobContainerClient.GetBlobClient(idBlob.ToString());

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var metadata = (await blobClient.GetPropertiesAsync()).Value.Metadata;

            var blobMetadata = new Dictionary<string, string>
            {
                {MetadataKeys.Name, blobUploadDto.Name},
                {MetadataKeys.IdContainer, metadata[MetadataKeys.IdContainer]}
            };
            var blobContent = new MemoryStream(Convert.FromBase64String(blobUploadDto.ContentBase64));

            await blobClient.UploadAsync(blobContent, metadata: blobMetadata);

            return new BlobDto
            {
                Id = idBlob,
                Name = blobUploadDto.Name,
                IdContainer = Guid.Parse(metadata[MetadataKeys.IdContainer])
            };
        }

        public async Task<BlobDto> DeleteBlobAsync(Guid idBlob)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(RootContainer);

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobClient = blobContainerClient.GetBlobClient(idBlob.ToString());

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var metadata = (await blobClient.GetPropertiesAsync()).Value.Metadata;

            var blobDto = new BlobDto
            {
                Id = idBlob,
                Name = metadata[MetadataKeys.Name],
                IdContainer = Guid.Parse(metadata[MetadataKeys.IdContainer])
            };

            await blobClient.DeleteAsync();

            return blobDto;
        }

        public async Task<IEnumerable<BlobDto>> DeleteBlobsAsync()
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(RootContainer);

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var toDelete = new List<BlobDto>();
            await foreach (var page in blobContainerClient.GetBlobsAsync(BlobTraits.Metadata).AsPages())
            {
                foreach (var blobItem in page.Values)
                {
                    toDelete.Add(new BlobDto
                    {
                        Id = Guid.Parse(blobItem.Name),
                        Name = blobItem.Metadata[MetadataKeys.Name],
                        IdContainer = Guid.Parse(blobItem.Metadata[MetadataKeys.IdContainer])
                    });
                }
            }

            var deleted = new List<BlobDto>();

            foreach (var blobDto in toDelete)
            {
                await blobContainerClient.DeleteBlobAsync(blobDto.Id.ToString());
                deleted.Add(blobDto);
            }

            return deleted;
        }
    }
}
