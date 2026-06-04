using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShipperController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        public ShipperController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }






        //--------------------
        [HttpGet("Index")]
        public async Task<IActionResult> IndexShipper()
        {
            var getAll = await _db.Shippers
                .AsNoTracking()
                .Select(c => new
                {
                    c.ShipperID,
                    c.CompanyName,
                    c.Phone
                })
                .ToListAsync();

            return Ok(getAll);
        }


        [HttpPost]
        public IActionResult Create([FromBody] Shipper obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.ShipperRepository.AddAsync(obj);
                _unitOfWork.SaveAsync();
                return Ok("Created");
            }
            else
            {
                return Ok("not found");
            }

        }


        [HttpPut]
        public IActionResult Update([FromBody] Shipper obj)
        {
            //var id = obj;

            _unitOfWork.ShipperRepository.Update(obj);

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
            _unitOfWork.ShipperRepository.Deleted(id);


            _unitOfWork.SaveAsync();
            return Ok("removed");

        }
    }
}
