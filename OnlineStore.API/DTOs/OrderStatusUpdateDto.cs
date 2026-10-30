using System.ComponentModel.DataAnnotations;

namespace OnlineStore.API.DTOs
{
    public class OrderStatusUpdateDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
