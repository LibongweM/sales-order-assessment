using CoreTest.Models;

namespace CoreTest.Services
{
    public interface IOrderLineService
    {
        Task CreateOrderLineAsync(string id, OrderLine orderLine);
        Task <List<OrderLine>> GetOrderLineByHeaderAsync(string headerId);
        Task UpdateOrderLineAsync(OrderLine orderLine);
        Task DeleteOrderLineAsync(string id);
        
      
    }
}
