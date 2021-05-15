using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace WebApi.Services.Share
{
    public class AzureShareService : IAzureShareService
    {
        private readonly ShareServiceClient _shareServiceClient;

        public AzureShareService(ShareServiceClient shareServiceClient)
        {
            _shareServiceClient = shareServiceClient;
        }

        public async Task<IEnumerable<Domain.Models.Share>> GetAll()
        {
            var shares = new List<Domain.Models.Share>();

            await foreach (var sharePage in _shareServiceClient.GetSharesAsync(ShareTraits.Metadata).AsPages())
            foreach (var share in sharePage.Values)
                shares.Add(new Domain.Models.Share
                {
                    Id = Guid.Parse(share.Name),
                    Name = share.Properties.Metadata["Name"],
                    OwnerId = int.Parse(share.Properties.Metadata["OwnerId"])
                });

            return shares;
        }

        public async Task<Domain.Models.Share> Get(Guid id)
        {
            var shareClient = _shareServiceClient.GetShareClient(id.ToString());

            if (await shareClient.ExistsAsync())
            {
                var properties = (await shareClient.GetPropertiesAsync()).Value;
                return new Domain.Models.Share
                {
                    Id = id,
                    Name = properties.Metadata["Name"],
                    OwnerId = int.Parse(properties.Metadata["OwnerId"])
                };
            }

            return null;
        }

        public async Task<Domain.Models.Share> Create(Domain.Models.Share share)
        {
            await _shareServiceClient.CreateShareAsync(share.Id.ToString(), new Dictionary<string, string>
            {
                {"Name", share.Name},
                {"OwnerId", share.OwnerId.ToString()}
            });

            var shareClient = _shareServiceClient.GetShareClient(share.Id.ToString());

            if (await shareClient.ExistsAsync())
            {
                await shareClient.CreateDirectoryAsync("root");

                var rootDirectory = shareClient.GetRootDirectoryClient();

                if (await rootDirectory.ExistsAsync()) return share;
            }

            return null;
        }

        public async Task<Domain.Models.Share> Update(Guid id, Domain.Models.Share share)
        {
            var shareClient = _shareServiceClient.GetShareClient(id.ToString());

            if (await shareClient.ExistsAsync())
                await shareClient.SetMetadataAsync(new Dictionary<string, string>
                {
                    {"Name", share.Name},
                    {"OwnerId", share.OwnerId.ToString()}
                });

            return await Get(id);
        }

        public async Task<bool> Delete(Guid id)
        {
            var response = await _shareServiceClient.DeleteShareAsync(id.ToString());

            return response.Status == (int) HttpStatusCode.Accepted;
        }
    }
}