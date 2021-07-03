using System.ComponentModel.DataAnnotations;

namespace Catalog.Dto
{
    public record CreateItemDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; init; }
        [Required]
        [Range(1, 1000)]
        public decimal Price { get; init; }
    }
}