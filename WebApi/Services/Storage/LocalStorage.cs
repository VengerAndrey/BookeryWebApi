using System;
using System.IO;
using System.Threading.Tasks;

namespace WebApi.Services.Storage
{
    public class LocalStorage : IStorage
    {
        private readonly string _rootPath = @"D:/localStorage/data";

        public async Task<bool> Upload(Guid id, Stream content)
        {
            try
            {
                await using var stream = File.Create(Path.Combine(_rootPath, id.ToString()));
                await content.CopyToAsync(stream);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public Stream Download(Guid id)
        {
            try
            {
                var stream = File.OpenRead(Path.Combine(_rootPath, id.ToString()));
                return stream;
            }
            catch (Exception e)
            {
                return Stream.Null;
            }
        }
    }
}
