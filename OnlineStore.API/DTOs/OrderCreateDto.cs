using System.ComponentModel.DataAnnotations;

namespace OnlineStore.API.DTOs
{
    public class OrderCreateDto
    {
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        [Required]
        [MinLength(1)]
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }
}

