using BulkyWeb.Data;
using BulkyWeb.Models.Permissions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Services
{
    public static class PermissionSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var actionProvider =
                scope.ServiceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();

            var db =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var actions = actionProvider.ActionDescriptors.Items
                .OfType<ControllerActionDescriptor>()
                .Select(x => $"{x.ControllerName}.{x.ActionName}")
                .Distinct()
                .ToList();

            var existingPermissions =
                await db.Permissions
                    .Select(x => x.Name)
                    .ToListAsync();

            foreach (var action in actions)
            {
                if (!existingPermissions.Contains(action))
                {
                    db.Permissions.Add(new Permission
                    {
                        Name = action
                    });
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
