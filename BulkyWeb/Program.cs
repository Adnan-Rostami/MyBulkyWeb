

using BulkyWeb.Authorization;
using BulkyWeb.Data;
using BulkyWeb.Middlewares;
using BulkyWeb.Models.Identities;
using BulkyWeb.Repository;
using BulkyWeb.Repository.IRepository;
using BulkyWeb.Serilogs;
using BulkyWeb.Services;
using BulkyWeb.Services.Backgrounds;
using BulkyWeb.Services.Caches;
using BulkyWeb.Services.Categories;
using BulkyWeb.Services.Orders;
using BulkyWeb.Services.Payments;
using BulkyWeb.Services.Products;
using BulkyWeb.Services.Roles;
using BulkyWeb.Services.UserRoles;
using BulkyWeb.Services.Users;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(2),
                errorNumbersToAdd: null);
        });

    // options.LogTo(Debug.WriteLine, LogLevel.Information);
    options.EnableSensitiveDataLogging();
});

// Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("JWT Key is missing from configuration.");

if (string.IsNullOrWhiteSpace(jwtIssuer))
    throw new InvalidOperationException("JWT Issuer is missing from configuration.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// App Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRolesService, UserRolesService>();

builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<IPaymentGateway, FakePaymentGateway>();
builder.Services.AddScoped<ICacheService, CacheService>();

// Hosted Services
builder.Services.AddHostedService<TokenCleanupService>();
builder.Services.AddHostedService<SyncBackgroundService>();

// HttpClient
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("MockApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7156/");
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Cache
builder.Services.AddMemoryCache();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("login", limiterOptions =>
    {
        limiterOptions.PermitLimit = 2;
        limiterOptions.Window = TimeSpan.FromMinutes(5);
        limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("api", limiterOptions =>
    {
        limiterOptions.PermitLimit = 1;
        limiterOptions.Window = TimeSpan.FromSeconds(1);
        limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });
});

// Logging
builder.Host.AddSerilogConfiguration();
Environment.SetEnvironmentVariable("no_proxy", "localhost,127.0.0.1");
Environment.SetEnvironmentVariable("HTTP_PROXY", "");
Environment.SetEnvironmentVariable("HTTPS_PROXY", "");
System.Net.WebRequest.DefaultWebProxy = null;
//var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
var settings = new ElasticsearchClientSettings(new Uri("http://elasticsearch:9200"))

    //.DefaultIndex("categories")
    .DisableDirectStreaming()
 .ServerCertificateValidationCallback((o, certificate, chain, errors) => true);

;

var handler = new System.Net.Http.HttpClientHandler
{
    Proxy = null,
    UseProxy = false
};



var client = new ElasticsearchClient(settings);

builder.Services.AddSingleton(client);
var app = builder.Build();

// Seed permissions BEFORE app.Run()
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var db = services.GetRequiredService<ApplicationDbContext>();
//    await db.Database.MigrateAsync();

//    await PermissionSeeder.SeedAsync(services);
//}
for (int i = 0; i < 5; i++) // ۵ بار تلاش برای اتصال
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
        await PermissionSeeder.SeedAsync(scope.ServiceProvider);
        break;
    }
    catch (Exception)
    {
        if (i == 4) throw; // اگه بار پنجم هم نشد خطا بده
        await Task.Delay(10000); // ۱۰ ثانیه صبر کن تا SQL بیدار بشه
    }
}

// Global Exception Middlewares
app.UseGlobalExceptionHandling();

// Environment-based config
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bulky API V1");
        c.RoutePrefix = string.Empty; // یعنی در آدرس /swagger باز شود
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Custom status code pages
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == StatusCodes.Status404NotFound)
    {
        response.ContentType = "application/json; charset=utf-8";

        await response.WriteAsJsonAsync(new
        {
            status = 404,
            message = "The URL you entered is incorrect."
        });
    }
});

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
