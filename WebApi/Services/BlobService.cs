using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Domain.Services;

namespace WebApi.Services
{
    public class BlobService : IBlobService
    {
        private const string RootContainer = "blobs";
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<bool> Upload(int id, Stream content)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(RootContainer);

            if (!await blobContainerClient.ExistsAsync())
            {
                return false;
            }

            var blobClient = blobContainerClient.GetBlobClient(id.ToString());

            await blobClient.UploadAsync(content);

            return true;
        }

        public async Task<Stream> Download(int id)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(RootContainer);

            if (!await blobContainerClient.ExistsAsync())
            {
                return null;
            }

            var blobClient = blobContainerClient.GetBlobClient(id.ToString());

            var stream = new MemoryStream();

            await blobClient.DownloadToAsync(stream);

            stream.Position = 0;

            return stream;
        }
    }
}
