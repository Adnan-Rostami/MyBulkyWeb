namespace BulkyWeb.Models.DTO.Category.Elastic
{
    public class CategoryElasticDocument
    {
        public int categoryID { get; set; }
        public string categoryName { get; set; } = default!;
        public string? description { get; set; }
        public byte[]? picture { get; set; }
    }
}
