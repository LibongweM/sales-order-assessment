using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreTest.Models
{
    public class OrderHeader
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string OrderNumber { get; set; } = string.Empty;

        public OrderType OrderType { get; set; }

        public OrderStatus OrderStatus { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public DateTime? OrderCreated { get; set; }

        public bool Archived { get; set; } = false;

        public List<OrderLine>? OrderLines { get; set; }
    }

    public enum OrderType
    {
        Normal,
        Staff,
        Mechanical,
        Perishable
    }

    public enum OrderStatus
    {
        New,
        InProgress,
        Complete,
        Dispatched,
    }
    
   
}
