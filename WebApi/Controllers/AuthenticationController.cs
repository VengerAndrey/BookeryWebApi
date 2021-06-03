using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Models.DTOs.Requests;
using EntityFramework.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WebApi.Services.JWT;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;

        public AuthenticationController(IJwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token([FromBody] AuthenticationRequest authenticationRequest)
        {
            var identity = await GetIdentity(authenticationRequest);

            if (identity is null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var user = await _userService.GetByEmail(authenticationRequest.Email);

            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, authenticationRequest.Email)
            };

            var response = _jwtService.Authenticate(authenticationRequest.Email, claims, DateTime.UtcNow);

            return Ok(response);
        }

        [HttpPost]
        [Route("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
        {
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            if (authorizationHeader.Count == 0)
            {
                return Unauthorized("Invalid token.");
            }

            var bearer = authorizationHeader[0];
            if (!bearer.Contains("Bearer "))
            {
                return Unauthorized("Invalid token.");
            }

            var accessToken = bearer.Replace("Bearer ", "");
            var response = _jwtService.Refresh(accessToken, refreshRequest.RefreshToken, DateTime.UtcNow);

            if (response is null)
            {
                return Unauthorized("Invalid token.");
            }

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

        private async Task<ClaimsIdentity> GetIdentity(AuthenticationRequest authenticationRequest)
        {
            var user = await _userService.GetByEmail(authenticationRequest.Email);

            if (user is null || user.Password != authenticationRequest.Password)
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "DefaultRole")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }
    }
}