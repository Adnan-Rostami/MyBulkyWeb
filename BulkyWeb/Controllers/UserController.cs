using BulkyWeb.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("Index")]
        public IActionResult Index()
        {
            var objCategoryList = _unitOfWork.CategoryRepository.GetAllAsync();
            _unitOfWork.SaveAsync();
            return Ok(objCategoryList);
        }









    }
}
