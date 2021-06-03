using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Item;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        [Route("item/{*path}")]
        public async Task<IActionResult> GetItem(string path)
        {
            var item = await _itemService.GetItem(path);

            if (item is null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpGet]
        [Route("sub-items/{*path}")]
        public async Task<IActionResult> GetSubItems(string path)
        {
            var items = await _itemService.GetSubItems(path);

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
            var created = await _itemService.CreateDirectory(path);

            if (created is null)
            {
                return Problem("Can't create a directory.");
            }

            return Created(created.Path, created);
        }

        [HttpPost]
        [Route("upload-file/{*path}")]
        public async Task<IActionResult> UploadFile(string path, [FromForm] IFormFile file)
        {
            var result = await _itemService.UploadFile(path, file.FileName, file.OpenReadStream());

            if (result is null)
            {
                return Problem("Can't upload a file.");
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("download-file/{*path}")]
        public async Task<IActionResult> DownloadFile(string path)
        {
            var stream = await _itemService.DownloadFile(path);

            return File(stream, "application/octet-stream");
        }

        [HttpDelete]
        [Route("delete/{*path}")]
        public async Task<IActionResult> Delete(string path)
        {
            return Ok(await _itemService.Delete(path));
        }
    }
}