using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace IdentityDemo.Classes
{
    public class CustomPasswordHasher : PasswordHasher<ApplicationUser>
    {
        private string EncryptPassword(string password) 
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(password);
            for (int i = 0; i < 3; i++)
            {
                var hashing = SHA512.Create();
                b = hashing.ComputeHash(b);
                int index = (b[2] + 1) / 4;
                b[index] = Convert.ToByte((b[index] * 3) % 256);
            }
            return "CustomHashedPass" + Convert.ToBase64String(b);
        }


        public override string HashPassword(ApplicationUser user, string password)
        {
            return EncryptPassword(password);
        }

        public override PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword == EncryptPassword(providedPassword))
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;
        }

    }
}
