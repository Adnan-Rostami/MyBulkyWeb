namespace BulkyWeb.Models.DTO.Category
{
    public class CategorySyncResult
    {
        public int DbCount { get; set; }
        public int SyncedCount { get; set; }
        public long ElasticCount { get; set; }
    }
}
