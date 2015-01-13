using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parse.AspNet.Identity
{
    class IdentityUserLogin:ParseObject
    {
        public IdentityUserLogin() : base("UserLogin")
        {
            
        }

        public ParseUser User
        {
            get { return Get<ParseUser>("user"); }
            set { Set("user", value);}
        }

        public string LoginProvider
        {
            get { return Get<string>("loginProvider"); }
            set { Set("loginProvider", value); }
        }

        public string ProviderKey
        {
            get { return Get<string>("providerKey"); }
            set { Set("providerKey", value); }
        }

        public static ParseQuery<IdentityUserLogin> Query
        {
            get { return new ParseQuery<IdentityUserLogin>(); }
        }

        protected T Get<T>(string key)
        {
            T res;
            if (TryGetValue(key, out res))
            {
                return res;
            }

            return default(T);
        }

        protected void Set<T>(string key, T value)
        {
            this[key] = value;
        }
    }
}
