using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using WebApi.Common;
using WebApi.Services.Database;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/Node")]
    [Authorize]
    public class SharedNodeController : ControllerBase
    {
        private readonly INodeService _nodeService;
        private readonly IUserService _userService;
        private readonly IUserNodeService _userNodeService;
        private readonly PathBuilder _pathBuilder;

        public SharedNodeController(INodeService nodeService, IUserService userService, IUserNodeService userNodeService)
        {
            _nodeService = nodeService;
            _userService = userService;
            _userNodeService = userNodeService;
            _pathBuilder = new PathBuilder();
        }

        [HttpGet]
        [Route("shared/{*path}")]
        public async Task<IActionResult> Get(string path)
        {
            var user = await GetUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var allNodes = (await _userNodeService.GetAll())
                .Where(x => x.UserId == user.Id)
                .Select(async x => await _nodeService.Get(x.NodeId))
                .Select(x => x.Result)
                .ToList();

            var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);

            var levelTree = TreeExtensions.GetLevelTree(virtualRoot, path, true);

            if (levelTree is null)
            {
                return NotFound();
            }

            var nodes = levelTree.Children.Select(x => x.Data).ToList();

            return new JsonResult(nodes);
        }

        [HttpGet]
        [Route("sharing/{id}")]
        public async Task<IActionResult> GetSharing(Guid id)
        {
            var user = await GetUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var node = await _nodeService.Get(id);
            if (node is null)
            {
                return NotFound();
            }

            if (node.OwnerId != user.Id)
            {
                return Forbid();
            }

            var userNodes = (await _userNodeService.GetAll()).Where(x => x.NodeId == node.Id).ToList();

            return new JsonResult(userNodes);
        }

        [HttpGet]
        [Route("details/{id}")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var user = await GetUser(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var userNode = (await _userNodeService.GetAll())
                .FirstOrDefault(x => x.UserId == user.Id && x.NodeId == id);
            
            if (userNode is null)
            {
                return NotFound();
            }

            var node = await _nodeService.Get(id);

            if (node is null)
            {
                return NotFound();
            }

            return new JsonResult(node);
        }

        [HttpPost]
        [Route("create/{*path}")]
        public async Task<IActionResult> Create(string path, [FromBody] Node create)
        {
            var user = await GetUser(User);
            var userNodeResult = await GetSharedNode(user, path, false, true);

            if (userNodeResult.ActionResult != null)
            {
                return userNodeResult.ActionResult;
            }

            if (userNodeResult.LevelTree.Children.All(x => x.Data.Name != create.Name))
            {
                if (userNodeResult.UserNode.AccessTypeId != AccessTypeId.Write)
                {
                    return Forbid();
                }

                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                _pathBuilder.ParsePath(path);
                _pathBuilder.AddNode(create.Name);

                create.OwnerId = userNodeResult.LevelTree.Data.OwnerId;
                create.ModifiedById = user.Id;
                create.CreatedById = user.Id;

                if (!userNodeResult.LevelTree.IsRoot)
                {
                    create.ParentId = userNodeResult.LevelTree.Data.Id;
                }

                create.CreationTimestamp = timestamp;
                create.ModificationTimestamp = timestamp;

                var created = await _nodeService.Create(create);

                ShareNode(new UserNode
                {
                    UserId = user.Id,
                    NodeId = created.Id,
                    Timestamp = timestamp,
                    AccessTypeId = AccessTypeId.Write
                });

                return Created(_pathBuilder.GetPath(), created);
            }

            return Conflict();
        }

        [HttpPut]
        [Route("update/{*path}")]
        public async Task<IActionResult> Update(string path, [FromBody] Node update)
        {
            var user = await GetUser(User);
            var userNodeResult = await GetSharedNode(user, path, true);

            if (userNodeResult.ActionResult != null)
            {
                return userNodeResult.ActionResult;
            }

            if (userNodeResult.LevelTree.Children.Where(x => x.Data.Id != userNodeResult.Node.Id).All(x => x.Data.Name != update.Name))
            {
                if (userNodeResult.UserNode.AccessTypeId != AccessTypeId.Write)
                {
                    return Forbid();
                }

                var entity = await _nodeService.Get(userNodeResult.Node.Id);

                if (entity is null)
                {
                    return NotFound();
                }
                
                entity.Name = update.Name?.Length > 0 ? update.Name : entity.Name;
                if (update.Size == -1)
                {
                    entity.Size = null;
                }
                else
                {
                    entity.Size = (update.Size ?? 0) > 0 ? update.Size : entity.Size;
                }
                entity.ModificationTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                entity.ModifiedById = user.Id;

                await _nodeService.Update(entity);

                _pathBuilder.ParsePath(path);
                _pathBuilder.GetLastNode();
                _pathBuilder.AddNode(update.Name);

                return Accepted(_pathBuilder.GetPath());
            }

            return Conflict();
        }

        [HttpPost]
        [Route("share")]
        public async Task<IActionResult> Share([FromBody] UserNode userNode)
        {
            var owner = await GetUser(User);
            if (owner is null)
            {
                return Unauthorized();
            }

            var node = await _nodeService.Get(userNode.NodeId);
            if (node is null)
            {
                return NotFound();
            }

            if (userNode.UserId == owner.Id)
            {
                return BadRequest();
            }

            if (node.OwnerId != owner.Id)
            {
                return Forbid();
            }

            var user = await _userService.Get(userNode.UserId);
            if (user is null)
            {
                return NotFound();
            }

            userNode.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            ShareNode(userNode);

            return Accepted();
        }

        [HttpPost]
        [Route("hide")]
        public async Task<IActionResult> Hide([FromBody] UserNode userNode)
        {
            var owner = await GetUser(User);
            if (owner is null)
            {
                return Unauthorized();
            }

            var node = await _nodeService.Get(userNode.NodeId);
            if (node is null)
            {
                return NotFound();
            }

            if (node.OwnerId != owner.Id)
            {
                return Forbid();
            }

            var user = await _userService.Get(userNode.UserId);
            if (user is null)
            {
                return NotFound();
            }

            HideNode(userNode);

            return NoContent();
        }

        private async void ShareNode(UserNode userNode)
        {
            var allNodes = (await _nodeService.GetAll()).ToList();
            var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);
            var node = await _nodeService.Get(userNode.NodeId);

            var root = TreeExtensions.Find(virtualRoot, node);

            if (root != null)
            {
                await ShareChildren(root, userNode);

                var rootUserNode = new UserNode
                {
                    NodeId = root.Data.Id,
                    UserId = userNode.UserId,
                    AccessTypeId = userNode.AccessTypeId,
                    Timestamp = userNode.Timestamp
                };

                if (await _userNodeService.Create(rootUserNode) is null)
                {
                    await _userNodeService.Update(rootUserNode);
                }
            }
        }

        private async Task ShareChildren(TreeExtensions.ITree<Node> node, UserNode userNode)
        {
            foreach (var child in node.Children)
            {
                if (!child.IsLeaf)
                {
                    await ShareChildren(child, userNode);
                }

                var leafUserNode = new UserNode
                {
                    NodeId = child.Data.Id,
                    UserId = userNode.UserId,
                    AccessTypeId = userNode.AccessTypeId,
                    Timestamp = userNode.Timestamp
                };

                if (await _userNodeService.Create(leafUserNode) is null)
                {
                    await _userNodeService.Update(leafUserNode);
                }
            }
        }

        private async void HideNode(UserNode userNode)
        {
            var allNodes = (await _nodeService.GetAll()).ToList();
            var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);
            var node = await _nodeService.Get(userNode.NodeId);

            var root = TreeExtensions.Find(virtualRoot, node);

            if (root != null)
            {
                await HideChildren(root, userNode);

                await _userNodeService.Delete(new UserNode
                {
                    NodeId = root.Data.Id,
                    UserId = userNode.UserId
                });
            }
        }

        private async Task HideChildren(TreeExtensions.ITree<Node> node, UserNode userNode)
        {
            foreach (var child in node.Children)
            {
                if (!child.IsLeaf)
                {
                    await HideChildren(child, userNode);
                }

                await _userNodeService.Delete(new UserNode
                {
                    NodeId = child.Data.Id,
                    UserId = userNode.UserId
                });
            }
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

        private async Task<UserNodeResult> GetSharedNode(User user, string path, bool isPreLevelTree, bool differentRoots = false)
        {
            var userNodeResult = new UserNodeResult();

            if (user is null)
            {
                userNodeResult.ActionResult = Unauthorized();
                return userNodeResult;
            }

            var allUserNodes = (await _userNodeService.GetAll()).Where(x => x.UserId == user.Id).ToList();
            var allUserNodesIds = allUserNodes.Select(x => x.NodeId);
            var allNodes = (await _nodeService.GetAll()).Where(x => allUserNodesIds.Contains(x.Id)).ToList();
            var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);

            _pathBuilder.ParsePath(path);
            
            if (isPreLevelTree)
            {
                _pathBuilder.GetLastNode();
            }

            var levelTree = TreeExtensions.GetLevelTree(virtualRoot, _pathBuilder.GetPath(), differentRoots);

            if (levelTree is null)
            {
                userNodeResult.ActionResult = NotFound();
                return userNodeResult;
            }

            userNodeResult.LevelTree = levelTree;

            if (!isPreLevelTree)
            {
                userNodeResult.UserNode = allUserNodes.FirstOrDefault(x => x.NodeId == levelTree.Data.Id);
                return userNodeResult;
            }

            var node = levelTree.Children.FirstOrDefault(x => x.Data.Name == _pathBuilder.GetLastNode(path));

            if (node is null)
            {
                userNodeResult.ActionResult = NotFound();
                return userNodeResult;
            }

            userNodeResult.Node = node.Data;
            userNodeResult.UserNode = allUserNodes.FirstOrDefault(x => x.NodeId == node.Data.Id);

            return userNodeResult;
        }

        private class UserNodeResult
        {
            public IActionResult ActionResult { get; set; }
            public TreeExtensions.ITree<Node> LevelTree { get; set; }
            public Node Node { get; set; }
            public UserNode UserNode { get; set; }
        }
    }
}
