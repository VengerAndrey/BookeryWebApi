using System;

namespace BookeryWebApi.Models
{
    public class Container
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OwnerLogin { get; set; }

        public Container()
        {
            Id = Guid.Empty;
            Name = "";
            OwnerLogin = "";
        }

        public Container(ContainerCreateDto containerCreateDto, string ownerLogin)
        {
            Id = Guid.NewGuid();
            Name = containerCreateDto.Name;
            OwnerLogin = ownerLogin;
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
            return Id.Equals(other.Id) && Name == other.Name && OwnerLogin.Equals(other.OwnerLogin);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, OwnerLogin);
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
