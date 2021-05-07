using System;
using System.Security.Claims;
using WebApi.Dtos.Responses;

namespace WebApi.Services
{
    public interface IJwtService
    {
        AuthenticationResponse Authenticate(string email, Claim[] claims, DateTime now);
        AuthenticationResponse Refresh(string accessToken, string refreshToken, DateTime now);
        void ClearExpiredRefreshTokens(DateTime now);
        void ClearRefreshToken(string email);
    }
}
