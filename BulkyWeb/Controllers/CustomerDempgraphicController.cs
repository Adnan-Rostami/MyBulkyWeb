
//using BulkyWeb.Data;
//using BulkyWeb.Repository.IRepository;
//using Microsoft.AspNetCore.Mvc;

//namespace BulkyWeb.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class CustomerDemographicController : Controller
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly IUnitOfWork _unitOfWork;

//        public CustomerDemographicController(IUnitOfWork unitOfWork, ApplicationDbContext db)
//        {
//            _db = db;
//            _unitOfWork = unitOfWork;
//        }






//        //--------------------
//        //[HttpGet("Index")]
//        //public async Task<IActionResult> IndexCustomerDemporaphic()
//        //{
//        //    var getAll = await _db.CustomerDemographics
//        //        .AsNoTracking()
//        //        .Select(c => new
//        //        {
//        //            c.OrderID,
//        //            c.CustomerID,
//        //            c.EmployeeID,
//        //            c.OrderDate,
//        //            c.RequiredDate,
//        //            c.ShippedDate,
//        //            c.ShipVia,
//        //            c.Freight,
//        //            c.ShipName,
//        //            c.ShipAddress,
//        //            c.ShipCity,
//        //            c.ShipRegion,
//        //            c.ShipPostalCode,
//        //            c.ShipCountry
//        //        })
//        //        .ToListAsync();

//        //    return Ok(getAll);
//        //}


//        [HttpPost]
//        public IActionResult Create([FromBody] CustomerDemographic obj)
//        {

//            if (ModelState.IsValid)
//            {
//                _unitOfWork.CustomerDemographicRepository.AddAsync(obj);
//                _unitOfWork.SaveAsync();
//                return Ok("Created");
//            }
//            else
//            {
//                return Ok("not found");
//            }

//        }


//        [HttpPut]
//        public IActionResult Update([FromBody] CustomerDemographic obj)
//        {
//            //var id = obj;

//            _unitOfWork.CustomerDemographicRepository.Update(obj);

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
//            _unitOfWork.CustomerDemographicRepository.Deleted(id);


//            _unitOfWork.SaveAsync();
//            return Ok("removed");

//        }






//    }
//}
