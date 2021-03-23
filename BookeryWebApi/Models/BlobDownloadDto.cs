using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookeryWebApi.Models
{
    public class BlobDownloadDto
    {
        public Guid Id { get; set; }
        public Guid IdContainer { get; set; }
    }
}
