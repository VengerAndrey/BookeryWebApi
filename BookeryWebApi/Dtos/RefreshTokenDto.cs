using System;

namespace BookeryWebApi.Dtos
{
    public class RefreshTokenDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
