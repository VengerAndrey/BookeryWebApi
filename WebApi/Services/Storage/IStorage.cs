using System;
using System.IO;
using System.Threading.Tasks;

namespace WebApi.Services.Storage
{
    public interface IStorage
    {
        Task<bool> Upload(Guid id, Stream content);
        Stream Download(Guid id);
    }
}
