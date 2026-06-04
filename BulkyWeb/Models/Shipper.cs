using BulkyWeb.Models.Orders;

public class Shipper
{
    public int ShipperID { get; set; }
    public string CompanyName { get; set; }
    public string? Phone { get; set; }

    public ICollection<Order>? Orders { get; set; }
}
