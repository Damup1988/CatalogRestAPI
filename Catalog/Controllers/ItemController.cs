using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Domain;
using Catalog.Dto;
using Catalog.Repository;
using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepository _repository;
        private readonly ILogger<ItemController> _logger;

        public ItemController(IItemRepository itemRepository, ILogger<ItemController> logger)
        {
            _repository = itemRepository;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IEnumerable<ReadItemDto>> GetItemsAsync()
        {
            var items = (await _repository.GetItemsAsync()).Select(item => item.AsDto());
            
            _logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Retrieved {items.Count()} items");
            
            //return Ok(items);
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadItemDto>> GetItemAsync(Guid id)
        {
            //var item = (await _repository.GetItemAsync(id)).AsDto();
            //if (item == null) return NotFound();
            //else return Ok(item);
            var item = await _repository.GetItemAsync(id);
            if (item == null) return NotFound();
            else return item.AsDto();
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