using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using EntityFramework.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Exceptions;
using WebApi.Services.Share;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShareController : ControllerBase
    {
        private readonly IShareService _shareService;
        private readonly IUserService _userService;

        public ShareController(IShareService shareService, IUserService userService)
        {
            _userService = userService;
            _shareService = shareService;
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
            var share = new Share {Name = name, OwnerId = user.Id};

            try
            {
                var result = await _shareService.Create(share);

                return Ok(result);
            }
            catch (ShareCRUDException e)
            {
                return Problem($"Can't create a share [Id={e.ShareId}, Name={e.ShareName}, OwnerId={e.OwnerId}].");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateShare([FromBody] Share share)
        {
            var user = await _userService.GetByEmail(User.Identity?.Name);
            if (user.Shares.FirstOrDefault(x => x.Id == share.Id && x.OwnerId == user.Id) == null) return Forbid();

            try
            {
                var result = await _shareService.Update(share.Id, share);

                return Ok(result);
            }
            catch (ShareCRUDException e)
            {
                return Problem($"Can't update a share [Id={e.ShareId}, Name={e.ShareName}, OwnerId={e.OwnerId}].");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetShare(Guid id)
        {
            var share = await _shareService.Get(id);

            if (share is null) return NotFound();

            return Ok(share);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteShare(Guid id)
        {
            var user = await _userService.GetByEmail(User.Identity?.Name);
            if (user.Shares.FirstOrDefault(x => x.Id == id && x.OwnerId == user.Id) == null) return Forbid();

            try
            {
                var result = await _shareService.Delete(id);

                return Ok(result);
            }
            catch (ShareCRUDException e)
            {
                return Problem($"Can't update a share [Id={e.ShareId}, Name={e.ShareName}, OwnerId={e.OwnerId}].");
            }
        }
    }
}