namespace BulkyWeb.Models.DTO.Order
{
    public class OrderUpdateDTO
    {
        // public DateTime? RequiredDate { get; set; }
        public int? ShipVia { get; set; }
        //public int? ProductID { get; set; }
        public decimal? Freight { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        // public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        // public DateTime? OrderDate { get; set; }
        public List<OrderDetailDTO>? Items { get; set; }
    }
}
