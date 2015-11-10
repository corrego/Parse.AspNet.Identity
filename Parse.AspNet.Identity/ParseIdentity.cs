using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parse.AspNet.Identity
{
    public static class ParseIdentity
    {
        public static void RegisterSubclasses()
        {
            ParseObject.RegisterSubclass<IdentityUserLogin>();
        }
    }
}
