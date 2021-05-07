using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Models.DTOs;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NodeController : ControllerBase
    {
        private readonly INodeService _nodeService;
        private readonly IUserService _userService;

        public NodeController(INodeService nodeService, IUserService userService)
        {
            _nodeService = nodeService;
            _userService = userService;
        }

        [HttpGet]
        [Route("userId={userId}")]
        public async Task<IActionResult> GetUserNodes(int userId)
        {
            var user = await _userService.Get(userId);

            if (user is null)
            {
                return NotFound();
            }

            var nodesId = user.Nodes.Select(userNode => userNode.Id).ToList();

            return Ok(nodesId);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var node = await _nodeService.Get(id);

            if (node is null)
            {
                return NotFound();
            }

            return Ok(node.ToDto());
        }

        [HttpGet]
        [Route("{id}/withSub")]
        public async Task<IActionResult> GetWithSub(int id)
        {
            var node = await _nodeService.GetWithSub(id);

            if (node is null)
            {
                return NotFound();
            }

            return Ok(node);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NodeDto nodeDto)
        {
            var node = new Node
            {
                Name = nodeDto.Name,
                ParentId = nodeDto.ParentId,
                OwnerId = nodeDto.OwnerId
            };

            var createdResult = await _nodeService.Create(node);

            return Ok(createdResult.ToDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] NodeDto nodeDto)
        {
            var node = new Node
            {
                Name = nodeDto.Name,
                ParentId = nodeDto.ParentId,
                OwnerId = nodeDto.OwnerId
            };

            var updateResult = await _nodeService.Update(id, node);

            if (updateResult is null)
            {
                return NotFound();
            }

            return Ok(updateResult);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteResult = await _nodeService.Delete(id);

            return Ok(deleteResult);
        }
    }
}
