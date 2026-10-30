using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Core.Entities;

namespace OnlineStore.Core.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync();

        Task<Order?> GetByIdAsync(int id);

        Task<Order> CreateAsync(
            int customerId,
            List<OrderItem> items);

        Task<bool> UpdateStatusAsync(int id, string status);

        Task<bool> DeleteAsync(int id);
    }
}