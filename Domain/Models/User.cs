using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        [JsonIgnore] public ICollection<Share> Shares { get; set; } = new List<Share>();
    }
}