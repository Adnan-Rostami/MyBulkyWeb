namespace BulkyWeb.Models
{
    public class Category
    {
        //Old
        // [Key]


        public int CategoryID { get; set; }
        //public string CategoryName { get; set; }
        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }
        public byte[]? Picture { get; set; }

        public ICollection<Product>? Products { get; set; }







    }




}
