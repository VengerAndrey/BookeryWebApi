using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Share
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Share) obj);
        }

        protected bool Equals(Share other)
        {
            return Id.Equals(other.Id) && Name == other.Name && UserId == other.UserId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, UserId);
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
