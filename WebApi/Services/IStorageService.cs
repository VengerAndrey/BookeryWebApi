using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Domain.Models;

namespace WebApi.Services
{
    public interface IStorageService
    {
        // shares
        Task<IEnumerable<Share>> GetAllShares();
        Task<Share> GetShare(Guid id);
        Task<Share> CreateShare(Share share);
        Task<bool> DeleteShare(Guid id);
        Task<Share> UpdateShare(Share share);

        // share items
        Task<IEnumerable<Item>> GetSubItems(string path);
        Task<Item> CreateDirectory(string path);
        Task<bool> UploadFile(string path, string name, Stream content);
        Task<Stream> DownloadFile(string path);
    }
}
