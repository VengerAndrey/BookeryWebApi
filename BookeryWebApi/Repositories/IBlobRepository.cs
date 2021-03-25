using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public interface IBlobRepository
    {
        Task<IEnumerable<BlobDto>> ListBlobsAsync(Guid idContainer);
        Task<BlobDto> AddBlobAsync(Guid idContainer, BlobUploadDto blobUploadDto);
        Task<IEnumerable<BlobDto>> DeleteBlobsAsync(Guid idContainer);
        Task<BlobDto> ListBlobAsync(Guid idContainer, Guid idBlob);
        Task<Blob> GetBlobAsync(Guid idContainer, Guid idBlob);
        Task<BlobDto> PutBlobAsync(Guid idContainer, Guid idBlob, BlobUploadDto blobUploadDto);
        Task<BlobDto> DeleteBlobAsync(Guid idContainer, Guid idBlob);
    }
}
