using AutoMapper;
using BulkyWeb.Exceptions;





//using BulkyWeb.Models.DTO.Category;
using BulkyWeb.Models.RoleModels;
using BulkyWeb.Validators.Roles;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;


        public RoleService(IMapper mapper, ILogger<RoleService> logger, RoleManager<IdentityRole> roleManager)
        {

            _mapper = mapper;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task DeleteAsync(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                throw new ValidationException("Invalid id");

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                throw new NotFoundException("Role not found");

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);



        }

        public async Task<List<RoleGetDTO>> GetAllAsync(RoleFilterDTO filter)
        {

            var query = _roleManager.Roles.AsQueryable();



            if (!string.IsNullOrEmpty(filter.RoleID))
                query = query.Where(x => x.Id == filter.RoleID);

            if (!string.IsNullOrEmpty(filter.RoleName))
                query = query.Where(x => x.Name.Contains(filter.RoleName));

            // Sorting
            switch (filter.SortBy)
            {
                case "name_desc":
                    query = query.OrderByDescending(x => x.Name);
                    break;

                default:
                    query = query.OrderBy(x => x.Name); // default
                    break;
            }

            // Pagination
            query = query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);

            var result = await _mapper
                .ProjectTo<RoleGetDTO>(query)
                .ToListAsync();

            return result;


        }




        public async Task<RoleGetDTO> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ValidationException("Invalid id");

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                throw new NotFoundException("Role Not Found");

            return _mapper.Map<RoleGetDTO>(role);


        }

        public async Task<RoleResponseDTO> RoleCreate(RoleCreateDTO filter)
        {
            var validator = new RoleCreateValidator();
            var resultvalidator = validator.Validate(filter);

            if (!resultvalidator.IsValid)
            {
                throw new ValidationException(resultvalidator.Errors);
            }


            try
            {
                var role = new IdentityRole(filter.Name);
                var result = await _roleManager.CreateAsync(role);


                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                return new RoleResponseDTO { Message = "Role Created" };


                //};
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Request was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during category creation");
                throw new DbUpdateException("Database error", ex);
            }

        }

        public async Task<RoleResponseDTO> RoleUpdateServiece(string id, RoleCreateDTO obj)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ValidationException("Id is not valid");

            var validator = new RoleUpdateValidator();
            var validate = validator.Validate(obj);
            if (!validate.IsValid)
                throw new ValidationException(validate.Errors);

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                throw new NotFoundException("Role not found");

            role.Name = obj.Name;
            role.NormalizedName = obj.Name.ToUpper();

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            return new RoleResponseDTO { Message = "Role Updated" };


        }
    }
}
