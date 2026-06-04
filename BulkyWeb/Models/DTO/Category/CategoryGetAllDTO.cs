namespace BulkyWeb.Models.DTO.Category
{
    public class CategoryGetAllDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public byte[]? Picture { get; set; }
    }
    public class CategorySearchResultDTO
    {
        public List<CategoryGetAllDTO> Items { get; set; } = new();
        public long TotalCount { get; set; }
    }
}
