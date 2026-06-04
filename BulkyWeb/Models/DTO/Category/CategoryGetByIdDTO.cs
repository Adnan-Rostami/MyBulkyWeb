namespace BulkyWeb.Models.DTO.Category
{
    public class CategoryGetByIdDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public byte[]? Picture { get; set; }
        public ICollection<CategoryProductDTO> Products { get; set; } = new List<CategoryProductDTO>();
    }
}
