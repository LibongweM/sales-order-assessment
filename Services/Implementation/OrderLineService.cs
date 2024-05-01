using CoreTest.Data;
using CoreTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreTest.Services.Implementation
{
    public class OrderLineService : IOrderLineService
    {
        private readonly ApplicationDbContext _context;
        public OrderLineService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<OrderLine>> GetAllOrderLineAsync()
        {
            var result = await _context.OrderLines.ToListAsync();
            return result;
        }
        
        public async Task CreateOrderLineAsync(string id, OrderLine orderLine)
        {
            OrderHeader orderHeader = _context.OrderHeaders.Where(oH => oH.Id == id).Include(oH => oH.OrderLines).FirstOrDefault();
            if (orderHeader != null && orderHeader.Archived == false)
            {
                int totalLines = orderHeader.OrderLines.Count;
                var lineNumber = GetOrderLineNumber(totalLines);
                
                var newOrderLine = new OrderLine
                {
                    ProductCode = orderLine.ProductCode,
                    ProductType = orderLine.ProductType,
                    ProductCostPrice = orderLine.ProductCostPrice,
                    ProductSalesPrice = orderLine.ProductSalesPrice,
                    Quantity = orderLine.Quantity,
                    LineNumber = lineNumber,
                    OrderHeader = orderHeader
                };

                _context.Add(newOrderLine);
                await _context.SaveChangesAsync();
            }
        }
        //
        // public Task GetOrderLineByHeaderAsync(string headerId)
        // {
        //     
        // }

        public async Task<List<OrderLine>> GetOrderLineByHeaderAsync(string headerId)
        {
            var orderHeader = await _context.OrderHeaders
                .Include(oH => oH.OrderLines)
                .FirstOrDefaultAsync(oH => oH.Id == headerId);

            return orderHeader?.OrderLines ?? new List<OrderLine>();
        }
        public async Task UpdateOrderLineAsync(OrderLine orderLine)
        {
            var _orderLine = await _context.OrderLines.FirstOrDefaultAsync(x=>x.Id ==orderLine.Id);
            if (_orderLine != null)
            {
                _orderLine.ProductCode = orderLine.ProductCode;
                _orderLine.ProductType = orderLine.ProductType;
                _orderLine.ProductCostPrice = orderLine.ProductCostPrice;
                _orderLine.ProductSalesPrice = orderLine.ProductSalesPrice;
                _orderLine.Quantity = orderLine.Quantity;
                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteOrderLineAsync(string id)
        {
            var orderLine = _context.OrderLines.Where(x => x.Id == id).Include(x => x.OrderHeader).FirstOrDefault();
          
            _context.OrderLines.Remove(orderLine);
            await _context.SaveChangesAsync();
        }
        
        private static int GetOrderLineNumber(int currentLines)
        {
            int lineNumber = 1;

            if (currentLines > 0)
            {
                lineNumber = currentLines + 1;

                return lineNumber;
            }

            return lineNumber;
        }
    }
}
