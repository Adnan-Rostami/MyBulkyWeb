using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TerritoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        public TerritoryController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }






        //--------------------
        //[HttpGet("Index")]
        //public async Task<IActionResult> IndexTerritory()
        //{
        //    var getAll = await _db.Territories
        //        .AsNoTracking()
        //        .Select(c => new
        //        {
        //            c.OrderID,
        //            c.CustomerID,
        //            c.EmployeeID,
        //            c.OrderDate,
        //            c.RequiredDate,
        //            c.ShippedDate,
        //            c.ShipVia,
        //            c.Freight,
        //            c.ShipName,
        //            c.ShipAddress,
        //            c.ShipCity,
        //            c.ShipRegion,
        //            c.ShipPostalCode,
        //            c.ShipCountry
        //        })
        //        .ToListAsync();

        //    return Ok(getAll);
        //}


        [HttpPost]
        public IActionResult Create([FromBody] Territory obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.TerritoryRepository.AddAsync(obj);
                _unitOfWork.SaveAsync();
                return Ok("Created");
            }
            else
            {
                return Ok("not found");
            }

        }


        [HttpPut]
        public IActionResult Update([FromBody] Territory obj)
        {
            //var id = obj;

            _unitOfWork.TerritoryRepository.Update(obj);

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
            _unitOfWork.TerritoryRepository.Deleted(id);


            _unitOfWork.SaveAsync();
            return Ok("removed");

        }






    }
}
