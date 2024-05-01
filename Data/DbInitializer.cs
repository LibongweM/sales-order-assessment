using CoreTest.Models;

namespace CoreTest.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if(context.OrderHeaders.Any() || context.OrderLines.Any())
        {
            return;
        }

        var orderHeaders = new[]
        {
            new OrderHeader
            {
                CustomerName = "Adams",
                OrderCreated = DateTime.Now,
                OrderNumber = "SO625144",
                OrderStatus = OrderStatus.New,
                OrderType = OrderType.Normal
            },
            new OrderHeader
            {
                CustomerName = "Smith",
                OrderCreated = DateTime.Now,
                OrderNumber = "SO625145",
                OrderStatus = OrderStatus.New,
                OrderType = OrderType.Normal
            }
        };

        foreach (var oh in orderHeaders)
        {
            context.OrderHeaders.Add(oh);
        }

        var orderLines = new[]
        {
            new OrderLine
            {
                LineNumber = 1,
                ProductCode = "PC1001",
                ProductType = ProductType.Apparel,
                ProductCostPrice = 100.0,
                ProductSalesPrice = 150.0,
                Quantity = 10,
                OrderHeader = orderHeaders[0] 
            },
            new OrderLine
            {
                LineNumber = 2,
                ProductCode = "PC1002",
                ProductType = ProductType.Parts,
                ProductCostPrice = 50.0,
                ProductSalesPrice = 75.0,
                Quantity = 5,
                OrderHeader = orderHeaders[0] 
            }
        };

        foreach (var ol in orderLines)
        {
            context.OrderLines.Add(ol);
        }
   
        context.SaveChanges();
    }
}

  
