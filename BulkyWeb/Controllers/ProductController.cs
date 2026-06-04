using AutoMapper;
using BulkyWeb.Data;
using BulkyWeb.Models.DTO.Product;
using BulkyWeb.Repository.IRepository;
using BulkyWeb.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BulkyWeb.Controllers
{
    [EnableRateLimiting("api")]
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductService _ProductService;

        public ProductController(IUnitOfWork unitOfWork, ApplicationDbContext db, IMapper mapper, IProductService ProductService)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ProductService = ProductService;
        }

        [HttpGet("GetAll")]
        //https://localhost:7210/Product/GetAll?MinUnitPrice=1&MaxUnitPrice=3111
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterDTO filterDTO)
        {
            return Ok(await _ProductService.GetAllAsync(filterDTO));

        }



        [HttpGet("GetByID/{id}")]

        public async Task<IActionResult> GetProductByID(int id)
        {
            return Ok(await _ProductService.GetByIdAsync(id));

        }



        [HttpPatch("UpdateByID/{id}")]
        //https://localhost:7210/Product/UpdateByID/78
        public async Task<IActionResult> UpdateByID(int id, [FromBody] ProductUpdateDTO filterDTO)
        {
            return Ok(await _ProductService.ProductUpdateService(id, filterDTO));

        }


        //https://localhost:7210/Product/CreateProduct
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> Create([FromBody] ProductCreateDTO obj)
        {
            return Ok(await _ProductService.ProductCreate(obj));


        }


        //https://localhost:7210/Product/RemoveProduct/80
        [HttpDelete("RemoveProduct/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ProductService.DeleteAsync(id);
            return Ok("Product is removed");

        }










        // Elastic



        [HttpGet("sync")]
        public async Task<IActionResult> sync()
        {
            Console.WriteLine("SYNC STARTED");
            await _ProductService.SyncProductsToElastic();
            return Ok(new { message = "Sync process completed successfully." });
        }




        [HttpGet("elastic/status")]
        public async Task<IActionResult> ElasticStatus()
        {
            var result = await _ProductService.GetProductsElasticStatusAsync();
            return Ok(result);
        }

        [HttpGet("elastic/search")]
        public async Task<IActionResult> ElasticSearch([FromQuery] ProductFilterDTO filter)
        {
            var result = await _ProductService.SearchProductsElasticAsync(filter);
            return Ok(result);
        }

        //[HttpGet("elastic/searchById/{ProductId}")]
        //public async Task<IActionResult> ElasticSearch(int ProductId)
        //{
        //    var result = await _ProductService.GetProductByIdElasticAsync(ProductId);
        //    return Ok(result);
        //}





        //---------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterDTO filterDTO, CancellationToken ct)
      => Ok(await _ProductService.GetAllAsync(filterDTO));

        // این endpoint جدید: سرچ استاندارد (ES-first با fallback)
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] ProductFilterDTO filterDTO, CancellationToken ct)
            => Ok(await _ProductService.SearchAsync(filterDTO, ct));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
            => Ok(await _ProductService.GetByIdAsync(id, ct));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDTO obj, CancellationToken ct)
            => Ok(await _ProductService.ProductCreate(obj, ct));

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDTO dto, CancellationToken ct)
            => Ok(await _ProductService.ProductUpdateService(id, dto, ct));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _ProductService.DeleteAsync(id, ct);
            return Ok("Product is removed");
        }

        // Admin only
        [HttpPost("admin/sync")]
        public async Task<IActionResult> Sync(CancellationToken ct)
            => Ok(await _ProductService.SyncProductsToElastic(ct));
    }

}




