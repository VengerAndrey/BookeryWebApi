using System.IO;
using System.Threading.Tasks;

namespace WebApi.Services.Photo
{
    public interface IPhotoService
    {
        Task<Stream> Get(int userId);
        Task<bool> Set(int userId, string extension, Stream content);
    }
}