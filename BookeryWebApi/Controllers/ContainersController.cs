using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookeryWebApi.Dtos;
using BookeryWebApi.Entities;
using BookeryWebApi.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace BookeryWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContainersController : ControllerBase
    {
        private readonly IBlobRepository _blobRepository;
        private readonly IDataRepository _dataRepository;

        public ContainersController(IBlobRepository blobRepository, IDataRepository dataRepository)
        {
            _blobRepository = blobRepository;
            _dataRepository = dataRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ListContainers()
        {
            var containerEntities = await _dataRepository.ListContainersAsync();

            var containerDtos = new List<ContainerDto>();

            foreach (var containerEntity in containerEntities)
            {
                containerDtos.Add(new ContainerDto(containerEntity));
            }

            return Ok(containerDtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddContainer([FromBody] ContainerCreateDto containerCreateDto)
        {
            var containerDto = new ContainerDto(containerCreateDto, User.Identity?.Name ?? "null");
            var containerEntity = await _dataRepository.AddContainerAsync(containerDto.ToContainerEntity());

            if (containerEntity is null)
            {
                await _dataRepository.DeleteContainerAsync(containerDto.Id);
                return Problem("Unable to create a container.");
            }

            return Created(Request.Scheme + "://" + Request.Host + Request.Path + containerDto.Id, containerDto);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteContainers()
        {
            var containerEntities = await _dataRepository.DeleteContainersAsync();
            await _blobRepository.DeleteBlobsAsync();

            var containerDtos = new List<ContainerDto>();

            foreach (var containerEntity in containerEntities)
            {
                containerDtos.Add(new ContainerDto(containerEntity));
            }

            return Accepted(containerDtos);
        }

        [HttpGet]
        [Route("{idContainer}")]
        public async Task<IActionResult> ListContainer(Guid idContainer)
        {
            var containerEntity = await _dataRepository.ListContainerAsync(idContainer);

            if (containerEntity is null)
            {
                return NotFound();
            }

            return Ok(new ContainerDto(containerEntity));
        }

        [HttpDelete]
        [Route("{idContainer}")]
        public async Task<IActionResult> DeleteContainer(Guid idContainer)
        {
            var blobEntities = await _dataRepository.ListBlobsAsync(idContainer);

            foreach (var blobEntity in blobEntities)
            {
                await _blobRepository.DeleteBlobAsync(blobEntity.Id);
            }

            var containerEntity = await _dataRepository.DeleteContainerAsync(idContainer);

            if (containerEntity is null)
            {
                return NotFound();
            }

            return Accepted(new ContainerDto(containerEntity));
        }

        [HttpGet]
        [Route("{idContainer}/blobs")]
        public async Task<IActionResult> ListBlobs(Guid idContainer)
        {
            var blobEntities = await _dataRepository.ListBlobsAsync(idContainer);

            if (blobEntities is null)
            {
                return NotFound();
            }

            var blobInfoDtos = new List<BlobInfoDto>();

            foreach (var blobEntity in blobEntities)
            {
                blobInfoDtos.Add(new BlobInfoDto(blobEntity));
            }

            return Ok(blobInfoDtos);
        }

        [HttpPost]
        [Route("{idContainer}/blobs")]
        public async Task<IActionResult> AddBlob(Guid idContainer, [FromBody] BlobUploadDto blobUploadDto)
        {
            var blob = new BlobDto(blobUploadDto, idContainer);

            var repositoryResult = await _blobRepository.AddBlobAsync(blob);
            var dbResult = await _dataRepository.AddBlobAsync(blob.ToBlobEntity());

            if (repositoryResult is null || dbResult is null)
            {
                await _blobRepository.DeleteBlobAsync(blob.Id);
                await _dataRepository.DeleteBlobAsync(blob.Id);
                return Problem("Unable to upload a blob.");
            }

            return Created(Request.Scheme + "://" + Request.Host + Request.Path + "/" + repositoryResult.Id, repositoryResult);
        }

        [HttpDelete]
        [Route("{idContainer}/blobs")]
        public async Task<IActionResult> DeleteBlobs(Guid idContainer)
        {
            var dbResult = (await _dataRepository.DeleteBlobsAsync(idContainer)).Select(x => new BlobInfoDto(x)).ToList();
            var repositoryResult = new List<BlobInfoDto>();

            foreach (var blobDto in dbResult)
            {
                var deleted = await _blobRepository.DeleteBlobAsync(blobDto.Id);
                repositoryResult.Add(deleted);
            }

            return Accepted(dbResult.Intersect(repositoryResult));
        }

        [HttpGet]
        [Route("{idContainer}/blobs/{idBlob}")]
        public async Task<IActionResult> ListBlob(Guid idContainer, Guid idBlob)
        {
            var blobEntity = await _dataRepository.ListBlobAsync(idBlob);

            if (blobEntity is null)
            {
                return NotFound();
            }

            return Ok(new BlobInfoDto(blobEntity));
        }

        [HttpGet]
        [Route("{idContainer}/blobs/{idBlob}/download")]
        public async Task<IActionResult> GetBlob(Guid idContainer, Guid idBlob)
        {
            var blobDto = await _blobRepository.GetBlobAsync(idBlob);

            if (blobDto is null)
            {
                return NotFound();
            }

            return Ok(blobDto);
        }

        [HttpPut]
        [Route("{idContainer}/blobs/{idBlob}")]
        public async Task<IActionResult> PutBlob(Guid idContainer, Guid idBlob, [FromBody] BlobUploadDto blobUploadDto)
        {
            var repositoryResult = await _blobRepository.PutBlobAsync(idBlob, blobUploadDto);
            var dbResult = await _dataRepository.PutBlobAsync(new BlobEntity
            {
                Id = idBlob,
                Name = blobUploadDto.Name,
                IdContainer = idContainer
            });

            if (repositoryResult is null || dbResult is null)
            {
                return NotFound();
            }

            if (!repositoryResult.Equals(new BlobInfoDto(dbResult)))
            {
                return Problem("The problem occurred while trying to put a blob.");
            }

            return Accepted(repositoryResult);
        }

        [HttpDelete]
        [Route("{idContainer}/blobs/{idBlob}")]
        public async Task<IActionResult> DeleteBlob(Guid idContainer, Guid idBlob)
        {
            var repositoryResult = await _blobRepository.DeleteBlobAsync(idBlob);
            var dbResult = await _dataRepository.DeleteBlobAsync(idBlob);

            if (repositoryResult is null || dbResult is null)
            {
                return NotFound();
            }

            if (!repositoryResult.Equals(new BlobInfoDto(dbResult)))
            {
                return Problem("The problem occurred while trying to delete a blob.");
            }

            return Accepted(repositoryResult);
        }
    }
}
