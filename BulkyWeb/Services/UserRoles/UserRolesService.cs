using AutoMapper;
using BulkyWeb.Exceptions;
using BulkyWeb.Models;
using BulkyWeb.Models.Identities;
using BulkyWeb.Services.Roles;
using Microsoft.AspNetCore.Identity;

namespace BulkyWeb.Services.UserRoles
{
    public class UserRolesService : IUserRolesService
    {
        // private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;


        public UserRolesService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, ILogger<RoleService> logger)
        {
            _mapper = mapper;
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;


        }

        public async Task<bool> AddRoleToUserAsync(UserRoleDTO model)
        {
            if (model == null) throw new ArgumentException("Invalid model");

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) throw new NotFoundException("User not founded");

            var role = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!role) throw new NotFoundException("role not founded");



            var isInRole = await _userManager.IsInRoleAsync(user, model.RoleName);
            if (isInRole)
                throw new Exception("User already has this role");

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            return result.Succeeded;
            //throw new NotImplementedException();
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new NotFoundException("User not founded");

            var result = await _userManager.GetRolesAsync(user);
            return result;

        }

        public async Task<bool> RemoveRoleFromUserAsync(UserRoleDTO model)
        {
            if (model == null) throw new ArgumentException("Invalid model");

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) throw new NotFoundException("User not founded");

            var role = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!role) throw new NotFoundException("role not founded");



            var isInRole = await _userManager.IsInRoleAsync(user, model.RoleName);
            if (isInRole)
                throw new Exception("User does not have this role");

            var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);

            return result.Succeeded;
            //  throw new NotImplementedException();
        }
    }
}
