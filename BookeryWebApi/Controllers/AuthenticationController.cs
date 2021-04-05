using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using BookeryWebApi.Common;
using Microsoft.IdentityModel.Tokens;

namespace BookeryWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public AuthenticationController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("token")]
        public IActionResult Token([FromHeader(Name = "Username")] string login, [FromHeader(Name = "Password")] string password)
        {
            var identity = GetIdentity(login, password);

            if (identity is null)
            {
                return BadRequest("Invalid username or password.");
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: AuthenticationOptions.Issuer,
                audience: AuthenticationOptions.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromSeconds(AuthenticationOptions.Lifetime)),
                signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Ok(response);
        }

        private ClaimsIdentity GetIdentity(string login, string password)
        {
            var userEntity = _context.Users.FirstOrDefault(x => x.Login == login && x.Password == password);

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
