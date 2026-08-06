using ABP.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Infrastructure.Identity.Seeds
{
    public static class DefaultUserRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Cashier.ToString()));
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Client.ToString()));
        }
    }
}
