namespace BulkyWeb.Models.Mocks
{
    public class ExternalDataEntry
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = default!;

        public int Value { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
