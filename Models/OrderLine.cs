using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoreTest.Models
{
    public class OrderLine
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int LineNumber { get; set; } = 1;

        [Required]
        public string ProductCode { get; set; } = string.Empty;

        public ProductType ProductType { get; set; }

        [Required]
        public double ProductCostPrice { get; set; }

        [Required]
        public double ProductSalesPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        public bool Archived { get; set; } = false;

        public OrderHeader OrderHeader { get; set; }
    }

    public enum ProductType
    {
        Apparel,
        Parts,
        Equipment,
        Motor
    }
}
