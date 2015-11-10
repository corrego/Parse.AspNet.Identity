using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Parse.AspNet.Identity
{
    class NullPasswordHasher:IPasswordHasher
    {
        /// <summary>
        /// Parse hashes passwords internally, so we only need to provide the plaintext passwords when assigning them
        /// </summary>
        public string HashPassword(string password)
        {
            return password;
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            throw new NotImplementedException();
        }
    }
}
