using BulkyWeb.Models.DTO.Payment;

namespace BulkyWeb.Repository.IRepository
{
    public interface IPaymentGateway
    {
        Task<PaymentRequestResult> CreateAsync(decimal amount, string callback);
        Task<PaymentVerifyResult> VerifyAsync(string authority, decimal amount);
    }
}
