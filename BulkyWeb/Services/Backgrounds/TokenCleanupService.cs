namespace BulkyWeb.Services.Backgrounds
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        //private readonly ApplicationDbContext _db;
        private readonly ILogger<SyncBackgroundService> _logger;
        public TokenCleanupService(IServiceProvider serviceProvider
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
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //using (var scope = _serviceProvider.CreateScope())
                    //{
                    //    _logger.LogInformation("Start Cleaning Expired Tokens");

                    //    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    //    Console.WriteLine($"Cleanup running at: {DateTime.Now}");
                    //    while (true)
                    //    {
                    //        var expiredTokens = db.RefreshTokens.Where(E => E.Expires < DateTime.Now).OrderBy(t => t.Expires).Take(100).ToList();
                    //        if (!expiredTokens.Any()) break;
                    //        db.RefreshTokens.RemoveRange(expiredTokens);
                    //        await db.SaveChangesAsync(stoppingToken);

                    //        _logger.LogInformation($"تعداد {expiredTokens.Count} توکن منقضی شده پاک شد.");
                    //        _logger.LogInformation($"  {expiredTokens.Count}  Expired Tokens.");
                    Console.WriteLine("TokenCleanUpService");
                    //    }
                    //    // ۲. کد پاکسازی رو اینجا صدا بزن (از طریق IServiceScopeFactory)

                    //}
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در حین پاکسازی توکن‌ها");
                }
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // هر ۱ دقیقه

            }
        }
    }
}
