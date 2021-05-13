using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShares()
        {
            var shares = await _storageService.GetAllShares();

            return Ok(shares);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShare([FromBody] string name)
        {
            var result = await _storageService.CreateShare(new Share {Id = Guid.Empty, Name = name});

            if (result is null)
            {
                return Problem("Can't create a share.");
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateShare([FromBody] Share share)
        {
            var result = await _storageService.UpdateShare(share);

            if (result is null)
            {
                return Problem("Can't update a share.");
            }

            return Ok(result);
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
            var result = await _storageService.DeleteShare(id);

            if (result)
            {
                return Ok();
            }

            return Problem("Can't delete a share.");
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
