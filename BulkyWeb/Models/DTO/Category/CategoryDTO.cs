//using System.ComponentModel.DataAnnotations;

//namespace BulkyWeb.Models.DTO.Category
//{
//    public class CategoryDTO
//    {

//        //public class CategoryGetByIdDTO
//        //{

//        //    public int CategoryID { get; set; }
//        //    public string CategoryName { get; set; }
//        //    public string? Description { get; set; }
//        //    public byte[]? Picture { get; set; }
//        //    public ICollection<CategoryProductDTO> Products { get; set; } = new List<CategoryProductDTO>();

//        //}
//        //public class CategoryUpdateDTO
//        //{
//        //    //public int CategoryID { get; set; }

//        //    public string CategoryName { get; set; }
//        ////    public string? Description { get; set; }
//        ////    public byte[]? Picture { get; set; }
//        ////}
//        //public class CategoryProductDTO
//        //{
//        //    public int ProductID { get; set; }
//        //    public string ProductName { get; set; }

//        //}


//        //public class CategoryCreateDTO
//        //{
//        //    [Required]
//        //    public string CategoryName { get; set; }

//        //    public string? Description { get; set; }

//        //    public byte[]? Picture { get; set; }
//        //}




//        public class CategoryFilterDTO
//        {
//            public int? CategoryID { get; set; }
//            public string? CategoryName { get; set; }
//            public string? Description { get; set; }
//            public byte[]? Picture { get; set; }
//            public string? SortBy { get; set; } = "id_asc";
//            public int PageNumber { get; set; } = 1;
//            public int PageSize { get; set; } = 3;
//        }
//    }
//}
