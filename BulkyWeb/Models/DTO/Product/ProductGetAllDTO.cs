namespace BulkyWeb.Models.DTO.Product
{
    public class ProductGetAllDTO
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public string? CategoryName { get; set; }
        public int? SupplierID { get; set; }
        public int? CategoryID { get; set; }
        public string? QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        // public int? Status { get; set; }    }
        // public CategoryShortInAllDTO? Category { get; set; }

    }

    public class CategoryShortInAllDTO
    {
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
    }

    public class ProductSearchResultDTO
    {
        public List<ProductGetAllDTO> Items { get; set; } = new();


        public long TotalCount { get; set; }
    }

    //public class ProductElasticGetAllDTO
    //{
    //    public int ProductID { get; set; }
    //    public string? ProductName { get; set; }
    //    public string? CategoryName { get; set; }
    //    public int? SupplierID { get; set; }
    //    public int? CategoryID { get; set; }
    //    public string? QuantityPerUnit { get; set; }
    //    public decimal? UnitPrice { get; set; }
    //    public int? UnitsInStock { get; set; }
    //    public short? UnitsOnOrder { get; set; }
    //    public short? ReorderLevel { get; set; }
    //    public bool Discontinued { get; set; }
    //    // public int? Status { get; set; }    }
    //    public CategoryShortInAllDTO? Category { get; set; }

    //}
}
