namespace BulkyWeb.Models.DTO.Order
{
    public class OrderGetAllDTO
    {

        public int OrderID { get; set; }
        public string? CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }

        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public decimal TotalAfterDiscount { get; set; }

    }
}
