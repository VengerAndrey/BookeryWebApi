using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares.Models;
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
        Task<Item> CreateItem(Item item);
        Task<bool> UploadFile(string path, Stream content);
    }
}
