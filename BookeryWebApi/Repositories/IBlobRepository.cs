using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public interface IBlobRepository
    {
        Task<BlobDto> AddBlobAsync(Blob blob);
        Task<Blob> GetBlobAsync(Guid idBlob);
        Task<BlobDto> PutBlobAsync(Guid idBlob, BlobUploadDto blobUploadDto);
        Task<BlobDto> DeleteBlobAsync(Guid idBlob);
        Task<IEnumerable<BlobDto>> DeleteBlobsAsync();
    }
}
