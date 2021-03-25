using System;

namespace BookeryWebApi.Models
{
    public class BlobDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid IdContainer { get; set; }
    }
}
