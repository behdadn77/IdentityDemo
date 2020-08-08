﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityDemo.Classes;
using IdentityDemo.Data;
using IdentityDemo.Models;
using IdentityDemo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static IdentityDemo.Models.ViewModels.IdentityPropertiesViewModel;

namespace IdentityDemo.Areas.SiteAdmin.Controllers
{
    [Authorize("SiteAdminPolicy")]
    [Area("SiteAdmin")]
    [Route("SiteAdmin/[controller]/[action]")]
    public class ManageUsersController : Controller
    {
        private readonly DBContext db;
        private readonly CustomUserManager userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IOptions<IdentityProperties> options;
        private readonly ILogger<ManageUsersController> logger;

        public ManageUsersController(DBContext db, CustomUserManager userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityProperties> options,
            ILogger<ManageUsersController> logger)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.options = options;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string query = "")
        {
            ViewData["defaultAdminUserName"] = options.Value.AdminUser.EmailAddress;
            ViewData["defaultAdminRoleName"] = "SiteAdmins";

            query = query ?? "";
            List<UserRolesViewModel> userRolesList = new List<UserRolesViewModel>();
            foreach (var user in userManager.Users.Where(x => x.UserName.Contains(query)).Skip((page - 1) * pageSize).Take(pageSize).OrderBy(x => x.UserName).ToList())
            {
                UserRolesViewModel userRoles = new UserRolesViewModel()
                {
                    User = user
                };
                foreach (var role in roleManager.Roles.ToList())
                {
                    userRoles.Roles.Add(new UserRoleStatus()
                    {
                        RoleName = role.Name,
                        IsInRole = await userManager.IsInRoleAsync(user, role.Name)
                    });
                }
                userRolesList.Add(userRoles);
            }
            ViewData["pageNum"] = page;
            ViewData["totalPages"] = Math.Ceiling(userManager.Users.Count() / (double)pageSize);
            ViewData["query"] = query;

            return View(userRolesList);
        }

        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = model.Username,
                    UserName = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var status = await userManager.CreateAsync(user, model.Password);
                if (!status.Succeeded)
                {
                    TempData["GlobalError"] = $"Failed to register user {status.Errors.ToString()}";
                }
            }
            else
            {
                TempData["GlobalError"] = "Account already exists!";
            }
            TempData["GlobalSuccess"] = "User registerd successfully";
            return RedirectToAction("ManageUsers");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            return View(new EditUserViewModel()
            {
                CurrentUsername = user.UserName, //as pk
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            ApplicationUser user = await userManager.FindByNameAsync(options.Value.AdminUser.EmailAddress);
            if (user == null)
            {
                TempData["GlobalError"] = "User doesn't exist.";
                return RedirectToAction("ManageUsers");
            }
            if (model.Username != options.Value.AdminUser.EmailAddress && user.UserName == options.Value.AdminUser.EmailAddress)
            {
                TempData["GlobalError"] = "Default Admin's Username can not be changed.";
                return RedirectToAction("ManageUsers");
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.Username;
            user.Email = model.Username;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["GlobalError"] = $"Updating details was Unsuccessful. {result.Errors.ToString()}";
                return RedirectToAction("ChangePass");
            }
            TempData["GlobalSuccess"] = "Details updated Successfully.";
            return RedirectToAction("ManageUsers");

        }

        [HttpGet]
        public IActionResult ChangePass(string username)
        {
            return View(new ChangePassViewModel()
            {
                Username = username
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePass(ChangePassViewModel model)
        {
            ApplicationUser user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                TempData["GlobalError"] = "User doesn't exist.";
                return RedirectToAction("ManageUsers");
            }
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, model.NewPass);
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["GlobalError"] = $"Changing password was Unsuccessful.  {result.Errors.ToString()}";
                return RedirectToAction("ChangePass");
            }
            TempData["GlobalSuccess"] = "Password changed Successfully.";
            return RedirectToAction("ManageUsers");

        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            return View(model: username);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserConfirm(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                if (await userManager.DeleteAsync(user) == IdentityResult.Success)
                {
                    TempData["GlobalSuccess"] = "User deleted successfully.";
                    return RedirectToAction("ManageUsers");
                }
                TempData["GlobalError"] = "Failed to delete user.";
                return RedirectToAction("ManageUsers");
            }
            TempData["GlobalError"] = "User not found.";
            return RedirectToAction("ManageUsers");
        }

        [HttpGet]
        public async Task<IActionResult> ChangeUserRoles(string userName)
        {

            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                UserRolesViewModel userRoles = new UserRolesViewModel()
                {
                    User = user
                };
                foreach (var role in roleManager.Roles.ToList())
                {
                    userRoles.Roles.Add(new UserRoleStatus()
                    {
                        RoleName = role.Name,
                        IsInRole = await userManager.IsInRoleAsync(user, role.Name)
                    });
                }
                return View(userRoles);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserRoles(string userName, List<string> roles)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                foreach (var role in roles)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                var allRoles = roleManager.Roles.Select(x => x.Name).ToList();
                foreach (var role in allRoles.Except(roles))
                {
                    if (user.UserName == options.Value.AdminUser.EmailAddress && role == "SiteAdmins")
                    {
                        TempData["GlobalError"] = "default site admin cannot be demoted";
                    }
                    else
                    {
                        await userManager.RemoveFromRoleAsync(user, role);
                    }
                }
            }
            TempData["GlobalError"] = "Err";
            return RedirectToAction("ChangeUserRoles", routeValues: new { userName });
        }
    }
}
