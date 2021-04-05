using System;
using BookeryWebApi.Entities;

namespace BookeryWebApi.Dtos
{
    public class BlobInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid IdContainer { get; set; }

        public BlobInfoDto()
        {
            Id = Guid.Empty;
            Name = "";
            IdContainer = Guid.Empty;
        }

        public BlobInfoDto(BlobDto blobDto)
        {
            Id = blobDto.Id;
            Name = blobDto.Name;
            IdContainer = blobDto.IdContainer;
        }

        public BlobInfoDto(BlobEntity blobEntity)
        {
            Id = blobEntity.Id;
            Name = blobEntity.Name;
            IdContainer = blobEntity.IdContainer;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BlobInfoDto) obj);
        }

        public BlobEntity ToBlobEntity() => new BlobEntity { Id = Id, Name = Name, IdContainer = IdContainer };

        protected bool Equals(BlobInfoDto other)
        {
            return Id.Equals(other.Id) && Name == other.Name && IdContainer.Equals(other.IdContainer);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, IdContainer);
        }

        public static bool operator ==(BlobInfoDto left, BlobInfoDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BlobInfoDto left, BlobInfoDto right)
        {
            return !Equals(left, right);
        }
    }
}
