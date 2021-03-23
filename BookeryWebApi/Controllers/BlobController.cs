﻿using Microsoft.AspNetCore.Http;
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

            if (container == null)
                return Problem("Container is already exists.");

            return Created(
                HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path + "/" +
                container.Id, container);
        }
    }
}
