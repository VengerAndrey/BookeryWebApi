using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookeryWebApi.Dtos;

namespace BookeryWebApi.Repositories
{
    public interface IBlobRepository
    {
        Task<BlobInfoDto> AddBlobAsync(BlobDto blobDto);
        Task<BlobDto> GetBlobAsync(Guid idBlob);
        Task<BlobInfoDto> PutBlobAsync(Guid idBlob, BlobUploadDto blobUploadDto);
        Task<BlobInfoDto> DeleteBlobAsync(Guid idBlob);
        Task<IEnumerable<BlobInfoDto>> DeleteBlobsAsync();
    }
}
