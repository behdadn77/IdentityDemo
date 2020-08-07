using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityDemo.Models;
using IdentityDemo.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityDemo.Models.ViewModels;
using static IdentityDemo.Models.ViewModels.IdentityPropertiesViewModel;
using Microsoft.Extensions.Options;
using IdentityDemo.Classes;

namespace IdentityDemo
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            //Configuration = configuration;

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(env.ContentRootPath);
            builder.AddJsonFile("identitysettings.json", optional: false, reloadOnChange: false);
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<IdentityProperties>(Configuration.GetSection("IdentityProperties"));

            //Identity configurations (can be separated in IdentityHostingStartup)
            services.AddDbContext<DBContext>(options =>
                  options.UseSqlServer(
                      Configuration.GetConnectionString("DBContextConnection")));

            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

                // Password configurations note: must change DataValidations for ViewModels
                options.Password.RequireDigit = false; 
                options.Password.RequiredLength = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = true;

            })
                .AddRoles<IdentityRole>()
                .AddDefaultUI()
                .AddEntityFrameworkStores<DBContext>()
                .AddUserManager<CustomUserManager>(); //CustomUserManager class Inherited From UserManger<ApplicationUser> remove if not necessary 

            services.AddAuthorization(x =>
            {
                x.AddPolicy("AdminPolicy", y => y.RequireRole("SiteAdmins"));
                x.AddPolicy("ContentManagerPolicy", y => y.RequireRole("ContentManagers"));
            });

            //
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider,
            IOptions<IdentityProperties> identityPropertiesOptions)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); //enabling Identity
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages(); //required for identity
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            SeedIdentity.Initialize(serviceProvider, identityPropertiesOptions.Value).Wait(); //creating roles and admin acc
        }
    }
}
