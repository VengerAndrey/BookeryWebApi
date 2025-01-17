﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Database;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var user = await GetUser(User);

            if (user is null)
            {
                return NotFound();
            }

            user.Password = null;

            return Ok(user);
        }

        [HttpGet]
        [Route("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmail(email);

            if (user is null)
            {
                return NotFound();
            }

            user.Password = null;

            return new JsonResult(user);
        }

        [HttpGet]
        [Route("id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.Get(id);

            if (user is null)
            {
                return NotFound();
            }

            user.Password = null;

            return new JsonResult(user);
        }

        private async Task<User> GetUser(ClaimsPrincipal principal)
        {
            var userIdString = principal.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return null;
            }

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return null;
            }

            return await _userService.Get(userId);
        }
    }
}