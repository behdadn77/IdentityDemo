using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDemo.Areas.ContentManager.Controllers
{
    //[Authorize("AdminPolicy")]
    [Area("ContentManager")]
    [Route("ContentManager/[controller]/[action]")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
