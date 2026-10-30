using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;
using OnlineStore.Infrastructure.Data;

namespace OnlineStore.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OnlineStoreDbContext _context;

        public OrderService(
            IOrderRepository orderRepository,
            OnlineStoreDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<Order> CreateAsync(
            int customerId,
            List<OrderItem> items)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException(
                    "Order must contain at least one product.");

            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == customerId);

            if (!customerExists)
                throw new ArgumentException(
                    "Customer not found.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable);

            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                Items = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            foreach (var item in items)
            {
                if (item.Quantity <= 0)
                    throw new ArgumentException(
                        "Quantity must be greater than zero.");

                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                    throw new ArgumentException(
                        $"Product {item.ProductId} not found.");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException(
                        $"Not enough stock for product {product.Name}.");

                product.StockQuantity -= item.Quantity;

                var unitPrice = product.Price;

                totalAmount += unitPrice * item.Quantity;

                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice
                });
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return order;
        }


        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var allowedStatuses = new[]
            {
        "Pending", "Processing", "Shipped",
        "Delivered", "Cancelled"
    };

            if (!allowedStatuses.Contains(status))
                throw new ArgumentException("Invalid order status.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable);

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return false;

            if (order.Status == "Cancelled")
                throw new ArgumentException(
                    "Cancelled orders cannot change status.");

            if (order.Status == "Delivered")
                throw new ArgumentException(
                    "Delivered orders cannot change status.");

            if (status == "Cancelled")
            {
                foreach (var item in order.Items)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product != null)
                        product.StockQuantity += item.Quantity;
                }
            }

            order.Status = status;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }



        public async Task<bool> DeleteAsync(int id)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return false;

            if (order.Status != "Cancelled")
            {
                foreach (var item in order.Items)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product != null)
                        product.StockQuantity += item.Quantity;
                }
            }

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

    }
}
