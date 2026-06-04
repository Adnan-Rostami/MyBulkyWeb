namespace BulkyWeb.Models.DTO.Payment
{
    public class PaymentRequestResult
    {
        public bool IsSuccess { get; set; }
        public string Authority { get; set; }
        public string Message { get; set; }
    }
}
