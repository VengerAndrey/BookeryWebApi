﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using WebApi.Common;

namespace WebApi.Services.Item
{
    public class ItemService : IItemService
    {
        private readonly PathBuilder _pathBuilder;
        private readonly ShareServiceClient _shareServiceClient;

        public ItemService(ShareServiceClient shareServiceClient)
        {
            _shareServiceClient = shareServiceClient;
            _pathBuilder = new PathBuilder();
        }

        public async Task<IEnumerable<Domain.Models.Item>> GetSubItems(string path)
        {
            _pathBuilder.ParsePath(path);

            if (_pathBuilder.IsFile()) return new List<Domain.Models.Item>();

            var directory = await GetPenultimateDirectoryClient(path);

            if (directory is null) return null;

            if (_pathBuilder.GetDepth(path) > 2)
            {
                directory = directory.GetSubdirectoryClient(_pathBuilder.GetLastNode(path));

                if (directory is null) return null;
            }

            var items = new List<Domain.Models.Item>();

            await foreach (var shareFileItem in directory.GetFilesAndDirectoriesAsync())
            {
                _pathBuilder.ParseUri(new Uri(directory.Uri + "/" + shareFileItem.Name));
                items.Add(new Domain.Models.Item
                {
                    Name = shareFileItem.Name,
                    IsDirectory = shareFileItem.IsDirectory,
                    Size = shareFileItem.FileSize,
                    Path = _pathBuilder.GetPath()
                });
            }

            return items;
        }

        public async Task<Domain.Models.Item> CreateDirectory(string path)
        {
            var directory = await GetPenultimateDirectoryClient(path);

            if (directory is null) return null;

            var name = _pathBuilder.GetLastNode(path);

            var response = await directory.CreateSubdirectoryAsync(name);
            var createdDirectory = response.Value;
            if (await createdDirectory.ExistsAsync())
            {
                _pathBuilder.ParseUri(createdDirectory.Uri);

                return new Domain.Models.Item
                {
                    Name = name,
                    IsDirectory = true,
                    Size = null,
                    Path = _pathBuilder.GetPath()
                };
            }

            return null;
        }

        public async Task<Domain.Models.Item> UploadFile(string path, string name, Stream content)
        {
            _pathBuilder.ParsePath(path);
            if (_pathBuilder.IsFile())
            {
                var directory = await GetPenultimateDirectoryClient(path);

                if (directory is null) return null;

                var file = directory.GetFileClient(name);

                await file.CreateAsync(content.Length);
                await file.UploadAsync(content);

                _pathBuilder.ParseUri(directory.Uri);
                _pathBuilder.AddNode(name);

                return new Domain.Models.Item
                {
                    Name = name,
                    IsDirectory = false,
                    Size = content.Length,
                    Path = _pathBuilder.GetPath()
                };
            }

            return null;
        }

        public async Task<Stream> DownloadFile(string path)
        {
            _pathBuilder.ParsePath(path);
            if (_pathBuilder.IsFile())
            {
                var directory = await GetPenultimateDirectoryClient(path);

                if (directory is null) return null;

                var file = directory.GetFileClient(_pathBuilder.GetLastNode(path));
                if (await file.ExistsAsync())
                {
                    var response = await file.DownloadAsync();
                    var downloadedFile = response.Value;

                    return downloadedFile.Content;
                }
            }

            return null;
        }

        private async Task<ShareDirectoryClient> GetPenultimateDirectoryClient(string path)
        {
            _pathBuilder.ParsePath(path);

            var shareClient = _shareServiceClient.GetShareClient(_pathBuilder.GetTopNode());

            if (await shareClient.ExistsAsync())
            {
                var directory = shareClient.GetDirectoryClient(_pathBuilder.GetTopNode());

                if (await directory.ExistsAsync())
                {
                    if (_pathBuilder.GetDepth(path) == 1) return directory;

                    while (!_pathBuilder.IsLastNode() && _pathBuilder.GetPath() != "")
                    {
                        directory = directory.GetSubdirectoryClient(_pathBuilder.GetTopNode());

                        if (!await directory.ExistsAsync()) return null;
                    }

                    return directory;
                }
            }

            return null;
        }
    }
}