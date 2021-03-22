using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace BookeryWebApi.Models
{
    public class Blob
    {
        public BlobDto BlobDto { get; set; }
        public Stream Content { get; set; }
    }
}
