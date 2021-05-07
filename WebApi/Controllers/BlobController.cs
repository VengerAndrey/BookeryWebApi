using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BlobController : ControllerBase
    {
        private readonly IBlobService _blobService;

        public BlobController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> Upload(int id, IFormFile file)
        {
            var uploadResult = await _blobService.Upload(id, file.OpenReadStream());

            return Ok(uploadResult);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var stream = await _blobService.Download(id);

            return File(stream, "application/octet-stream");
        }
    }
}
