namespace BulkyWeb.Models.DTO.Product.ProductElasticDocument
{
    public class ProductSyncResult
    {
        public int DbCount { get; set; }
        public int SyncedCount { get; set; }
        public long ElasticCount { get; set; }
    }
}
