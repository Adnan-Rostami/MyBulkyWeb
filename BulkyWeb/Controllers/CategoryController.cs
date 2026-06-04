using AutoMapper;
using BulkyWeb.Models.DTO.Category;
using BulkyWeb.Repository.IRepository;
using BulkyWeb.Services.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    // [Authorize(Policy = "Dynamic")]
    public class CategoryController : ControllerBase
    {


        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICategoryService _CategoryService;
        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper, ICategoryService CategoryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _CategoryService = CategoryService;
        }


        //   [HttpGet("GetAll")]
        //[Authorize]
        [HttpGet]

        public async Task<IActionResult> GetAll([FromQuery] CategoryFilterDTO filterDTO)

        {

            return Ok(await _CategoryService.GetAllAsync(filterDTO));

        }







        //https://localhost:7210/Category/CreateCategory
        [HttpPost("CreateCategory")]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO obj)
        {
            return Ok(await _CategoryService.CategoryCreate(obj));

        }





        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _CategoryService.GetByIdAsync(id));

        }





        [HttpPatch("UpdateCategory/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDTO obj)
        {
            return Ok(await _CategoryService.CategoryUpdateServiece(id, obj));

        }



        [HttpDelete("RemoveCategory/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _CategoryService.DeleteAsync(id);
            return Ok("Removed");

        }


        [Authorize(Policy = "Dynamic")]
        [HttpGet("GetFound")]
        public IActionResult GetFound()
        {
            return Ok("authorized");
        }


        [Authorize(Policy = "Dynamic")]
        [HttpGet("GetNotFound")]
        public IActionResult GetNotFound()
        {
            return Ok("Not authorized");
        }






        [HttpGet("sync")]
        public async Task<IActionResult> sync()
        {
            Console.WriteLine("SYNC STARTED");
            await _CategoryService.SyncCategoriesToElastic();
            return Ok(new { message = "Sync process completed successfully." });
        }

        //[HttpGet("elastic/status")]
        //public async Task<IActionResult> ElasticStatus()
        //{
        //    var result = await _CategoryService.GetCategoriesElasticStatusAsync();
        //    return Ok(result);
        //}

        [HttpGet("elastic/search")]
        public async Task<IActionResult> ElasticSearch([FromQuery] CategoryFilterDTO filter)
        {
            var result = await _CategoryService.SearchAsync(filter);

            return Ok(result);
        }

        //[HttpGet("elastic/searchById/{ProductId}")]
        //public async Task<IActionResult> ElasticSearch(int ProductId)
        //{
        //    var result = await _CategoryService.GetProductByIdElasticAsync(ProductId);
        //    return Ok(result);
        //}



        [HttpGet("elastic/searchById/{CategoryId}")]
        public async Task<IActionResult> ElasticSearch(int CategoryId)
        {
            var result = await _CategoryService.GetByIdAsync(CategoryId);
            //if (result is null) return NotFound();
            return Ok(result);
        }






    }
}
