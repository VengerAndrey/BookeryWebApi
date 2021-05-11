using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShares()
        {
            var shares = await _storageService.GetAllShares();

            return Ok(shares);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShare([FromBody] string name)
        {
            var result = await _storageService.CreateShare(new Share {Id = Guid.Empty, Name = name});

            if (result is null)
            {
                return Problem("Can't create a share.");
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateShare([FromBody] Share share)
        {
            var result = await _storageService.UpdateShare(share);

            if (result is null)
            {
                return Problem("Can't update a share.");
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetShare(Guid id)
        {
            var share = await _storageService.GetShare(id);

            if (share is null)
            {
                return NotFound();
            }

            return Ok(share);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteShare(Guid id)
        {
            var result = await _storageService.DeleteShare(id);

            if (result)
            {
                return Ok();
            }

            return Problem("Can't delete a share.");
        }

        [HttpGet]
        [Route("getSubItems")]
        public async Task<IActionResult> GetSubItems([FromBody] string path)
        {
            var items = await _storageService.GetSubItems(path);

            if (items is null)
            {
                return NotFound();
            }

            return Ok(items);
        }

        [HttpPost]
        [Route("createItem")]
        public async Task<IActionResult> CreateItem([FromBody] Item item)
        {
            var created = await _storageService.CreateItem(item);

            if (created is null)
            {
                return Problem("Can't create an item.");
            }

            return Created(created.Path, created);
        }
    }
}
