using System.Collections.Generic;
using Domain.Models.DTOs;

namespace Domain.Models
{
    public class User : DomainObject
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<Node> Nodes { get; set; }

        public UserDto ToDto() => new UserDto {Id = Id, Email = Email, Username = Username, Password = Password};
    }
}
