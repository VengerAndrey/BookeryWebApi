using System;

namespace Domain.Models.DTOs
{
    public class RefreshToken
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}