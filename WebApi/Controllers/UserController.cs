using System.IO;
using System.Threading.Tasks;
using EntityFramework.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Photo;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly IUserService _userService;

        public UserController(IUserService userService, IPhotoService photoService)
        {
            _userService = userService;
            _photoService = photoService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var user = await _userService.GetByEmail(User.Identity?.Name);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        [Route("photo")]
        public async Task<IActionResult> GetPhoto()
        {
            var user = await _userService.GetByEmail(User.Identity?.Name);

            var photo = await _photoService.Get(user.Id);

            if (photo is null)
            {
                return NotFound();
            }

            return File(photo, "application/octet-stream");
        }

        [HttpPost]
        [Route("photo")]
        public async Task<IActionResult> SetPhoto([FromForm] IFormFile file)
        {
            var user = await _userService.GetByEmail(User.Identity?.Name);

            return Ok(await _photoService.Set(user.Id, Path.GetExtension(file.FileName), file.OpenReadStream()));
        }
    }
}