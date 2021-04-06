using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookeryWebApi.Dtos.Responses;

namespace BookeryWebApi.Services
{
    public interface IJwtService
    {
        AuthenticationResponse Authenticate(string username, Claim[] claims, DateTime now);
        AuthenticationResponse Refresh(string accessToken, string refreshToken, DateTime now);
        void ClearExpiredRefreshTokens(DateTime now);
        void ClearRefreshToken(string username);
    }
}
