namespace BulkyWeb.Models.DTO.Product.ProductElasticDocument
{
    public class ProductElasticDocument
    {
        public int productID { get; set; }
        public string productName { get; set; } = default!;
        public int? categoryID { get; set; }

        public string? categoryName { get; set; }

        public string? quantityPerUnit { get; set; }
        public decimal? unitPrice { get; set; }
        public int? unitsInStock { get; set; }
        public short? unitsOnOrder { get; set; }
        public short? reorderLevel { get; set; }
        public bool discontinued { get; set; }
    }
}
