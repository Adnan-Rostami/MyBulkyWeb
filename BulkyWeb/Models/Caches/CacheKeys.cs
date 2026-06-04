using BulkyWeb.Models.DTO.Category;
using BulkyWeb.Models.DTO.Product;

namespace BulkyWeb.Models.Caches
{
    public static class CacheKeys
    {
        public static int ProductVersion = 1;

        public static string Products(ProductFilterDTO filter)
        {
            return $"products:v{ProductVersion}_" +
                  $"pid:{filter.ProductID}_" +
                  $"cat:{filter.CategoryID}_" +
                  $"minp:{filter.MinUnitPrice}_" +
                  $"maxp:{filter.MaxUnitPrice}_" +
                  $"minstock:{filter.MinUnitsInStock}_" +
                  $"maxstock:{filter.MaxUnitsInStock}_" +
                  $"sort:{filter.SortBy}_" +
                  $"page:{filter.PageNumber}_" +
                  $"size:{filter.PageSize}";
        }


        public static int CategoryVersion = 1;

        public static string Categories(CategoryFilterDTO filter)
        {

            return $"Categories:v{CategoryVersion}_" +
                  $"Cid:{filter.CategoryName}_" +
                  $"name:{filter.CategoryID}_" +
                  $"cat:{filter.Description}_" +
                  $"sort:{filter.SortBy}_" +
                  $"page:{filter.PageNumber}_" +
                  $"size:{filter.PageSize}";
        }



    }
}
