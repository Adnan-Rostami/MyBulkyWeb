using BulkyWeb.Models;

namespace BulkyWeb.Services.UserRoles
{
    public interface IUserRolesService
    {
        Task<bool> AddRoleToUserAsync(UserRoleDTO model);

        Task<bool> RemoveRoleFromUserAsync(UserRoleDTO model);

        Task<IList<string>> GetUserRolesAsync(string userId);
    }
}
