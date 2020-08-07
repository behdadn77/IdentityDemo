using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDemo.Areas.Identity.Controllers
{
    public class IdentityController : Controller
    {

        //UNCOMMENT EACH PAGE YOU WANT TO DISABLE
        //Check out https://github.com/aspnet/Identity/issues/1824

        //[Route("Identity/Account/ForgotPassword")]
        //[HttpGet]
        //public IActionResult ForgotPasswordGet()
        //{
        //    //return NotFound();
        //    return Redirect("/Identity/Account/Login");
        //}

        //[Route("Identity/Account/ForgotPassword")]
        //[HttpPost]
        //public IActionResult ForgotPasswordPost()
        //{
        //    return NotFound();
        //}



        //[Route("Identity/Account/Manage")]
        //[HttpGet]
        //public IActionResult ManageGet()
        //{
        //    return NotFound();
        //}

        //[Route("Identity/Account/Manage")]
        //[HttpPost]
        //public IActionResult ManagePost()
        //{
        //    return NotFound();
        //}


        //[Route("Identity/Account/Register")]
        //[HttpGet]
        //public IActionResult RegisterGet()
        //{
        //    return Redirect("/Identity/Account/Login");
        //}

        //[Route("Identity/Account/Register")]
        //[HttpPost]
        //public IActionResult RegisterPost()
        //{
        //    return NotFound();
        //}

    }
}
