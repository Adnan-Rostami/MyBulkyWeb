using BulkyWeb.Models;
using BulkyWeb.Services.UserRoles;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRoleController : Controller
    {

        private readonly IUserRolesService _UserRoleService;
        public UserRoleController(IUserRolesService UserRoleService)
        {
            _UserRoleService = UserRoleService;

        }
        //https://localhost:7210/UserRole/GetAll?userId={id}
        //https://localhost:7210/UserRole/GetAll/{id}
        [HttpGet("GetAll/{userId}")]
        public async Task<IActionResult> GetAll([FromQuery] string userId)
        {
            return Ok(await _UserRoleService.GetUserRolesAsync(userId));
        }
        //https://localhost:7210/Role/RoleCreate
        [HttpPost("RoleCreate")]
        public async Task<IActionResult> RoleCreate([FromBody] UserRoleDTO filterDTO)
        {
            return Ok(await _UserRoleService.AddRoleToUserAsync(filterDTO));
        }
        //https://localhost:7210/Role/RemoveRole?filterDTO={id}
        //https://localhost:7210/Role/RemoveRole/{id}
        [HttpDelete("RemoveRole/{filterDTO}")]
        public async Task<IActionResult> RemoveRole([FromQuery] UserRoleDTO filterDTO)
        {
            return Ok(await _UserRoleService.RemoveRoleFromUserAsync(filterDTO));
        }


    }
}
