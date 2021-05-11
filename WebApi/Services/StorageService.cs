using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Domain.Models;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using WebApi.Common;

namespace WebApi.Services
{
    public class StorageService : IStorageService
    {
        private readonly ShareServiceClient _shareServiceClient;
        private readonly PathBuilder _pathBuilder;

        public StorageService(ShareServiceClient shareServiceClient)
        {
            _shareServiceClient = shareServiceClient;
            _pathBuilder = new PathBuilder();
        }

        public async Task<IEnumerable<Share>> GetAllShares()
        {
            var shares = new List<Share>();

            await foreach (var sharePage in _shareServiceClient.GetSharesAsync(ShareTraits.Metadata).AsPages())
            {
                foreach (var share in sharePage.Values)
                {
                    shares.Add(new Share
                    {
                        Id = Guid.Parse(share.Name), 
                        Name = share.Properties.Metadata["Name"]
                    });
                }
            }

            return shares;
        }

        public async Task<Share> GetShare(Guid id)
        {
            var shareClient = _shareServiceClient.GetShareClient(id.ToString());

            if (await shareClient.ExistsAsync())
            {
                return new Share
                {
                    Id = id,
                    Name = (await shareClient.GetPropertiesAsync()).Value.Metadata["Name"]
                };
            }

            return null;
        }

        public async Task<Share> CreateShare(Share share)
        {
            share.Id = Guid.NewGuid();

            await _shareServiceClient.CreateShareAsync(share.Id.ToString(), new Dictionary<string, string>()
            {
                {"Name", share.Name}
            });

            var shareClient = _shareServiceClient.GetShareClient(share.Id.ToString());

            if (await shareClient.ExistsAsync())
            {
                await shareClient.CreateDirectoryAsync("root");

                var rootDirectory = shareClient.GetRootDirectoryClient();

                if (await rootDirectory.ExistsAsync())
                {
                    return share;
                }
            }

            return null;
        }

        public async Task<bool> DeleteShare(Guid id)
        {
            var response = await _shareServiceClient.DeleteShareAsync(id.ToString());

            return response.Status == (int) HttpStatusCode.Accepted;
        }

        public async Task<Share> UpdateShare(Share share)
        {
            var shareClient = _shareServiceClient.GetShareClient(share.Id.ToString());

            if (await shareClient.ExistsAsync())
            {
                await shareClient.SetMetadataAsync(new Dictionary<string, string>()
                {
                    {"Name", share.Name}
                });
            }

            return await GetShare(share.Id);
        }

        public async Task<IEnumerable<Item>> GetSubItems(string path)
        {
            _pathBuilder.ParsePath(path);

            if (_pathBuilder.IsFile())
            {
                return new List<Item>();
            }

            var directory = await GetDirectoryClient(path);

            if (directory is null)
            {
                return null;
            }

            var items = new List<Item>();

            await foreach (var shareFileItem in directory.GetFilesAndDirectoriesAsync())
            {
                _pathBuilder.ParseUri(new Uri(directory.Uri + "/" + shareFileItem.Name));
                items.Add(new Item
                {
                    Name = shareFileItem.Name,
                    IsDirectory = shareFileItem.IsDirectory,
                    Size = shareFileItem.FileSize,
                    Path = _pathBuilder.GetPath()
                });
            }

            return items;
        }

        public async Task<Item> CreateItem(Item item)
        {
            var directory = await GetDirectoryClient(item.Path);

            if (directory is null)
            {
                return null;
            }

            if (item.IsDirectory)
            {
                var response = await directory.CreateSubdirectoryAsync(item.Name);
                var createdDirectory = response.Value;
                if (await createdDirectory.ExistsAsync())
                {
                    _pathBuilder.ParseUri(createdDirectory.Uri);
                    item.Path = _pathBuilder.GetPath();

                    return item;
                }
            }
            else
            {
                var response = await directory.CreateFileAsync(item.Name, item.Size ?? 0);
                var createdFile = response.Value;
                if (await createdFile.ExistsAsync())
                {
                    _pathBuilder.ParseUri(createdFile.Uri);
                    item.Path = _pathBuilder.GetPath();

                    return item;
                }
            }

            return null;
        }

        public async Task<bool> UploadFile(string path, Stream content)
        {
            _pathBuilder.ParsePath(path);
            if (_pathBuilder.IsFile())
            {
                var fileName = _pathBuilder.GetFileName();
                var directory = await GetDirectoryClient(path);
                var file = directory.GetFileClient(fileName);

                await file.CreateAsync(content.Length);
                await file.UploadAsync(content);

                return true;
            }

            return false;
        }

        private async Task<ShareDirectoryClient> GetDirectoryClient(string path)
        {
            _pathBuilder.ParsePath(path);

            var shareClient = _shareServiceClient.GetShareClient(_pathBuilder.GetTopNode());

            if (await shareClient.ExistsAsync())
            {
                var directory = shareClient.GetDirectoryClient(_pathBuilder.GetTopNode());

                if (await directory.ExistsAsync())
                {
                    while (!_pathBuilder.IsFileLeft() && _pathBuilder.GetPath() != "")
                    {
                        directory = directory.GetSubdirectoryClient(_pathBuilder.GetTopNode());

                        if (!await directory.ExistsAsync())
                        {
                            return null;
                        }
                    }

                    return directory;
                }
            }

            return null;
        }
    }
}
