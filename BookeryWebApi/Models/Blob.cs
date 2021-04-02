using System;

namespace BookeryWebApi.Models
{
    public class Blob
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid IdContainer { get; set; }
        public string ContentBase64 { get; set; }

        public Blob()
        {
            Id = Guid.Empty;
            Name = "";
            IdContainer = Guid.Empty;
            ContentBase64 = "";
        }

        public Blob(BlobUploadDto blobUploadDto, Guid idContainer)
        {
            Id = Guid.NewGuid();
            Name = blobUploadDto.Name;
            IdContainer = idContainer;
            ContentBase64 = blobUploadDto.ContentBase64;
        }
    }
}
