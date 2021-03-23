﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace BookeryWebApi.Models
{
    public class Blob
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid IdContainer { get; set; }
        public Stream Content { get; set; }
    }
}
