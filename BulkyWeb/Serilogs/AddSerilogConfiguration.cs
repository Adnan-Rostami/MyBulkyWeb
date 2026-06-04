using Serilog;
using Serilog.Events;

namespace BulkyWeb.Serilogs
{
    public static class SerilogConfig // نام کلاس رو هم بذار
    {
        public static void AddSerilogConfiguration(this IHostBuilder host)
        {
            host.UseSerilog((content, loggerConfig) =>
                {
                    //var logPath = Path.Combine(Directory.GetCurrentDirectory(), "C:\\BulkyWeb\\Logs");
                    var logPath = "C:\\BulkyWeb\\Logs";
                    loggerConfig
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Map(
                         "SourceContext",
                    "General",
                    (sourceContext, wt) =>

                    {
                        // تمیز کردن نام کلاس برای اسم پوشه
                        var className = sourceContext.Split('.').Last().Trim('"');
                        wt.File(Path.Combine(logPath, className, "log-.txt"),
                            rollingInterval: RollingInterval.Day,
                              retainedFileCountLimit: 5,
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}");

                    });
                });
        }
    }
}