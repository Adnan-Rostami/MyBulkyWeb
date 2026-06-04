using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        public SupplierController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }



        //--------------------
        [HttpGet("Index")]
        public async Task<IActionResult> Index()

        {
            //List<Category> objCategoryList = _db.Categories.ToList();
            //var aa = _unitOfWork.CategoryRepository.GetAll();
            var GetAll = await _db.Suppliers.AsNoTracking().Select(c => new
            {
                c.SupplierID,
                c.Address,
                c.Country,
                c.CompanyName,
                c.ContactName,
                c.ContactTitle,
                c.City,
                c.Region,
                c.PostalCode,
                c.Fax,
                c.Phone,
                c.HomePage

            }
    )
    .ToListAsync();

            return Ok(GetAll);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Supplier obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.SupplierRepository.AddAsync(obj);
                _unitOfWork.SaveAsync();
                return Ok("Created");
            }
            else
            {
                return Ok("not found");
            }

        }


        [HttpPut]
        public IActionResult Update([FromBody] Supplier obj)
        {
            //var id = obj;

            _unitOfWork.SupplierRepository.Update(obj);

            _unitOfWork.SaveAsync();

            return Ok();
        }




        [HttpDelete]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            _unitOfWork.SupplierRepository.Deleted(id);


            _unitOfWork.SaveAsync();
            return Ok("removed");

        }
    }
}
