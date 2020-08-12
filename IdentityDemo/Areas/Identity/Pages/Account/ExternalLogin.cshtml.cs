using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using IdentityDemo.Classes;

namespace IdentityDemo.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly CustomUserManager _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            CustomUserManager userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Display(Name = "نام")]
            public string FirstName { get; set; }

            [Display(Name = "نام خانوادگی")]
            public string LastName { get; set; }

            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGetAsync(string email, string firstName, string lastName, string providerDisplayName, string returnUrl = null)
        {
            Input = new InputModel()
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            ProviderDisplayName = providerDisplayName;
            ReturnUrl = returnUrl;
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            Input = new InputModel();
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    var user = new ApplicationUser
                    {
                        UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    }; //Don't use Input Email if email confirmation is disabled

                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                    {
                        user.FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                    }

                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Surname))
                    {
                        user.LastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
                    }

                    user.EmailConfirmed = true; //email confirmation is not required when using the email address provided by external login

                    var res = await _userManager.CreateAsync(user);
                    if (res.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        //login
                        var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
                        if (loginInfo == null)
                        {
                            ErrorMessage = "Error loading external login information during confirmation.";
                            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
                        }
                        else
                        {
                            res = await _userManager.AddLoginAsync(user, loginInfo);
                            if (res.Succeeded)
                            {
                                await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                                return RedirectToPage("./ExternalLogin", new
                                {
                                    Email = user.Email,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    ProviderDisplayName,
                                    ReturnUrl,
                                });
                            }
                        }
                    }
                    foreach (var error in res.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login");
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                //update name info
                var user = await _userManager.GetUserAsync(this.User);
                if (user != null)
                {
                    user.FirstName = Input.FirstName;
                    user.LastName = Input.LastName;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return LocalRedirect(returnUrl);
                    }
                }
            }

            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
