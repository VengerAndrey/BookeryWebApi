using System.Collections.Generic;
using Domain.Models.DTOs;

namespace Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<Share> Shares { get; set; }

        public UserDto ToDto() => new UserDto {Id = Id, Email = Email, Username = Username, Password = Password};
    }
}
