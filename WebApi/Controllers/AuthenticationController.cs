﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Models.DTOs.Requests;
using Domain.Models.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WebApi.Common;
using WebApi.Services.Database;
using WebApi.Services.Hash;
using WebApi.Services.JWT;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IHasher _hasher;

        public AuthenticationController(IJwtService jwtService, IUserService userService, IHasher hasher)
        {
            _jwtService = jwtService;
            _userService = userService;
            _hasher = hasher;
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
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim("LastName", user.LastName),
                new Claim("FirstName", user.FirstName)
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
        [Route("log-out")]
        public IActionResult LogOut()
        {
            _jwtService.ClearRefreshToken(User.Identity?.Name);

            return Ok();
        }

        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest signUpRequest)
        {
            var user = await _userService.GetByEmail(signUpRequest.Email);

            if (user != null)
            {
                return BadRequest(SignUpResult.EmailAlreadyExists);
            }

            if (!EmailValidator.Validate(signUpRequest.Email))
            {
                return BadRequest(SignUpResult.InvalidEmail);
            }
            
            await _userService.Create(new User
            {
                Email = signUpRequest.Email,
                LastName = signUpRequest.LastName,
                FirstName = signUpRequest.FirstName,
                Password = _hasher.Hash(signUpRequest.Password)
            });

            return Ok(SignUpResult.Success);
        }

        private async Task<ClaimsIdentity> GetIdentity(AuthenticationRequest authenticationRequest)
        {
            var user = await _userService.GetByEmail(authenticationRequest.Email);

            var hashedPassword = _hasher.Hash(authenticationRequest.Password);

            if (user is null || user.Password != hashedPassword)
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