using BulkyWeb.Models.Orders;
using System.ComponentModel.DataAnnotations.Schema;
namespace BulkyWeb.Models
{
    public class OrderDetail
    {
        [ForeignKey("OrderID")]

        public int OrderID { get; set; }

        [ForeignKey("ProductID")]
        public int ProductID { get; set; }

        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public decimal Discount { get; set; }

        public decimal TotalBeforeDiscount { get; set; }
        public decimal TotalAfterDiscount { get; set; }

        public Order? Order { get; set; }
        public Product? Product { get; set; }

    }
}








