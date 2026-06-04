namespace BulkyWeb.Models.DTO.Order
{
    public class OrderResponseDTO
    {
        public int OrderId { get; set; }

        public string Message { get; set; } = string.Empty;
    }


    public class OrderUpdateResponseDTO : OrderResponseDTO
    {
        public DateTime? UpdatedAt { get; set; }
    }
    public class OrderCreateResponseDTO : OrderResponseDTO
    {
        public DateTime? CreatedAt { get; set; }
        public string? PaymentUrl { get; set; }

    }
}