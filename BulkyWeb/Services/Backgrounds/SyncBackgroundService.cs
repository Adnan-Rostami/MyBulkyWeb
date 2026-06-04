using BulkyWeb.Data;
using BulkyWeb.Models.Mocks;

namespace BulkyWeb.Services.Backgrounds
{
    public class SyncBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        //private readonly ApplicationDbContext _db;
        private readonly ILogger<SyncBackgroundService> _logger;

        public SyncBackgroundService(IServiceProvider serviceProvider
            , ILogger<SyncBackgroundService> logger
            //, ApplicationDbContext db
            )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            //_db = db;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int retryCount = 0;
            TimeSpan[] retryIntervals = {
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(10)
    };

            while (!stoppingToken.IsCancellationRequested)
            {
                // --- اضافه کردن BeginScope در اینجا ---
                // هر بار که حلقه اجرا می‌شود، یک شناسه جدید (CorrelationId) می‌سازد
                using (_logger.BeginScope(new Dictionary<string, object> { ["IterationId"] = Guid.NewGuid() }))
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("MockApi");

                        try
                        {
                            var response = await httpClient.GetAsync("WeatherForecast", stoppingToken);

                            if (response.IsSuccessStatusCode)
                            {
                                var data = await response.Content.ReadFromJsonAsync<ExternalDataDto>(cancellationToken: stoppingToken);

                                if (data != null)
                                {
                                    var entity = new ExternalDataEntry { ExternalId = data.Id, Value = data.Number };
                                    db.ExternalDataEntries.Add(entity);
                                    await db.SaveChangesAsync(stoppingToken);

                                    _logger.LogInformation("داده جدید با آیدی {ExternalId} ذخیره شد.", entity.ExternalId);

                                    // --- نکته مهم: ریست کردن شمارنده خطا در صورت موفقیت ---
                                    retryCount = 0;

                                    await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
                                }
                            }
                            else
                            {
                                _logger.LogError("خطا در فراخوانی API. کد وضعیت: {StatusCode}", (int)response.StatusCode);
                                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                            }
                        }
                        catch (HttpRequestException ex)
                        {
                            var delay = retryCount < retryIntervals.Length
                                ? retryIntervals[retryCount]
                                : TimeSpan.FromHours(1);

                            _logger.LogError(ex, "مشکل در شبکه. تلاش مجدد در {Delay} دقیقه دیگر.", delay.TotalMinutes);

                            retryCount++;
                            await Task.Delay(delay, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "خطای غیرمنتظره در بک‌گراند سرویس.");
                        }
                    }
                }

                // یک مکث کوتاه برای جلوگیری از چرخش سریع حلقه در صورت خطاهای غیر شبکه
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

    }

}
//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        int retryCount = 0;
//        // زمان‌های وقفه: ۱ دقیقه، ۵ دقیقه، ۱۰ دقیقه
//        TimeSpan[] retryIntervals = {
//    TimeSpan.FromMinutes(1),
//    TimeSpan.FromMinutes(5),
//    TimeSpan.FromMinutes(10)
//};
//        while (!stoppingToken.IsCancellationRequested)
//        {
//            using (_logger.BeginScope(new Dictionary<string, object> { ["IterationId"] = Guid.NewGuid() }))
//            {
//                using (var scope = _serviceProvider.CreateScope())
//                {
//                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//                    var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("MockApi");

//                    try
//                    {
//                        // ۱. صدا کردنِ سایت ماک
//                        var response = await httpClient.GetAsync("WeatherForecast", stoppingToken);
//                        var rawJson = await response.Content.ReadAsStringAsync(stoppingToken);
//                        if (response.IsSuccessStatusCode)
//                        {
//                            var data = await response.Content.ReadFromJsonAsync<ExternalDataDto>(cancellationToken: stoppingToken);

//                            if (data == null)
//                            {
//                                _logger.LogWarning("MockApi returned empty/invalid json.");
//                            }

//                            else
//                            {

//                                var entity = new ExternalDataEntry
//                                {
//                                    ExternalId = data.Id,
//                                    Value = data.Number,
//                                    //CreatedAt = DateTime.UtcNow
//                                };
//                                db.ExternalDataEntries.Add(entity);
//                                await db.SaveChangesAsync(stoppingToken);

//                                //اینجا برای ثبت اینفو و وارنینگ؟
//                                _logger.LogInformation("داده جدید با آیدی {ExternalId} با موفقیت ذخیره شد.", entity.ExternalId);
//                                _logger.LogInformation("سیستم بعد از مدت 3 دقیقه، باز فعال میشود");
//                                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

//                                // ۳. پاکسازی (Retention - پاک کردن قدیمی‌ها)
//                                //var oldData = db.ExternalDataEntries.OrderBy(x => x.Id).Take(5); // اینجا تست کن
//                                //db.ExternalDataEntries.RemoveRange(oldData);
//                                //await db.SaveChangesAsync(stoppingToken);
//                            }
//                        }
//                        else
//                        {
//                            var body = await response.Content.ReadAsStringAsync(stoppingToken);
//                            _logger.LogWarning("MockApi failed: {StatusCode} Body={Body}", (int)response.StatusCode, body);
//                            //اینجا برای ثبت خطا و fail
//                            _logger.LogError("خطا در فراخوانی API. کد وضعیت: {StatusCode}", (int)response.StatusCode, body);
//                            _logger.LogInformation("سیستم بعد از مدت 1 دقیقه، باز فعال میشود");
//                            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

//                        }
//                    }
//                    catch (HttpRequestException ex)
//                    {
//                        //_logger.LogWarning("API is not responed, try it again later.");
//                        //_logger.LogInformation("سیستم بعد از مدت 5 دقیقه، باز فعال میشود");
//                        //await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
//                        var delay = retryCount < retryIntervals.Length
//                            ? retryIntervals[retryCount]
//                            : TimeSpan.FromHours(1); // اگر بیشتر از ۳ بار خطا داد، ۱ ساعت صبر کن

//                        _logger.LogError(ex, "Error occurred. Retrying in {Delay}...", delay);

//                        retryCount++;
//                        await Task.Delay(delay, stoppingToken);
//                    }
//                    catch (Exception ex) { _logger.LogError(ex, "Background sync failed."); }
//                }
//                // هر ۱۰ ثانیه یکبار اجرا بشه
//                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
//            }
//        }
//    }
//}
