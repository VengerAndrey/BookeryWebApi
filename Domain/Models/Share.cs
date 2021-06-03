using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Share
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }

        [JsonIgnore] public ICollection<User> Users { get; set; } = new List<User>();

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Share) obj);
        }

        protected bool Equals(Share other)
        {
            return Id.Equals(other.Id) && Name == other.Name && OwnerId == other.OwnerId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, OwnerId);
        }

        public static bool operator ==(Share left, Share right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Share left, Share right)
        {
            return !Equals(left, right);
        }
    }
}