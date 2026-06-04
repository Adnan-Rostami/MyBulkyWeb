using BulkyWeb.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BulkyWeb.Services
{
    public static class JwtClaimsExtension
    {
        public static async Task<List<Claim>> GetPermissionClaims(
            ApplicationDbContext dbContext,
            List<string> roles)
        {
            var permissions = await (
                from r in dbContext.Roles
                join rp in dbContext.RolePermissions
                    on r.Id equals rp.RoleID
                join p in dbContext.Permissions
                    on rp.PermissionID equals p.Id
                where roles.Contains(r.Name!)
                select p.Name
            ).Distinct().ToListAsync();

            return permissions
                .Select(p => new Claim("permission", p))
                .ToList();
        }
    }
}
