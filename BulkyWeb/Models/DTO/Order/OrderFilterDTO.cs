namespace BulkyWeb.Models.DTO.Order
{
    public class OrderFilterDTO
    {
        public int? OrderID { get; set; }
        public String? CustomerId { get; set; }

        public string? SortBy { get; set; } = "id_asc";
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 3;
    }
}
