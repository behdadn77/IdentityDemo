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
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using IdentityDemo.Services;
using IdentityDemo.Options;
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
            builder.AddJsonFile("emailsettings.json", optional: false, reloadOnChange: false);
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.AddUserSecrets<Program>();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<IdentityDefaultOptions>(Configuration.GetSection("IdentityProperties"));
            services.Configure<EmailSenderOptions>(Configuration.GetSection("EmailProperties"));

            #region Identity configurations 
            //(can be separated in IdentityHostingStartup)
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

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;

            })
                .AddRoles<IdentityRole>()
                .AddDefaultUI()
                .AddEntityFrameworkStores<DBContext>()
                .AddUserManager<CustomUserManager>(); //CustomUserManager class Inherited From UserManger<ApplicationUser> remove if not necessary 

            //external auth
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                        Configuration.GetSection("Authentication:Google");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                });

            services.AddAuthorization(x =>
            {
                x.AddPolicy("SiteAdminPolicy", y => y.RequireRole("SiteAdmins"));
                x.AddPolicy("ContentManagerPolicy", y => y.RequireRole("ContentManagers"));
            });

            #endregion

            #region Cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

                //options.Cookie.HttpOnly = true;
                //options.ExpireTimeSpan = TimeSpan.FromDays(500);
                //options.SlidingExpiration = true;
            });
            #endregion

            services.AddTransient<IEmailSender, EmailSender>(); // requires using Microsoft.AspNetCore.Identity.UI.Services; using WebPWrecover.Services;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider,
            IOptions<IdentityDefaultOptions> identityPropertiesOptions)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage(); //added
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
