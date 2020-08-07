using System;
using IdentityDemo.Data;
using IdentityDemo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(IdentityDemo.Areas.Identity.IdentityHostingStartup))]
namespace IdentityDemo.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<DBContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DBContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;

                    // Password configurations
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 0;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;

                })
                    .AddRoles<IdentityRole>()
                    .AddDefaultUI()
                    .AddEntityFrameworkStores<DBContext>();

                services.AddAuthorization(x =>
                {
                    x.AddPolicy("AdminPolicy", y => y.RequireRole("Admins"));
                    x.AddPolicy("AdminPolicy", y => y.RequireRole("Admins"));
                });

            });
        }
    }
}