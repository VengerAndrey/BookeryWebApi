using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;

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
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.Get(id);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user.ToDto());
        }
    }
}
