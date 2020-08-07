using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDemo.Areas.SiteAdmin.Controllers
{
    [Authorize("AdminPolicy")]
    [Area("SiteAdmin")]
    [Route("SiteAdmin/[controller]/[action]")]

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
