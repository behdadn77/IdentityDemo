using IdentityDemo.Models;
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
        public static async Task Initialize(IServiceProvider serviceProvider
/*, SitePropertiesViewModel siteProperties*/)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //userManager.PasswordHasher = new CustomPasswordHasher();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = new string[] { "Admins", "ContentManagers" };
            foreach (var item in /*siteProperties.roles*/ roles)
            { 
                if (await roleManager.RoleExistsAsync(item) == false)
                {
                    IdentityRole newrole = new IdentityRole(item);
                    await roleManager.CreateAsync(newrole);
                }
            }

            var user = await userManager.FindByEmailAsync(/*siteProperties.AdminInfo.adminusername*/"jj@example.com");
            if (user == null)
            {
                //user = new ApplicationUser()
                //{
                //    Email = siteProperties.AdminInfo.adminusername,
                //    UserName = siteProperties.AdminInfo.adminusername,
                //    PhoneNumber = siteProperties.AdminInfo.adminphone,
                //    FirstName = "admin",
                //    LastName = ""
                //};
                user = new ApplicationUser()
                {
                    FirstName = "John",
                    LastName = "Johnson",
                    Email = "jj@example.com",
                    NormalizedEmail = "jj@example.com".ToUpper(),
                    UserName = "jj@example.com",
                    NormalizedUserName = "jj@example.com".ToUpper(), //IMPORTENT USERNAME MUST BE SAME AS EMAIL ADDRESS OTHERWISE LOGIN FAILES
                    PhoneNumber = "+111111111111",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    //SecurityStamp = Guid.NewGuid().ToString("D")
                };


                var status = await userManager.CreateAsync(user, "password" /*siteProperties.AdminInfo.adminpassword*/);
                if (status.Succeeded == true)
                {
                    await userManager.AddToRoleAsync(user, "Admins");
                }
            }
        }

    }
}
