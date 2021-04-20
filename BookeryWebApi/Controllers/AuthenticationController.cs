using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BookeryWebApi.Common;
using BookeryWebApi.Dtos.Requests;
using BookeryWebApi.Dtos.Responses;
using BookeryWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

namespace BookeryWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IJwtService _jwtService;

        public AuthenticationController(DatabaseContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost]
        [Route("token")]
        public IActionResult Token([FromBody] AuthenticationRequest authenticationRequest)
        {
            var identity = GetIdentity(authenticationRequest);

            if (identity is null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, authenticationRequest.Login)
            };

            var authenticationResult = _jwtService.Authenticate(authenticationRequest.Login, claims, DateTime.UtcNow);

            var response = new AuthenticationResponse
            {
                Username = identity.Name,
                AccessToken = authenticationResult.AccessToken,
                RefreshToken = authenticationResult.RefreshToken.Token
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("refreshToken")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
        {
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            if (authorizationHeader.Count == 0)
            {
                return Unauthorized("Invalid token.");
            }

            var bearer = authorizationHeader[0];
            if(!bearer.Contains("Bearer "))
            {
                return Unauthorized("Invalid token.");
            }

            var accessToken = bearer.Replace("Bearer ", "");
            var authenticationResult = _jwtService.Refresh(accessToken, refreshRequest.RefreshToken, DateTime.UtcNow);

            if (authenticationResult is null)
            {
                return Unauthorized("Invalid token.");
            }

            var response = new
            {
                username = authenticationResult.RefreshToken.Username,
                accessToken = authenticationResult.AccessToken,
                refreshToken = authenticationResult.RefreshToken.Token
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            _jwtService.ClearRefreshToken(User.Identity?.Name);

            return Ok();
        }

        private ClaimsIdentity GetIdentity(AuthenticationRequest authenticationRequest)
        {
            var userEntity = _context.Users.FirstOrDefault(x => x.Login == authenticationRequest.Login &&
                                                                x.Password == authenticationRequest.Password);

            if (userEntity is null)
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userEntity.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "DefaultRole")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }
    }
}
