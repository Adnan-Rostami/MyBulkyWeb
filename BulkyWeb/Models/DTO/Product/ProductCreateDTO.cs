namespace BulkyWeb.Models.DTO.Product
{
    public class ProductCreateDTO
    {
        // [Required]
        public string ProductName { get; set; }

        public int? SupplierID { get; set; }
        // [Required]
        public int CategoryID { get; set; }

        public string? QuantityPerUnit { get; set; }
        // [Required]

        public decimal UnitPrice { get; set; }

        public int UnitsInStock { get; set; } = 0;


        public short? UnitsOnOrder { get; set; }

        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; } = false;
        //    public int? Status { get; set; } = null;    }
    }
}