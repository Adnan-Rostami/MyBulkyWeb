namespace BulkyWeb.Models.DTO.Category
{
    public class CategoryCreateDTO
    {
        //    [Required]
        public string CategoryName { get; set; }

        public string? Description { get; set; }

        public byte[]? Picture { get; set; }
    }
}
