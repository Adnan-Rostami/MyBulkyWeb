//namespace BulkyWeb.Models.DTO.Product
//{
//    public class ProductDTO
//    {

//        //public class ProductGetAllDTO
//        //{
//        //    public int ProductID { get; set; }
//        //    public string ProductName { get; set; }
//        //    public int? SupplierID { get; set; }
//        //    public int? CategoryID { get; set; }
//        //    public string? QuantityPerUnit { get; set; }
//        //    public decimal? UnitPrice { get; set; }
//        //    public short? UnitsInStock { get; set; }
//        //    public short? UnitsOnOrder { get; set; }
//        //    public short? ReorderLevel { get; set; }
//        //    public bool Discontinued { get; set; }
//        //    // public int? Status { get; set; }


//        ////}
//        //public class ProductGetByIdDTO
//        //{
//        //    // [Required]
//        //    public int ProductID { get; set; }
//        //    public string ProductName { get; set; }

//        //    public int? SupplierID { get; set; }
//        //    public int? CategoryID { get; set; }

//        //    public string? QuantityPerUnit { get; set; }
//        //    public decimal? UnitPrice { get; set; }
//        //    public short? UnitsInStock { get; set; }
//        //    public short? UnitsOnOrder { get; set; }
//        //    public short? ReorderLevel { get; set; }
//        //    public bool Discontinued { get; set; }
//        //    // public int? Status { get; set; }

//        //    public SupplierShortDTO Supplier { get; set; }
//        //    public CategoryShortDTO Category { get; set; }
//        ////}
//        //public class ProductCreateDTO
//        //{
//        //    [Required]
//        //    public string ProductName { get; set; }

//        //    public int? SupplierID { get; set; }
//        //    [Required]
//        //    public int CategoryID { get; set; }

//        //    public string? QuantityPerUnit { get; set; }
//        //    [Required]

//        //    public decimal UnitPrice { get; set; }

//        //    public short UnitsInStock { get; set; } = 0;


//        //    public short? UnitsOnOrder { get; set; }

//        //    public short? ReorderLevel { get; set; }
//        //    public bool Discontinued { get; set; } = false;
//        //    //    public int? Status { get; set; } = null;

//        //}
//        //public class ProductUpdateDTO
//        //{

//        //    public string? ProductName { get; set; }

//        //    public int? SupplierID { get; set; }
//        //    public int? CategoryID { get; set; }
//        //    public string? QuantityPerUnit { get; set; }
//        //    public decimal? UnitPrice { get; set; }
//        //    public short? UnitsInStock { get; set; }
//        //    public short? UnitsOnOrder { get; set; }
//        //    public short? ReorderLevel { get; set; }
//        //    public bool? Discontinued { get; set; }
//        //    //    public int? Status { get; set; }


//        //}
//        //public class SupplierShortDTO
//        //{

//        //    public string? CompanyName { get; set; }
//        //    public string? ContactName { get; set; }
//        //}
//        //public class CategoryShortDTO
//        //{

//        //    public string? CategoryName { get; set; }
//        //    public string? Description { get; set; }
//        //}

//        public class ProductFilterDTO
//        {
//            public int? ProductID { get; set; }
//            public int? CategoryID { get; set; }
//            public int? SupplierID { get; set; }
//            public string? ProductName { get; set; }
//            public int? UnitsInStock { get; set; }
//            public bool? DisContinued { get; set; }
//            public int? ReorderLevel { get; set; }
//            public string? SortBy { get; set; } = "id_asc";
//            public int? PageNumber { get; set; } = 1;
//            public int? PageSize { get; set; } = 3;
//            //public int? UnitPrice { get; set; }
//            public decimal? MinUnitPrice { get; set; }
//            public decimal? MaxUnitPrice { get; set; }

//            public int? MinUnitsInStock { get; set; }
//            public int? MaxUnitsInStock { get; set; }
//        }
//    }

//}
