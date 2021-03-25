using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BookeryWebApi.Models;
using BookeryWebApi.Repositories;

namespace BookeryWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainersController : ControllerBase
    {
        private readonly IContainerRepository _containerRepository;
        private readonly IBlobRepository _blobRepository;

        public ContainersController(IContainerRepository containerRepository, IBlobRepository blobRepository)
        {
            _containerRepository = containerRepository;
            _blobRepository = blobRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ListContainers()
        {
            var containers = await _containerRepository.ListContainersAsync();

            return Ok(containers);
        }

        [HttpPost]
        public async Task<IActionResult> AddContainer([FromBody] ContainerCreateDto containerCreateDto)
        {
            var container = await _containerRepository.AddContainerAsync(containerCreateDto);

            if (container is null)
            {
                return Problem("Unable to create a container.");
            }

            return Created(Request.Scheme + "://" + Request.Host + Request.Path + container.Id, container);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteContainers()
        {
            var containers = await _containerRepository.DeleteContainersAsync();

            return Accepted(containers);
        }

        [HttpGet]
        [Route("{idContainer}")]
        public async Task<IActionResult> ListContainer(Guid idContainer)
        {
            var container = await _containerRepository.ListContainerAsync(idContainer);

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
            var container = await _containerRepository.DeleteContainerAsync(idContainer);

            if (container is null)
            {
                return NotFound();
            }

            return Accepted(container);
        }

        [HttpGet]
        [Route("{idContainer}/blobs")]
        public async Task<IActionResult> ListBlobs(Guid idContainer)
        {
            var blobDtos = await _blobRepository.ListBlobsAsync(idContainer);

            if (blobDtos is null)
            {
                return NotFound();
            }

            return Ok(blobDtos);
        }

        [HttpPost]
        [Route("{idContainer}/blobs")]
        public async Task<IActionResult> AddBlob(Guid idContainer, [FromBody] BlobUploadDto blobUploadDto)
        {
            var blobDto = await _blobRepository.AddBlobAsync(idContainer, blobUploadDto);

            if (blobDto is null)
            {
                return Problem("Unable to upload a blob.");
            }

            return Created(Request.Scheme + "://" + Request.Host + Request.Path + "/" + blobDto.Id, blobDto);
        }

        [HttpDelete]
        [Route("{idContainer}/blobs")]
        public async Task<IActionResult> DeleteBlobs(Guid idContainer)
        {
            var blobDtos = await _blobRepository.DeleteBlobsAsync(idContainer);

            return Accepted(blobDtos);
        }

        [HttpGet]
        [Route("{idContainer}/blobs/{idBlob}")]
        public async Task<IActionResult> ListBlob(Guid idContainer, Guid idBlob)
        {
            var blobDto = await _blobRepository.ListBlobAsync(idContainer, idBlob);

            if (blobDto is null)
            {
                return NotFound();
            }

            return Ok(blobDto);
        }

        [HttpGet]
        [Route("{idContainer}/blobs/{idBlob}/download")]
        public async Task<IActionResult> GetBlob(Guid idContainer, Guid idBlob)
        {
            var blob = await _blobRepository.GetBlobAsync(idContainer, idBlob);

            if (blob is null)
            {
                return NotFound();
            }

            return Ok(blob);
        }

        [HttpPut]
        [Route("{idContainer}/blobs/{idBlob}")]
        public async Task<IActionResult> PutBlob(Guid idContainer, Guid idBlob, [FromBody] BlobUploadDto blobUploadDto)
        {
            var blobDto = await _blobRepository.PutBlobAsync(idContainer, idBlob, blobUploadDto);

            if (blobDto is null)
            {
                return NotFound();
            }

            return Accepted(blobDto);
        }

        [HttpDelete]
        [Route("{idContainer}/blobs/{idBlob}")]
        public async Task<IActionResult> DeleteBlob(Guid idContainer, Guid idBlob)
        {
            var blobDto = await _blobRepository.DeleteBlobAsync(idContainer, idBlob);

            if (blobDto is null)
            {
                return NotFound();
            }

            return Accepted(blobDto);
        }
    }
}
