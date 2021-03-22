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

        public BlobController(IBlobRepository blobRepository)
        {
            _blobRepository = blobRepository;
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
            var blobs = await _blobRepository.ListBlobsAsync(idContainer);
            return Ok(blobs);
        }
    }
}
