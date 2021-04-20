using System;
using System.Security.Claims;
using BookeryWebApi.Dtos;

namespace BookeryWebApi.Services
{
    public interface IJwtService
    {
        Token Authenticate(string username, Claim[] claims, DateTime now);
        Token Refresh(string accessToken, string refreshToken, DateTime now);
        void ClearExpiredRefreshTokens(DateTime now);
        void ClearRefreshToken(string username);
    }
}
