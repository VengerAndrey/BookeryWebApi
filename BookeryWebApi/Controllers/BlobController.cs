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
    public class BlobController : ControllerBase
    {
        private readonly IBlobRepository _blobRepository;
        private readonly IDataRepository _dataRepository;

        public BlobController(IBlobRepository blobRepository, IDataRepository dataRepository)
        {
            _blobRepository = blobRepository;
            _dataRepository = dataRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetContainers()
        {
            var containers = await _blobRepository.ListContainersAsync();
            return Ok(containers);
        }

        [HttpGet]
        [Route("{idContainer}")]
        public async Task<IActionResult> GetBlobs(Guid idContainer)
        {
            var blobDtos = await _blobRepository.ListBlobsAsync(idContainer);
            return Ok(blobDtos);
        }

        [HttpGet]
        [Route("{idContainer}/{idBlob}")]
        public async Task<IActionResult> DownloadBlob(Guid idContainer, Guid idBlob)
        {
            var blob = await _blobRepository.GetBlobAsync(new BlobDownloadDto {Id = idBlob, IdContainer = idContainer});
            Response.Headers.Add("name", blob.Name);
            return File(blob.Content, "application/octet-stream");
        }

        [HttpPost]
        public async Task<IActionResult> AddContainer([FromBody] ContainerCreateDto containerCreateDto)
        {
            var container = await _blobRepository.AddContainerAsync(containerCreateDto);

            if (container is null)
                return Problem("Container is already exists.");

            container = await _dataRepository.AddContainerAsync(container);

            if (container is null)
                return Problem("Enable to create a container.");

            return Created(Request.Scheme + "://" + Request.Host + Request.Path + container.Id, container);
        }
    }
}
