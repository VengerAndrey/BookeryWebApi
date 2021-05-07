using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using WebApi.Dtos;
using WebApi.Dtos.Responses;
using Microsoft.IdentityModel.Tokens;
using WebApi.Common;

namespace WebApi.Services
{
    public class JwtService : IJwtService
    {
        private readonly ConcurrentDictionary<string, RefreshTokenDto> _refreshTokens = new ConcurrentDictionary<string, RefreshTokenDto>();

        public AuthenticationResponse Authenticate(string email, Claim[] claims, DateTime now)
        {
            var needAudience =
                string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);

            var accessTokenExpireAt = now.AddSeconds(AuthenticationOptions.AccessTokenExpiration);

            var jwt = new JwtSecurityToken(
                issuer: AuthenticationOptions.Issuer,
                audience: needAudience ? AuthenticationOptions.Audience : string.Empty,
                claims: claims,
                expires: accessTokenExpireAt,
                signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256Signature));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refreshToken = new RefreshTokenDto
            {
                Email = email,
                Token = GenerateRefreshTokenString(),
                ExpireAt = now.AddSeconds(AuthenticationOptions.RefreshTokenExpiration)
            };

            _refreshTokens.AddOrUpdate(refreshToken.Token, refreshToken, (s, dto) => refreshToken);

            return new AuthenticationResponse
            {
                Email = email,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpireAt = accessTokenExpireAt
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

            if (username != existingRefreshToken.Email || existingRefreshToken.ExpireAt < now)
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

        public void ClearRefreshToken(string email)
        {
            var refreshTokens = _refreshTokens.Where(x => x.Value.Email == email).ToList();

            foreach (var expiredRefreshToken in refreshTokens)
            {
                _refreshTokens.TryRemove(expiredRefreshToken.Key, out _);
            }
        }

        private static (ClaimsPrincipal, JwtSecurityToken) DecodeJwt(string token)
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

        private static string GenerateRefreshTokenString()
        {
            var bytes = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
