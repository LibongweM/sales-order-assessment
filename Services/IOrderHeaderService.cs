using CoreTest.Models;

namespace CoreTest.Services
{
    public interface IOrderHeaderService
    {
        Task<List<OrderHeader>> GetAllOrderHeadersAsync();
        Task<List<OrderHeader>> GetAllOrderHeadersWithoutLinesAsync();
        Task<OrderHeader> GetOrderHeaderIdAsync(string id);
        Task AddOrderHeaderAsync(OrderHeader orderHeader);
        Task UpdateOrderHeaderAsync(OrderHeader orderHeader);
        Task DeleteOrderHeaderAsync( string id);
    }
}
