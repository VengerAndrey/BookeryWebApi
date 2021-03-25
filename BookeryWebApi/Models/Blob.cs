using System;

namespace BookeryWebApi.Models
{
    public class Blob
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid IdContainer { get; set; }
        public string ContentBase64 { get; set; }
    }
}
