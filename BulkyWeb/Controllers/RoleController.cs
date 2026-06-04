using AutoMapper;
using BulkyWeb.Data;
using BulkyWeb.Models.RoleModels;
using BulkyWeb.Repository.IRepository;
using BulkyWeb.Services.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRoleService _RoleService;
        public RoleController(IUnitOfWork unitOfWork, ApplicationDbContext db, IMapper mapper, IRoleService RoleService)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _RoleService = RoleService;
        }
        //https://localhost:7210/Role/GetAll
        [Authorize(Roles = "ModirBozorg")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] RoleFilterDTO filterDTO)

        {

            return Ok(await _RoleService.GetAllAsync(filterDTO));
        }


        //https://localhost:7210/Role/RoleCreate
        [HttpPost("RoleCreate")]
        public async Task<IActionResult> Create([FromBody] RoleCreateDTO obj)
        {
            return Ok(await _RoleService.RoleCreate(obj));

        }
        //https://localhost:7210/Role/GetById/id
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {

            return Ok(await _RoleService.GetByIdAsync(id));



        }
        ////https://localhost:7210/Role/UpdateRole/id
        [HttpPatch("UpdateRole/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RoleCreateDTO obj)
        {
            return Ok(await _RoleService.RoleUpdateServiece(id, obj));
        }

        ////https://localhost:7210/Role/RemoveRole/id
        [HttpDelete("RemoveRole/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _RoleService.DeleteAsync(id);
            return Ok("Removed");
        }



    }
}