namespace BulkyWeb.Models.DTO.Payment
{
    public class PaymentVerifyResult
    {
        public bool IsSuccess { get; set; }
        public long RefId { get; set; }
        public string Message { get; set; }
        public decimal PaidAmount { get; set; }
    }
}
