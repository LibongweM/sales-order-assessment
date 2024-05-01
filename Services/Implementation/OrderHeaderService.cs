using CoreTest.Components.Pages;
using CoreTest.Data;
using CoreTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreTest.Services.Implementation
{
    public class OrderHeaderService : IOrderHeaderService
    {
        private readonly ApplicationDbContext _context;
        public OrderHeaderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderHeader>> GetAllOrderHeadersAsync()
        {
            var result = await _context.OrderHeaders.Include(x =>x.OrderLines).ToListAsync();
            return result;
        }
        
        public async Task<List<OrderHeader>> GetAllOrderHeadersWithoutLinesAsync()
        {
            return await _context.OrderHeaders.ToListAsync();
        }

        public async Task<OrderHeader> GetOrderHeaderIdAsync(string id)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            return orderHeader;
        }

        public async Task AddOrderHeaderAsync(OrderHeader orderHeader)
        {
            
            _context.OrderHeaders.Add(orderHeader);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderHeaderAsync(OrderHeader orderHeader)
        {
            var _orderHeader = await _context.OrderHeaders.FirstOrDefaultAsync(x=>x.Id == orderHeader.Id);
            if (_orderHeader != null)
            {
                _orderHeader.OrderNumber = orderHeader.OrderNumber;
                _orderHeader.OrderStatus = orderHeader.OrderStatus;
                _orderHeader.CustomerName = orderHeader.CustomerName;
                _orderHeader.OrderType = orderHeader.OrderType;
                _orderHeader.OrderCreated = orderHeader.OrderCreated;
                
                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteOrderHeaderAsync(string id)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            if (orderHeader != null)
            {
                _context.OrderHeaders.Remove(orderHeader);
                await _context.SaveChangesAsync();
            }
        }
    }
}
