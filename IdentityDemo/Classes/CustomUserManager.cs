using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Classes
{
    public class CustomUserManager : UserManager<ApplicationUser>
    {
        public CustomUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<CustomUserManager> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            PasswordHasher = new CustomPasswordHasher();
        }
    }
}
