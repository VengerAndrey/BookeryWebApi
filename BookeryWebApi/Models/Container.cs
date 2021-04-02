using System;

namespace BookeryWebApi.Models
{
    public class Container
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Container()
        {
            Id = Guid.Empty;
            Name = "";
        }

        public Container(ContainerCreateDto containerCreateDto)
        {
            Id = Guid.NewGuid();
            Name = containerCreateDto.Name;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Container) obj);
        }

        protected bool Equals(Container other)
        {
            return Id.Equals(other.Id) && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public static bool operator ==(Container left, Container right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Container left, Container right)
        {
            return !Equals(left, right);
        }
    }
}
