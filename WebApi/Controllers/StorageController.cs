using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services.Database;
using WebApi.Services.Storage;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StorageController : ControllerBase
    {
        private readonly IStorage _storage;
        private readonly INodeService _nodeService;
        private readonly IUserService _userService;
        private readonly IUserNodeService _userNodeService;

        public StorageController(IStorage storage, INodeService nodeService, IUserService userService, IUserNodeService userNodeService)
        {
            _storage = storage;
            _nodeService = nodeService;
            _userService = userService;
            _userNodeService = userNodeService;
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> Upload(Guid id, [FromForm] IFormFile file)
        {
            var user = await GetUser(User);
            if (user is null)
            {
                return Unauthorized();
            }

            var node = await _nodeService.Get(id);
            if (node is null)
            {
                return NotFound();
            }

            var userNode = (await _userNodeService.GetAll())
                .Where(x => x.UserId == user.Id)
                .FirstOrDefault(x => x.NodeId == id);

            if (node.OwnerId == user.Id || userNode != null && userNode.AccessTypeId == AccessTypeId.Write)
            {
                node.Size = file.Length;
                node.ModificationTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                node.ModifiedById = node.OwnerId == user.Id ? node.OwnerId : userNode.UserId;

                await _nodeService.Update(node);

                var result = await _storage.Upload(id, file.OpenReadStream());

                if (result)
                {
                    return Ok();
                }

                return Problem();
            }

            return Forbid();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var user = await GetUser(User);
            if (user is null)
            {
                return Unauthorized();
            }

            var node = await _nodeService.Get(id);
            if (node is null)
            {
                return NotFound();
            }

            var userNode = (await _userNodeService.GetAll())
                .Where(x => x.UserId == user.Id)
                .FirstOrDefault(x => x.NodeId == id);

            if (node.OwnerId == user.Id || userNode != null)
            {
                var stream = _storage.Download(id);
                if (stream == Stream.Null)
                {
                    return NotFound();
                }

                return File(stream, "application/octet-stream");
            }

            return Forbid();
        }

        [HttpGet]
        [Route("photo/{id}")]
        public IActionResult DownloadProfilePhoto(Guid id)
        {
            var stream = _storage.Download(id);
            if (stream == Stream.Null)
            {
                return NotFound();
            }

            return File(stream, "application/octet-stream");
        }

        [HttpPost]
        [Route("photo/{id}")]
        public async Task<IActionResult> UploadProfilePhoto(Guid id, [FromForm] IFormFile file)
        {
            var user = await GetUser(User);
            if (user is null)
            {
                return Unauthorized();
            }

            if (user.Id != id)
            {
                return Forbid();
            }

            var result = await _storage.Upload(id, file.OpenReadStream());

            if (result)
            {
                return Ok();
            }

            return Problem();
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
