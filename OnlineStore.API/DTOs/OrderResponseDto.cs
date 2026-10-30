namespace OnlineStore.API.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}
