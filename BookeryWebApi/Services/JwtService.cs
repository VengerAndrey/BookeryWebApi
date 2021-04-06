using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using BookeryWebApi.Common;
using BookeryWebApi.Dtos;
using BookeryWebApi.Dtos.Responses;
using Microsoft.IdentityModel.Tokens;

namespace BookeryWebApi.Services
{
    public class JwtService : IJwtService
    {
        private readonly ConcurrentDictionary<string, RefreshTokenDto> _refreshTokens = new ConcurrentDictionary<string, RefreshTokenDto>();

        public AuthenticationResponse Authenticate(string username, Claim[] claims, DateTime now)
        {
            var needAudience =
                string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);

            var jwt = new JwtSecurityToken(
                issuer: AuthenticationOptions.Issuer,
                audience: needAudience ? AuthenticationOptions.Audience : string.Empty,
                claims: claims,
                expires: now.AddSeconds(AuthenticationOptions.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256Signature));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refreshToken = new RefreshTokenDto
            {
                Username = username,
                Token = GenerateRefreshTokenString(),
                ExpireAt = now.AddSeconds(AuthenticationOptions.RefreshTokenExpiration)
            };

            _refreshTokens.AddOrUpdate(refreshToken.Token, refreshToken, (s, dto) => refreshToken);

            return new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public AuthenticationResponse Refresh(string accessToken, string refreshToken, DateTime now)
        {
            var (principal, jwt) = DecodeJwt(accessToken);

            if (jwt is null || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                return null;
            }

            var username = principal.Identity?.Name;

            if (!_refreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
            {
                return null;
            }

            if (username != existingRefreshToken.Username || existingRefreshToken.ExpireAt < now)
            {
                return null;
            }

            return Authenticate(username, principal.Claims.ToArray(), now);
        }

        public void ClearExpiredRefreshTokens(DateTime now)
        {
            var expiredRefreshTokens = _refreshTokens.Where(x => x.Value.ExpireAt < now).ToList();

            foreach (var expiredRefreshToken in expiredRefreshTokens)
            {
                _refreshTokens.TryRemove(expiredRefreshToken.Key, out _);
            }
        }

        public void ClearRefreshToken(string username)
        {
            var refreshTokens = _refreshTokens.Where(x => x.Value.Username == username).ToList();

            foreach (var expiredRefreshToken in refreshTokens)
            {
                _refreshTokens.TryRemove(expiredRefreshToken.Key, out _);
            }
        }

        private (ClaimsPrincipal, JwtSecurityToken) DecodeJwt(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return (null, null);
            }

            var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AuthenticationOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = AuthenticationOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthenticationOptions.GetSymmetricSecurityKey(),
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            return (principal, validatedToken as JwtSecurityToken);
        }

        private string GenerateRefreshTokenString()
        {
            var bytes = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
