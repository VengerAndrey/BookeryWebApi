using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookeryWebApi.Models
{
    public class BlobCreateDto
    {
        public string Name { get; set; }
        public Guid IdContainer { get; set; }
        public Stream Content { get; set; }
    }
}
