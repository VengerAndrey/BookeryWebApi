using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;

namespace WebApi.Services.Photo
{
    public class PhotoService : IPhotoService
    {
        private readonly ShareDirectoryClient _directoryClient;

        public PhotoService(ShareServiceClient shareServiceClient)
        {
            var shareClient = shareServiceClient.GetShareClient("profile-photos");

            if (!shareClient.Exists())
            {
                throw new Exception("Can't connect to profile photos storage.");
            }

            _directoryClient = shareClient.GetDirectoryClient("photos");

            if (!_directoryClient.Exists())
            {
                throw new Exception("Can't connect to profile photos storage.");
            }
        }

        public async Task<Stream> Get(int userId)
        {
            await foreach (var shareFileItem in _directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (shareFileItem.Name.Substring(0, shareFileItem.Name.IndexOf(".")) == userId.ToString())
                {
                    var fileClient = _directoryClient.GetFileClient(shareFileItem.Name);
                    var response = await fileClient.DownloadAsync();
                    return response.Value.Content;
                }
            }

            return null;
        }

        public async Task<bool> Set(int userId, string extension, Stream content)
        {
            try
            {
                var fileClient = _directoryClient.GetFileClient(userId + extension);
                await fileClient.CreateAsync(content.Length);
                await fileClient.UploadAsync(content);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}