using BulkyWeb.Models.RoleModels;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Services.Roles
{
    public interface IRoleService
    {
        //Task<List<RoleGetDTO>> GetAllAsync(RoleGetDTO filter);
        Task<List<RoleGetDTO>> GetAllAsync(RoleFilterDTO filter);
        Task<RoleResponseDTO> RoleCreate(RoleCreateDTO filter);

        Task<RoleGetDTO> GetByIdAsync(string id);
        Task<RoleResponseDTO> RoleUpdateServiece(string id, [FromBody] RoleCreateDTO obj);


        Task DeleteAsync(string id);
    }
}
