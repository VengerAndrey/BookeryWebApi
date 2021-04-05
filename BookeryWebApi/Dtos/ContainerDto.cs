using System;
using BookeryWebApi.Entities;

namespace BookeryWebApi.Dtos
{
    public class ContainerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OwnerLogin { get; set; }

        public ContainerDto()
        {
            Id = Guid.Empty;
            Name = "";
            OwnerLogin = "";
        }

        public ContainerDto(ContainerEntity containerEntity)
        {
            Id = containerEntity.Id;
            Name = containerEntity.Name;
            OwnerLogin = containerEntity.OwnerLogin;
        }

        public ContainerDto(ContainerCreateDto containerCreateDto, string ownerLogin)
        {
            Id = Guid.NewGuid();
            Name = containerCreateDto.Name;
            OwnerLogin = ownerLogin;
        }

        public ContainerEntity ToContainerEntity() => new ContainerEntity {Id = Id, Name = Name, OwnerLogin = OwnerLogin};

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ContainerDto) obj);
        }

        protected bool Equals(ContainerDto other)
        {
            return Id.Equals(other.Id) && Name == other.Name && OwnerLogin.Equals(other.OwnerLogin);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, OwnerLogin);
        }

        public static bool operator ==(ContainerDto left, ContainerDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ContainerDto left, ContainerDto right)
        {
            return !Equals(left, right);
        }
    }
}
