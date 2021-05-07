using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IBlobService
    {
        Task<bool> Upload(int id, Stream content);
        Task<Stream> Download(int id);
    }
}
