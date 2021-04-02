using System;

namespace BookeryWebApi.Models
{
    public class BlobDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid IdContainer { get; set; }

        public BlobDto()
        {
            Id = Guid.Empty;
            Name = "";
            IdContainer = Guid.Empty;
        }

        public BlobDto(Blob blob)
        {
            Id = blob.Id;
            Name = blob.Name;
            IdContainer = blob.IdContainer;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BlobDto) obj);
        }

        protected bool Equals(BlobDto other)
        {
            return Id.Equals(other.Id) && Name == other.Name && IdContainer.Equals(other.IdContainer);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, IdContainer);
        }

        public static bool operator ==(BlobDto left, BlobDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BlobDto left, BlobDto right)
        {
            return !Equals(left, right);
        }
    }
}
