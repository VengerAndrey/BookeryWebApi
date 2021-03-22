using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public class MockBlobRepository : IBlobRepository
    {
        private string _path = "D:/blobs/";

        public async Task<IEnumerable<Container>> ListContainersAsync()
        {
            return await Task.Run(() =>
            {
                var containersNames = Directory.GetDirectories(_path);

                var containers = new List<Container>();

                foreach (var fullContainerName in containersNames)
                {
                    var containerName = Path.GetFileName(fullContainerName);
                    int split = containerName.IndexOf("!");
                    var name = containerName.Substring(0, split);
                    var id = containerName.Substring(name.Length + 1, containerName.Length - name.Length - 1);
                    containers.Add(new Container { Id = Guid.Parse(id), Name = name });
                }

                return containers;
            });
        }

        public async Task AddContainerAsync(Container container)
        {
            await Task.Run(() =>
            {
                Directory.CreateDirectory(_path + container.Name + "!" + container.Id);
            });
        }

        public async Task<IEnumerable<BlobDto>> ListBlobsAsync(Container container)
        {
            /*return await Task.Run(async () =>
            {
                var containers = await ListContainersAsync();

                var first = containers.First(x => x.Id == container.Id);

                var files = Directory.GetFiles(_path + first.Name + "!" + first.Id);

                var blobs = new List<Blob>();

                foreach (var file in files)
                {
                    blobs.Add(new Blob {Id = Guid.NewGuid(), Name = file, IdContainer = fi});
                }

                return blobs;
            });*/

            return new List<BlobDto>();
        }

        public Task<IEnumerable<Blob>> GetBlobsAsync(Container container)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BlobDownloadInfo>> GetBlobsAsync(Guid idContainer)
        {
            throw new NotImplementedException();
        }

        public Task AddBlobAsync(Container container, Blob blob)
        {
            throw new NotImplementedException();
        }
    }
}
