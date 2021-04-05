using System;
using BookeryWebApi.Entities;

namespace BookeryWebApi.Dtos
{
    public class BlobDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid IdContainer { get; set; }
        public string ContentBase64 { get; set; }

        public BlobDto()
        {
            Id = Guid.Empty;
            Name = "";
            IdContainer = Guid.Empty;
            ContentBase64 = "";
        }

        public BlobDto(BlobEntity blobEntity)
        {
            Id = blobEntity.Id;
            Name = blobEntity.Name;
            IdContainer = blobEntity.IdContainer;
            ContentBase64 = "";
        }

        public BlobDto(BlobUploadDto blobUploadDto, Guid idContainer)
        {
            Id = Guid.NewGuid();
            Name = blobUploadDto.Name;
            IdContainer = idContainer;
            ContentBase64 = blobUploadDto.ContentBase64;
        }

        public BlobEntity ToBlobEntity() => new BlobEntity {Id = Id, Name = Name, IdContainer = IdContainer};
    }
}
