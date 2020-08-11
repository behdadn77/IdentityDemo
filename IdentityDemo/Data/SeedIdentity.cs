using IdentityDemo.Classes;
using IdentityDemo.Models;
using IdentityDemo.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Data
{
    public class SeedIdentity
    {
        public static async Task Initialize(IServiceProvider serviceProvider, IdentityProperties identityProperties)
        {
            var userManager = serviceProvider.GetRequiredService<CustomUserManager>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = new string[] { "SiteAdmins", "ContentManagers" };
            foreach (var item in roles)
            {
                if (await roleManager.RoleExistsAsync(item) == false)
                {
                    IdentityRole newrole = new IdentityRole(item);
                    await roleManager.CreateAsync(newrole);
                }
            }

            var user = await userManager.FindByEmailAsync(identityProperties.AdminUser.EmailAddress);
            if (user == null)
            {
                user = new ApplicationUser()
                {
                    Email = identityProperties.AdminUser.EmailAddress,
                    NormalizedEmail = identityProperties.AdminUser.EmailAddress.ToUpper(),
                    UserName = identityProperties.AdminUser.EmailAddress,
                    NormalizedUserName = identityProperties.AdminUser.EmailAddress.ToUpper(), //IMPORTENT USERNAME MUST BE SAME AS EMAIL ADDRESS OTHERWISE LOGIN FAILES
                    EmailConfirmed = true,
                };


                var status = await userManager.CreateAsync(user, identityProperties.AdminUser.Password);
                if (status.Succeeded == true)
                {
                    await userManager.AddToRoleAsync(user, "SiteAdmins");
                }
            }
        }

    }
}
