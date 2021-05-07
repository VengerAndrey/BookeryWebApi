using System;

namespace Domain.Models.DTOs.Responses
{
    public class AuthenticationResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
