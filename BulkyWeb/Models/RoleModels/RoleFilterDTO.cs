namespace BulkyWeb.Models.RoleModels
{
    public class RoleFilterDTO
    {
        public string? RoleID { get; set; }
        public string? RoleName { get; set; }
        public string? SortBy { get; set; } = "Name_asc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 3;
    }
}
