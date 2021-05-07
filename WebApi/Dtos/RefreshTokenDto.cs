using System;

namespace WebApi.Dtos
{
    public class RefreshTokenDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
