using System;
using System.Security.Claims;
using Domain.Models.DTOs.Responses;

namespace WebApi.Services.JWT
{
    public interface IJwtService
    {
        AuthenticationResponse Authenticate(string email, Claim[] claims, DateTime now);
        AuthenticationResponse Refresh(string accessToken, string refreshToken, DateTime now);
        void ClearExpiredRefreshTokens(DateTime now);
        void ClearRefreshToken(string email);
    }
}