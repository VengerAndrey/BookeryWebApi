using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebApi.Services.Item
{
    public interface IItemService
    {
        Task<Domain.Models.Node> GetItem(string path);
        Task<IEnumerable<Domain.Models.Node>> GetSubItems(string path);
        Task<Domain.Models.Node> CreateDirectory(string path);
        Task<Domain.Models.Node> RenameFile(string path, string name);
        Task<Domain.Models.Node> UploadFile(string path, string name, Stream content);
        Task<Stream> DownloadFile(string path);
        Task<bool> Delete(string path);
    }
}