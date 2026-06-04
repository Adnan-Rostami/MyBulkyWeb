using BulkyWeb.Data;
using BulkyWeb.Models.Orders;
using BulkyWeb.Models.Payments;
using BulkyWeb.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Services.Payments
{
    public class PaymentService
    {
        private readonly ApplicationDbContext _db;
        //private readonly HttpClient _http;
        //private readonly IConfiguration _config;
        private readonly IUnitOfWork _uow;
        private readonly IPaymentGateway _gateway;
        private readonly ILogger<PaymentService> _logger;



        public PaymentService(ApplicationDbContext db
            //, IHttpClientFactory httpFactory, IConfiguration config
            , IUnitOfWork uow, IPaymentGateway gateway,
            ILogger<PaymentService> logger)
        {
            _db = db;
            //_http = httpFactory.CreateClient();
            //_config = config;
            _uow = uow;
            _gateway = gateway;
            _logger = logger;

        }




        public async Task<string> CreatePayment(int orderId, string userId)
        {
            var order = await _db.Orders
    .FirstOrDefaultAsync(o => o.OrderID == orderId && o.UserId == userId);

            if (order == null)
                throw new Exception("You don’t own this order");

            //   var merchantId = _config["ZarinPal:MerchantId"];
            const string MerchantId =
  "BulkyBulkyBulkyBulkyBulkyBulky";
            //var callback = _config["ZarinPal:CallbackUrl"];
            var callback = "https://example.com/payment/verify";
            var amount = order.TotalAfterDiscount;

            var result = await _gateway.CreateAsync(amount, callback);

            if (!result.IsSuccess)
                throw new Exception("Payment gateway failed");
            var payment = new Payment
            {
                OrderId = orderId,
                Authority = result.Authority,
                Amount = amount,
                Status = "Pending",
                Gateway = "Fake"

                //merchant_id = MerchantId,
                //amount = amount,
                //callback_url = callback,
                //description = "پرداخت سفارش",
                //metadata = new { email = "", mobile = "" }
            };
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            //return $"https://fake-gateway.local/pay/{result.Authority}";
            return $"https://localhost:7001/api/payment/verify?authority={result.Authority}&status=OK";

            //var url = "https://sandbox.zarinpal.com/pg/v4/payment/request.json";
            //var url = "https://payment.zarinpal.com/pg/v4/payment/request.json";

            //try
            //{
            //    var client = new HttpClient();

            //    var response = await client.GetAsync("https://sandbox.zarinpal.com");

            //    var text = await response.Content.ReadAsStringAsync();

            //    Console.WriteLine(text);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
            //var res = await _http.PostAsJsonAsync(url, req);
            //var raw = await res.Content.ReadAsStringAsync();
            //var json = JsonSerializer.Deserialize<ZarinPalRequestResponse>(raw);
            //Console.WriteLine(raw);
            //// var json = await res.Content.ReadFromJsonAsync<ZarinPalRequestResponse>();

            //if (json?.data?.code == 100)
            //{
            //    string authority = json.data.authority;

            //    var payment = new Payment
            //    {
            //        OrderId = orderId,
            //        Amount = amount,
            //        Authority = authority,
            //        Status = "Pending"
            //    };

            //    _db.Payments.Add(payment);
            //    await _db.SaveChangesAsync();

            //    return $"https://sandbox.zarinpal.com/pg/StartPay/{authority}";
            //}

            //throw new Exception(json?.errors?.message ?? "Zarinpal error");
        }

















        //public async Task<PaymentVerifyResult> Verify(string authority, decimal amount)
        public async Task<long?> Verify(string authority, string userId)
        {
            var strategy = _db.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                //using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
                using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);

                try
                {

                    var payment = await _db.Payments
                    .Include(o => o.Order)
                    .FirstOrDefaultAsync(x => x.Authority == authority);
                    Console.WriteLine($"DB Payment Amount: {payment.Amount.ToString("G29")}");
                    Console.WriteLine($"DB Order Amount: {payment.Order.TotalAfterDiscount.ToString("G29")}");
                    Console.WriteLine($"DB Order Amount: {payment.Order.TotalAfterDiscount.ToString("G29")}");



                    if (payment == null)
                        throw new Exception("Payment Not Found!");

                    if (payment.Order.UserId != userId)
                        throw new Exception("Unauthorized");


                    if (payment.Status == "Success")
                        return payment.RefId;


                    var result = await _gateway.VerifyAsync(authority, payment.Amount);

                    if (result.IsSuccess)
                    {

                        if (payment.Amount != result.PaidAmount)
                            throw new Exception("تغییر غیرمجاز در مبلغ پرداخت!");

                        payment.Status = "Success";
                        payment.RefId = result.RefId;
                        payment.VerifiedAt = DateTime.UtcNow;

                        payment.Order.Status = OrderStatus.success;


                        await Task.Delay(30000);



                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return result.RefId;
                    }
                    throw new Exception("خطا در تایید درگاه");
                }


                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, "Error occurred during category creation");

                    await transaction.RollbackAsync();
                    var inner = ex.InnerException;
                    if (inner is SqlException sqlEx && sqlEx.Number == 1205)
                    {
                        throw new Exception("ترافیک بالاست، لطفا مجدد تلاش کنید.");
                    }

                    //payment.Status = "Failed";
                    //await _db.SaveChangesAsync();
                    throw;
                    //return BadRequest(new
                    //{
                    //    Status = "Failed",
                    //    Message = "Payment has failed"
                    //});
                }

            });
        }
        //var merchantId = _config["ZarinPal:MerchantId"];

        //        var req = new
        //        {
        //            merchant_id = merchantId,
        //            amount = payment.Amount,
        //            authority = authority
        //        };

        //        var url = "https://sandbox.zarinpal.com/pg/v4/payment/verify.json";
        //        // var url = "https://payment.zarinpal.com/pg/v4/payment/request.json";
        //        var res = await _http.PostAsJsonAsync(url, req);

        //            try
        //            {
        //                var client = new HttpClient();

        //        var response = await client.GetAsync("https://sandbox.zarinpal.com");

        //        var text = await response.Content.ReadAsStringAsync();

        //        Console.WriteLine(text);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex.ToString());
        //            }


        //var json = await res.Content.ReadFromJsonAsync<ZarinPalVerifyResponse>();

        //if (json?.data?.code == 100 || json?.data?.code == 101)
        //{
        //    //   payment.Status = "Success";
        //    payment.RefId = json.data.ref_id;
        //    payment.VerifiedAt = DateTime.UtcNow;

        //    payment.Order.Status = OrderStatus.success;

        //    await _db.SaveChangesAsync();
        //    if (payment.Status == "Success")
        //        return payment.RefId;
        //    //  return payment.RefId;
        //}

        //payment.Status = "Failed";
        //await _db.SaveChangesAsync();

        //return null;
        //        }
        //    }

        //    public class ZarinPalRequestResponse
        //{
        //    public ZarinPalRequestData data { get; set; }
        //    public ZarinPalError errors { get; set; }
        //}

        //public class ZarinPalRequestData
        //{
        //    public int code { get; set; }
        //    public string message { get; set; }
        //    public string authority { get; set; }
        //    public string fee_type { get; set; }
        //    public int fee { get; set; }

        //    //public int code { get; set; }
        //    //public string authority { get; set; }
        //}

        //public class ZarinPalVerifyResponse
        //{
        //    public ZarinPalVerifyData data { get; set; }
        //}

        //public class ZarinPalVerifyData
        //{
        //    public int code { get; set; }
        //    public long ref_id { get; set; }
        //}
        //public class ZarinPalError
        //{
        //    public int code { get; set; }
        //    public string message { get; set; }
        //}
    }
}