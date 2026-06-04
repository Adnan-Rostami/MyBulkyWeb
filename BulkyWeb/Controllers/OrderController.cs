using AutoMapper;
using BulkyWeb.Data;
using BulkyWeb.Models.DTO.Order;
using BulkyWeb.Repository.IRepository;
using BulkyWeb.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(IUnitOfWork unitOfWork, ApplicationDbContext db, IMapper mapper, ILogger<OrderController> logger, IOrderService orderService)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _orderService = orderService;
        }

        [HttpGet("GetAll")]
        //https://localhost:7210/Order/GetAll
        //    https://localhost:7210/Order/GetAll?PageNumber=2&PageSize=5
        //https://localhost:7210/Order/GetAll?OrderID=10248
        //https://localhost:7210/Order/GetAll?CustomerId=AN
        //https://localhost:7210/Order/GetAll?SortBy=id_asc
        //    https://localhost:7210/Order/GetAll?SortBy=id_desc
        //        https://localhost:7210/Order/GetAll?CustomerId=AN&PageNumber=1&PageSize=10&SortBy=id_desc
        //https://localhost:7210/Order/GetAll?OrderID=10248&CustomerId=AN&SortBy=id_desc&PageNumber=2&PageSize=10

        public async Task<IActionResult> GetAll([FromQuery] OrderFilterDTO filterDTO)
        {
            return Ok(await _orderService.GetAllAsync(filterDTO));


            //        var query = _unitOfWork.OrderRepository.Query(); // IQueryable<Order>
            //        if (filterDTO.OrderID.HasValue)
            //        {
            //            query = query.Where(x => x.OrderID == filterDTO.OrderID);
            //        }

            //        if (!string.IsNullOrWhiteSpace(filterDTO.CustomerId))
            //        {
            //            query = query.Where(x => x.CustomerID.Contains(filterDTO.CustomerId));
            //        }


            //        switch (filterDTO.SortBy)
            //        {
            //            case "id_desc":
            //                query = query.OrderByDescending(x => x.OrderID);
            //                break;

            //            default:
            //                query = query.OrderBy(x => x.OrderID); // default
            //                break;
            //        }
            //        //         int page = filterDTO.PageNumber.HasValue && filterDTO.PageNumber > 0
            //        //? filterDTO.PageNumber.Value


            //        //: 1;
            //        //         int pageSize = filterDTO.PageSize.HasValue && filterDTO.PageSize <= 0 ? 3 : filterDTO.PageSize.Value;
            //        int page = filterDTO.PageNumber is > 0 ? filterDTO.PageNumber.Value : 1;
            //        int pageSize = filterDTO.PageSize is > 0 ? filterDTO.PageSize.Value : 3;
            //        query = query
            //                .Skip((page - 1) * pageSize)
            //                .Take(pageSize);

            //        var result = await _mapper
            //            .ProjectTo<OrderGetAllDTO>(query)
            //            .ToListAsync();

            //        return Ok(result);
            //    }

            //    //--------------------
            //    //https://localhost:7210/Order/GetByID/10248
            //    [HttpGet("GetById/{id}")]
            //    public async Task<IActionResult> GetById(int id)
            //    {
            //        var fileasa = await _unitOfWork.OrderRepository.Query()
            //.Where(x => x.OrderID == id)
            //.ProjectTo<OrderGetByIdDTO>(_mapper.ConfigurationProvider)
            //.FirstOrDefaultAsync();

            //        var file = await _unitOfWork.OrderRepository.Query().Where(x => x.OrderID == id)
            //            //.Include(o => o.OrderDetails)

            //            .ProjectTo<OrderGetByIdDTO>(_mapper.ConfigurationProvider)

            //            .FirstOrDefaultAsync();


            //        var filea = await _unitOfWork.OrderRepository.Query()
            //            .Where(x => x.OrderID == id)
            //            .Include(o => o.OrderDetails)
            //            .ThenInclude(od => od.Product)
            //            .ProjectTo<OrderGetByIdDTO>(_mapper.ConfigurationProvider)
            //            .FirstOrDefaultAsync();

            //        if (file == null)
            //        {
            //            return NotFound("Order Not Found");
            //        }
            //        //        var order = await _context.Orders
            //        //.Include(o => o.OrderDetails)
            //        //.ThenInclude(od => od.Product)
            //        //.FirstOrDefaultAsync(o => o.OrderID == id);

            //        return Ok(file);
        }



        [Authorize(Policy = "Order.Create")]
        //        https://localhost:7210/Order/CreateOrder
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> Create([FromBody] OrderCreateDTO obj, CancellationToken ct)
        {

            return Ok(await _orderService.OrderCreate(obj, ct));


        }





        [HttpPatch("UpdateByID/{id}")]
        ///https://localhost:7210/Order/UpdateByID/78
        public async Task<IActionResult> UpdateByID(int id, [FromBody] OrderUpdateDTO body)
        {

            return Ok(await _orderService.UpdateByIDAsync(id, body));


        }












        //https://localhost:7210/Order/RemoveOrder/11086
        [HttpDelete("RemoveOrder/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderService.DeleteAsync(id);
            return Ok("removed");
            //        await _unitOfWork.OrderRepository
            //.Query()
            //// .Include(od => od.OrderDetails)
            //.Where(x => x.OrderID == id)
            //.ExecuteDeleteAsync();

            //var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            //_unitOfWork.OrderRepository.Remove(order);
            //await _unitOfWork.SaveAsync();
            await _db.OrderDetails
     .Where(x => x.OrderID == id)
     .ExecuteDeleteAsync();

            await _db.Orders
                .Where(x => x.OrderID == id)
                .ExecuteDeleteAsync();
            //            await _unitOfWork.OrderRepository()

            //.Query()




            await _unitOfWork.SaveAsync();
            return Ok("removed");

        }






    }
}
