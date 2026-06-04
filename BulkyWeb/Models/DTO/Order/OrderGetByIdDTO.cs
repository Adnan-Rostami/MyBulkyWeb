namespace BulkyWeb.Models.DTO.Order
{
    public class OrderGetByIdDTO
    {
        public int OrderID { get; set; }
        public string? CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public int? ShipVia { get; set; }
        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }

        public decimal TotalBeforeDiscount { get; set; }

        public decimal TotalAfterDiscount { get; set; }



        // public OrderCustomerShortDTO? Customer { get; set; }
        //public OrderEmployeeShortDTO? Employee { get; set; }
        public OrderShipperShortDTO? Shipper { get; set; }
        public List<OrderDetailDTO> Items { get; set; } = new();

    }
}
