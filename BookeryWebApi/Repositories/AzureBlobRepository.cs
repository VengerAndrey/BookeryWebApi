﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public class AzureBlobRepository : IBlobRepository
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IDataRepository _dataRepository;

        public AzureBlobRepository(BlobServiceClient blobServiceClient, IDataRepository dataRepository)
        {
            _blobServiceClient = blobServiceClient;
            _dataRepository = dataRepository;
        }

        public async Task<IEnumerable<Container>> ListContainersAsync()
        {
            return await _dataRepository.ListContainersAsync();
        }

        public async Task<Container> AddContainerAsync(ContainerCreateDto containerCreateDto)
        {
            var container = new Container {Id = Guid.NewGuid(), Name = containerCreateDto.Name};

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(container.Id.ToString());

            if (await blobContainerClient.ExistsAsync())
            {
                //Container re-creation 
                return null;
            }

            await _dataRepository.AddContainerAsync(container);
            await _blobServiceClient.CreateBlobContainerAsync(container.Id.ToString());
            return container;
        }

        public async Task<IEnumerable<BlobDto>> ListBlobsAsync(Guid idContainer)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobs = new List<BlobDto>();

            await foreach (var page in blobContainerClient.GetBlobsAsync(BlobTraits.Metadata).AsPages())
            {
                foreach (var blobItem in page.Values)
                {
                    blobs.Add(new BlobDto {Id = Guid.Parse(blobItem.Name), Name = blobItem.Metadata["name"], IdContainer = idContainer});
                }
            }

            return blobs;
        }

        public async Task<Blob> GetBlobAsync(BlobDownloadDto blobDownloadDto)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobDownloadDto.IdContainer.ToString());

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobClient = blobContainerClient.GetBlobClient(blobDownloadDto.Id.ToString());

            var content = await blobClient.DownloadAsync();

            return new Blob
            {
                Id = blobDownloadDto.Id,
                Name = (await blobClient.GetPropertiesAsync()).Value.Metadata["name"],
                IdContainer = blobDownloadDto.IdContainer,
                Content = content.Value.Content
            };
        }

        public async Task<IEnumerable<Blob>> GetBlobsAsync(Guid idContainer)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(idContainer.ToString());

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var downloadedBlobs = new List<Blob>();

            var blobDtos = await ListBlobsAsync(idContainer);

            foreach (var blobDto in blobDtos)
            {
                var blobClient = blobContainerClient.GetBlobClient(blobDto.Id.ToString());
                downloadedBlobs.Add(new Blob
                {
                    Id = blobDto.Id,
                    Name = blobDto.Name,
                    IdContainer = blobDto.IdContainer,
                    Content = blobClient.DownloadAsync().Result.Value.Content
                });
            }

            return downloadedBlobs;
        }

        public async Task<BlobDto> AddBlobAsync(BlobCreateDto blobCreateDto)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobCreateDto.IdContainer.ToString());

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var id = Guid.NewGuid();

            var blobClient = blobContainerClient.GetBlobClient(id.ToString());

            var metadata = new Dictionary<string, string>();

            metadata.Add("name", blobCreateDto.Name);

            await blobClient.UploadAsync(blobCreateDto.Content, metadata: metadata);

            return new BlobDto {Id = id, Name = blobCreateDto.Name, IdContainer = blobCreateDto.IdContainer};
        }
    }
}
