using Catalog.Domain;
using Catalog.Dto;

namespace Catalog
{
    public static class Extensions
    {
        public static ReadItemDto AsDto(this Item item)
        {
            return new ReadItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                CreatedDate = item.CreatedDate
            };
        }
    }
}