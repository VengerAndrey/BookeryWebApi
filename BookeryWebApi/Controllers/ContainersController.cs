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
        private readonly IDataRepository _dataRepository;

        public ContainersController(IBlobRepository blobRepository, IDataRepository dataRepository)
        {
            _blobRepository = blobRepository;
            _dataRepository = dataRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ListContainers()
        {
            var containers = await _dataRepository.ListContainersAsync();

            return Ok(containers);
        }

        [HttpPost]
        public async Task<IActionResult> AddContainer([FromBody] ContainerCreateDto containerCreateDto)
        {
            var container = new Container(containerCreateDto);
            var containerDbResult = await _dataRepository.AddContainerAsync(container);

            if (containerDbResult is null)
            {
                await _dataRepository.DeleteContainerAsync(container.Id);
                return Problem("Unable to create a container.");
            }

            return Created(Request.Scheme + "://" + Request.Host + Request.Path + container.Id, container);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteContainers()
        {
            var containers = await _dataRepository.DeleteContainersAsync();

            return Accepted(containers);
        }

        [HttpGet]
        [Route("{idContainer}")]
        public async Task<IActionResult> ListContainer(Guid idContainer)
        {
            var container = await _dataRepository.ListContainerAsync(idContainer);

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
            var container = await _dataRepository.DeleteContainerAsync(idContainer);

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
            var blobDtos = await _dataRepository.ListBlobsAsync(idContainer);

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
            var blob = new Blob(blobUploadDto, idContainer);

            var blobDtoRepositoryResult = await _blobRepository.AddBlobAsync(blob);
            var blobDtoDbResult = await _dataRepository.AddBlobAsync(new BlobDto(blob));

            if (blobDtoRepositoryResult is null || blobDtoDbResult is null)
            {
                await _blobRepository.DeleteBlobAsync(blob.Id);
                await _dataRepository.DeleteBlobAsync(blob.Id);
                return Problem("Unable to upload a blob.");
            }

            return Created(Request.Scheme + "://" + Request.Host + Request.Path + "/" + blobDtoRepositoryResult.Id, blobDtoRepositoryResult);
        }

        [HttpDelete]
        [Route("{idContainer}/blobs")]
        public async Task<IActionResult> DeleteBlobs(Guid idContainer)
        {
            var blobDtosDbResult = (await _dataRepository.DeleteBlobsAsync(idContainer)).ToList();
            var blobDtosRepositoryResult = new List<BlobDto>();

            foreach (var blobDto in blobDtosDbResult)
            {
                var deleted = await _blobRepository.DeleteBlobAsync(blobDto.Id);
                blobDtosRepositoryResult.Add(deleted);
            }

            return Accepted(blobDtosDbResult.Intersect(blobDtosRepositoryResult));
        }

        [HttpGet]
        [Route("{idContainer}/blobs/{idBlob}")]
        public async Task<IActionResult> ListBlob(Guid idContainer, Guid idBlob)
        {
            var blobDto = await _dataRepository.ListBlobAsync(idBlob);

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
            var blob = await _blobRepository.GetBlobAsync(idBlob);

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
            var blobDtoRepositoryResult = await _blobRepository.PutBlobAsync(idBlob, blobUploadDto);
            var blobDtoDbResult = await _dataRepository.PutBlobAsync(new BlobDto
                {Id = idBlob, Name = blobUploadDto.Name, IdContainer = idContainer});

            if (blobDtoRepositoryResult is null || blobDtoDbResult is null)
            {
                return NotFound();
            }

            if (!blobDtoRepositoryResult.Equals(blobDtoDbResult))
            {
                return Problem("The problem occurred while trying to put a blob.");
            }

            return Accepted(blobDtoRepositoryResult);
        }

        [HttpDelete]
        [Route("{idContainer}/blobs/{idBlob}")]
        public async Task<IActionResult> DeleteBlob(Guid idContainer, Guid idBlob)
        {
            var blobDtoRepositoryResult = await _blobRepository.DeleteBlobAsync(idBlob);
            var blobDtoDbResult = await _dataRepository.DeleteBlobAsync(idBlob);

            if (blobDtoRepositoryResult is null || blobDtoDbResult is null)
            {
                return NotFound();
            }

            if (!blobDtoRepositoryResult.Equals(blobDtoDbResult))
            {
                return Problem("The problem occurred while trying to delete a blob.");
            }

            return Accepted(blobDtoRepositoryResult);
        }
    }
}
