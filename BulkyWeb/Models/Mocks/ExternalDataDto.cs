namespace BulkyWeb.Models.Mocks
{
    public class ExternalDataDto
    {
        public int? ExternalId { get; set; }

        public string Id { get; set; } = default!;
        public int Number { get; set; }
        public DateTime GeneratedAt { get; set; }       // زمان

    }
}
