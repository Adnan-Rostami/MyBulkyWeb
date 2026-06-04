namespace BulkyWeb.Models.DTO.Product
{
    public class ProductFilterDTO
    {
        public int? ProductID { get; set; }
        public int? CategoryID { get; set; }
        public int? SupplierID { get; set; }
        public string? ProductName { get; set; }
        public int? UnitsInStock { get; set; }
        public bool? DisContinued { get; set; }
        public int? ReorderLevel { get; set; }
        public string? SortBy { get; set; } = "id_asc";
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 3;
        //public int? UnitPrice { get; set; }
        public decimal? MinUnitPrice { get; set; }
        public decimal? MaxUnitPrice { get; set; }

        public int? MinUnitsInStock { get; set; }
        public int? MaxUnitsInStock { get; set; }
    }
}
