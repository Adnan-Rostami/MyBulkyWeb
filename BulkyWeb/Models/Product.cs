//using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace BulkyWeb.Models
//{
//    public class Product
//    {
//        public int Id { get; set; }
//        [Required]
//        public string Title { get; set; }
//        public string Description { get; set; }
//        [Required]
//        public string ISBN { get; set; }
//        [Required]
//        public string Author { get; set; }
//        [Required]
//        [Range(1, 10000)]
//        //[Display(Name = "List Price")]
//        public double ListPrice { get; set; }
//        [Required]
//        [Range(1, 10000)]
//        //[Display(Name = "Price for 1-50")]
//        public double Price { get; set; }

//        [Required]
//        [Range(1, 10000)]
//        //[Display(Name = "Price for 51-100")]
//        public double Price50 { get; set; }

//        [Required]
//        [Display(Name = "Price for 100+")]
//        [Range(1, 10000)]
//        public double Price100 { get; set; }
//        //[ValidateNever]
//        //public string ImageUrl { get; set; }

//        //[Required]
//        //[Display(Name = "Category")]
//        public int Category_Id { get; set; }
//        [ForeignKey("Category_Id")]
//        //[ValidateNever]
//        public Category Category { get; set; }
//        public string? imageURL  { get; set; }

//        //[Required]
//        //[Display(Name = "Cover Type")]
//        //public int CoverTypeId { get; set; }
//        //[ValidateNever]
//        //public CoverType CoverType { get; set; }



//    }
//}
using BulkyWeb.Models;

public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; } = null!;

    public int? SupplierID { get; set; }
    public int? CategoryID { get; set; }

    public string? QuantityPerUnit { get; set; }
    public decimal UnitPrice { get; set; }
    public int? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
    //  public int? Status { get; set; }

    public Supplier? Supplier { get; set; }
    public Category? Category { get; set; }

    public IList<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
