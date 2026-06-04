namespace BulkyWeb.Models.DTO.Product
{
    public class ProductGetByIdDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public int? SupplierID { get; set; }
        //public int? CategoryID { get; set; }
        public string? CategoryName { get; set; } = default!;

        public string? QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        // public int? Status { get; set; }

        public SupplierShortDTO Supplier { get; set; }
        public CategoryShortDTO Category { get; set; }
    }
}
