using BulkyWeb.Models.DTO.Payment;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class FakePaymentGateway : IPaymentGateway
    {
        public Task<PaymentRequestResult> CreateAsync(decimal amount, string callback)
        {
            return Task.FromResult(new PaymentRequestResult
            {
                Authority = Guid.NewGuid().ToString("N"),
                IsSuccess = true
            });
        }

        public Task<PaymentVerifyResult> VerifyAsync(string authority, decimal amount)
        {
            return Task.FromResult(new PaymentVerifyResult
            {
                IsSuccess = true,
                RefId = Random.Shared.NextInt64(100000, 999999),
                PaidAmount = amount
            });
        }


    }
}
