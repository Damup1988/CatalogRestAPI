using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Domain;
using Catalog.Dto;
using Catalog.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepository _repository;

        public ItemController(IItemRepository itemRepository)
        {
            _repository = itemRepository;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadItemDto>>> GetItemsAsync()
        {
            return Ok((await _repository.GetItemsAsync())
                .Select(item => item.AsDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadItemDto>> GetItemAsync(Guid id)
        {
            var item = (await _repository.GetItemAsync(id)).AsDto();
            if (item == null) return NotFound();
            else return Ok(item);
        }
        
        [HttpPost]
        public async Task<ActionResult<ReadItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            
            await _repository.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemAsync), new {id = item.Id}, item.AsDto());
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = await _repository.GetItemAsync(id);
            if (existingItem == null) return NotFound();
            Item updatedItem = existingItem with
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };
            
            await _repository.UpdateItemAsync(updatedItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = await _repository.GetItemAsync(id);
            if (existingItem == null) return NotFound();
            await _repository.DeleteItemAsync(id);
            return NoContent();
        }
    }
}