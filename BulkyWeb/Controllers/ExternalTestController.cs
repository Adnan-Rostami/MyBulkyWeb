using BulkyWeb.Data;
using BulkyWeb.Models.Mocks;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalTestController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _db;
        public ExternalTestController(IHttpClientFactory httpClientFactory, ApplicationDbContext db)
        {
            //_httpClient = httpClientFactory.CreateClient("MockApi");
            //_db = db;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet("test-call")]
        public async Task<IActionResult> TestCall()
        {
            try
            {

                var client = _httpClientFactory.CreateClient("MockApi");

                var response = await client.GetAsync("WeatherForecast");
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Mock api error");
                }

                var data = await response.Content.ReadFromJsonAsync<ExternalDataDto>();

                // ذخیره در دیتابیس خودت
                var entity = new ExternalDataEntry
                {
                    ExternalId = data.Id,
                    Value = data.Number,
                    CreatedAt = DateTime.UtcNow
                };

                _db.ExternalDataEntries.Add(entity);
                await _db.SaveChangesAsync();

                return Ok(entity); // برگردونیمش که ببینی تو DB چی ذخیره شده
            }
            catch
            {
                throw;
            }



            //var response = await _httpClient.GetAsync("WeatherForecast");



            //    var content = await response.Content.ReadAsStringAsync();
            //    return Content(content, "application/json");
        }
    }
}
