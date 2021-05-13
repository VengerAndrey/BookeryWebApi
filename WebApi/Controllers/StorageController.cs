using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IShareService _shareService;
        private readonly IUserService _userService;

        public StorageController(IStorageService storageService, IShareService shareService, IUserService userService)
        {
            _storageService = storageService;
            _shareService = shareService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShares()
        {
            var user = await _userService.GetByEmail(User.Identity?.Name);

            return Ok(user.Shares);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShare([FromBody] string name)
        {
            var user = await _userService.GetByEmail(User.Identity?.Name);
            var share = new Share {Id = Guid.NewGuid(), Name = name, UserId = user.Id};

            var storageResult = await _storageService.CreateShare(share);
            var dbResult = await _shareService.Create(share);

            if (storageResult is null || dbResult is null || storageResult != dbResult)
            {
                return Problem("Can't create a share.");
            }

            return Ok(storageResult);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateShare([FromBody] Share share)
        {
            var user = await _userService.GetByEmail(User.Identity?.Name);
            if (user.Shares.FirstOrDefault(x => x.Id == share.Id && x.UserId == user.Id) == null)
            {
                return Problem("No access.");
            }

            var storageResult = await _storageService.UpdateShare(share);
            var dbResult = await _shareService.Update(share);

            if (storageResult is null || dbResult is null || storageResult != dbResult)
            {
                return Problem("Can't update a share.");
            }

            return Ok(storageResult);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetShare(Guid id)
        {
            var share = await _storageService.GetShare(id);

            if (share is null)
            {
                return NotFound();
            }

            return Ok(share);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteShare(Guid id)
        {
            var user = await _userService.GetByEmail(User.Identity?.Name);
            if (user.Shares.FirstOrDefault(x => x.Id == id && x.UserId == user.Id) == null)
            {
                return Problem("No access.");
            }

            var storageResult = await _storageService.DeleteShare(id);
            var dbResult = await _shareService.Delete(id);

            if (!storageResult || !dbResult)
            {
                return Problem("Can't delete a share.");
            }

            return Ok();
        }

        [HttpGet]
        [Route("sub/{*path}")]
        public async Task<IActionResult> GetSubItems(string path)
        {
            var items = await _storageService.GetSubItems(path);

            if (items is null)
            {
                return NotFound();
            }

            return Ok(items);
        }

        [HttpPost]
        [Route("create-directory/{*path}")]
        public async Task<IActionResult> CreateDirectory(string path)
        {
            var created = await _storageService.CreateDirectory(path);

            if (created is null)
            {
                return Problem("Can't create a directory.");
            }

            return Created(created.Path, created);
        }

        [HttpPost]
        [Route("upload/{*path}")]
        public async Task<IActionResult> UploadFile(string path, [FromForm] IFormFile file)
        {
            var result = await _storageService.UploadFile(path, file.FileName, file.OpenReadStream());

            if (result)
            {
                return Ok();
            }

            return Problem("Can't upload a file.");
        }

        [HttpGet]
        [Route("download/{*path}")]
        public async Task<IActionResult> DownloadFile(string path)
        {
            var stream = await _storageService.DownloadFile(path);

            return File(stream, "application/octet-stream");
        }
    }
}
