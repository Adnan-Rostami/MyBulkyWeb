using BulkyWeb.Models.Identities;
using BulkyWeb.Models.Payments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BulkyWeb.Models.Orders
{

    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public string? CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? ShipVia { get; set; }
        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }

        //[ForeignKey("CustomerID")]
        //public Customer? Customer { get; set; }

        //[ForeignKey("EmployeeID")]
        //public Employee? Employee { get; set; }

        [ForeignKey("ShipVia")]
        public Shipper? Shipper { get; set; }

        public IList<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();


        public decimal TotalBeforeDiscount { get; set; }
        public decimal TotalAfterDiscount { get; set; }


        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }
        public OrderStatus Status { get; set; }

        public ICollection<Payment> Payments { get; set; }
    = new List<Payment>();



    }
}
