using Microsoft.AspNetCore.Mvc;
using OnlineStore.API.DTOs;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderResponseDto>>> GetAll()
        {
            var orders = await _orderService.GetAllAsync();

            return Ok(orders.Select(ToResponseDto).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderResponseDto>> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
                return NotFound("Order not found.");

            return Ok(ToResponseDto(order));
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> Create(
            [FromBody] OrderCreateDto dto)
        {
            var items = dto.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();

            try
            {
                var order = await _orderService.CreateAsync(
                    dto.CustomerId, items);

                var savedOrder = await _orderService.GetByIdAsync(order.Id);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = order.Id },
                    ToResponseDto(savedOrder ?? order));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id, [FromBody] OrderStatusUpdateDto dto)
        {
            try
            {
                var updated = await _orderService.UpdateStatusAsync(
                    id, dto.Status);

                if (!updated)
                    return NotFound("Order not found.");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _orderService.DeleteAsync(id);

            if (!deleted)
                return NotFound("Order not found.");

            return NoContent();
        }

        private static OrderResponseDto ToResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }
    }
}
