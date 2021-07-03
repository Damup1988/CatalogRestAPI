using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult<IEnumerable<ReadItemDto>> GetItems()
        {
            return Ok(_repository.GetItems().Select(item => item.AsDto()));
        }

        [HttpGet("{id}")]
        public ActionResult<ReadItemDto> GetItem(Guid id)
        {
            var item = _repository.GetItem(id).AsDto();
            if (item == null) return NotFound();
            else return Ok(item);
        }
        
        [HttpPost]
        public ActionResult<ReadItemDto> CreateItem(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            
            _repository.CreateItem(item);
            return CreatedAtAction(nameof(GetItem), new {id = item.Id}, item.AsDto());
        }
        
        [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = _repository.GetItem(id);
            if (existingItem == null) return NotFound();
            Item updatedItem = existingItem with
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };
            
            _repository.UpdateItem(updatedItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id)
        {
            var existingItem = _repository.GetItem(id);
            if (existingItem == null) return NotFound();
            _repository.DeleteItem(id);
            return NoContent();
        }
    }
}