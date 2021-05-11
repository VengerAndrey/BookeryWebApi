using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;

namespace WebApi.Services
{
    public class FileService
    {
        private readonly ShareServiceClient _shareServiceClient;

        public FileService(ShareServiceClient shareServiceClient)
        {
            _shareServiceClient = shareServiceClient;
        }

        public async void Test()
        {
            var shareClient = _shareServiceClient.GetShareClient("test");

            var directoryClient = shareClient.GetDirectoryClient("dir1");

            if (!await directoryClient.ExistsAsync())
            {
                await shareClient.CreateDirectoryAsync("dir1");
            }

            if (await directoryClient.ExistsAsync())
            {
                var r = directoryClient.GetFilesAndDirectories();

                foreach (var shareFileItem in r)
                {
                }

                var fileClient = directoryClient.GetFileClient("test.txt");
                await using FileStream stream = File.OpenRead("D:/Blobs/1.txt");
                await fileClient.CreateAsync(stream.Length);
                await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
            }
        }
    }
}
