namespace BulkyWeb.Models.DTO.Category
{
    public class CategoryFilterDTO
    {
        public int? CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public byte[]? Picture { get; set; }
        public string? SortBy { get; set; } = "id_asc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 3;
    }
}
