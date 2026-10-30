using System.ComponentModel.DataAnnotations;

namespace OnlineStore.API.DTOs
{
    public class OrderItemCreateDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
