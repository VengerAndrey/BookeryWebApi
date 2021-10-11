using System;
using System.Threading.Tasks;
using EntityFramework.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    /*[ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccessController : ControllerBase
    {
        private readonly IAccessService _accessService;

        public AccessController(IAccessService accessService)
        {
            _accessService = accessService;
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> AccessById(Guid id)
        {
            var result = await _accessService.AccessById(User.Identity?.Name, id);

            if (result)
            {
                return Ok();
            }

            return NotFound();
        }
    }*/
}