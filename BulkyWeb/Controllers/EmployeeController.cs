//using BulkyWeb.Data;
//using BulkyWeb.Repository.IRepository;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace BulkyWeb.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]

//    public class EmployeeController : Controller
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly IUnitOfWork _unitOfWork;
//        public EmployeeController(IUnitOfWork unitOfWork, ApplicationDbContext db)
//        {
//            _db = db;
//            _unitOfWork = unitOfWork;
//        }






//        //--------------------
//        [HttpGet("Index")]
//        public async Task<IActionResult> IndexEmployee()
//        {
//            var getAll = await _db.Employees
//                .AsNoTracking()
//                .Select(c => new
//                {
//                    c.EmployeeID,
//                    c.LastName,
//                    c.FirstName,
//                    c.Title,
//                    c.TitleOfCourtesy,
//                    c.BirthDate,
//                    c.HireDate,
//                    c.Address,
//                    c.City,
//                    c.Region,
//                    c.PostalCode,
//                    c.Country,
//                    c.HomePhone,
//                    c.Extension,
//                    c.Photo,
//                    c.Notes,
//                    c.ReportsTo,
//                    c.PhotoPath,
//                    c.Age
//                })
//                .ToListAsync();

//            return Ok(getAll);
//        }


//        [HttpPost]
//        public IActionResult Create([FromBody] Employee obj)
//        {

//            if (ModelState.IsValid)
//            {
//                _unitOfWork.EmployeeRepository.AddAsync(obj);
//                _unitOfWork.SaveAsync();
//                return Ok("Created");
//            }
//            else
//            {
//                return Ok("not found");
//            }

//        }


//        [HttpPut]
//        public IActionResult Update([FromBody] Employee obj)
//        {
//            //var id = obj;

//            _unitOfWork.EmployeeRepository.Update(obj);

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
//            _unitOfWork.EmployeeRepository.Deleted(id);


//            _unitOfWork.SaveAsync();
//            return Ok("removed");

//        }
//    }
//}
