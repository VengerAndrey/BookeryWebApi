using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookeryWebApi.Models;
using BookeryWebApi.Repositories;

namespace BookeryWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainersController : ControllerBase
    {
        private readonly IBlobRepository _blobRepository;

        public ContainersController(IBlobRepository blobRepository)
        {
            _blobRepository = blobRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ListContainers()
        {
            var containers = await _blobRepository.ListContainersAsync();
            return Ok(containers);
        }

        [HttpPost]
        public async Task<IActionResult> AddContainer([FromBody] ContainerCreateDto containerCreateDto)
        {
            var container = await _blobRepository.AddContainerAsync(containerCreateDto);

            if (container is null)
                return Problem("Unable to create a container.");

            return Created(Request.Scheme + "://" + Request.Host + Request.Path + container.Id, container);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteContainers()
        {
            var containers = await _blobRepository.DeleteContainersAsync();
            return Accepted(containers);
        }

        [HttpGet]
        [Route("{idContainer}")]
        public async Task<IActionResult> ListContainer(Guid idContainer)
        {
            var container = await _blobRepository.ListContainerAsync(idContainer);

            if (container is null)
            {
                return NotFound();
            }

            return Ok(container);
        }

        [HttpDelete]
        [Route("{idContainer}")]
        public async Task<IActionResult> DeleteContainer(Guid idContainer)
        {
            var container = await _blobRepository.DeleteContainerAsync(idContainer);

            if (container is null)
            {
                return NotFound();
            }

            return Accepted(container);
        }
    }
}
