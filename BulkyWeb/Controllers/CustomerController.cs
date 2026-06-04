//using BulkyWeb.Data;
//using BulkyWeb.Repository.IRepository;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace BulkyWeb.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class CustomerController : Controller
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly IUnitOfWork _unitOfWork;
//        public CustomerController(IUnitOfWork unitOfWork, ApplicationDbContext db)
//        {
//            _db = db;
//            _unitOfWork = unitOfWork;
//        }






//        //--------------------
//        [HttpGet("Index")]
//        public async Task<IActionResult> IndexCustomers()
//        {
//            var getAll = await _db.Customers
//                .AsNoTracking()
//                .Select(c => new
//                {
//                    c.CustomerID,
//                    c.CompanyName,
//                    c.ContactName,
//                    c.ContactTitle,
//                    c.Address,
//                    c.City,
//                    c.Region,
//                    c.PostalCode,
//                    c.Country,
//                    c.Phone,
//                    c.Fax
//                })
//                .ToListAsync();

//            return Ok(getAll);
//        }


//        [HttpPost]
//        public IActionResult Create([FromBody] Customer obj)
//        {

//            if (ModelState.IsValid)
//            {
//                _unitOfWork.CustomerRepository.AddAsync(obj);
//                _unitOfWork.SaveAsync();
//                return Ok("Created");
//            }
//            else
//            {
//                return Ok("not found");
//            }

//        }


//        [HttpPut]
//        public IActionResult Update([FromBody] Customer obj)
//        {
//            //var id = obj;

//            _unitOfWork.CustomerRepository.Update(obj);

//            _unitOfWork.SaveAsync();

//            return Ok();
//        }




//        [HttpDelete]
//        public IActionResult Delete(int id)
//        {
//            if (id == 0)
//            {
//                return NotFound();
//            }
//            _unitOfWork.CustomerRepository.Deleted(id);


//            _unitOfWork.SaveAsync();
//            return Ok("removed");

//        }
//    }
//}
