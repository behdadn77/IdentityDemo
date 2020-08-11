using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Classes
{
    public class IdentityDefaultOptions
    {
        public AdminUser AdminUser { get; set; }
    }
    public class AdminUser
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}
