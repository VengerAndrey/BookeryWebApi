using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityFramework.Services;
using WebApi.Exceptions;

namespace WebApi.Services.Share
{
    public class ShareService : IShareService
    {
        private readonly IAzureShareService _azureShareService;
        private readonly IDbShareService _dbShareService;

        public ShareService(IAzureShareService azureShareService, IDbShareService dbShareService)
        {
            _azureShareService = azureShareService;
            _dbShareService = dbShareService;
        }

        public Task<IEnumerable<Domain.Models.Share>> GetAll()
        {
            return _azureShareService.GetAll();
        }

        public Task<Domain.Models.Share> Get(Guid id)
        {
            return _azureShareService.Get(id);
        }

        public async Task<Domain.Models.Share> Create(Domain.Models.Share entity)
        {
            entity.Id = Guid.NewGuid();

            var azureResult = await _azureShareService.Create(entity);
            var dbResult = await _dbShareService.Create(entity);

            if (azureResult is null || dbResult is null || azureResult != dbResult)
            {
                throw new ShareCRUDException(entity.Id, entity.Name, entity.OwnerId);
            }

            return azureResult;
        }

        public async Task<Domain.Models.Share> Update(Guid id, Domain.Models.Share entity)
        {
            var azureResult = await _azureShareService.Update(id, entity);
            var dbResult = await _dbShareService.Update(id, entity);

            if (azureResult is null || dbResult is null || azureResult != dbResult)
            {
                throw new ShareCRUDException(entity.Id, entity.Name, entity.OwnerId);
            }

            return azureResult;
        }

        public async Task<bool> Delete(Guid id)
        {
            var azureResult = await _azureShareService.Delete(id);
            var dbResult = await _dbShareService.Delete(id);

            if (!azureResult || !dbResult)
            {
                throw new ShareCRUDException(id);
            }

            return true;
        }
    }
}