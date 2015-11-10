using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parse.AspNet.Identity
{
    [ParseClassName("UserLogin")]
    class IdentityUserLogin:ParseObject
    {

        [ParseFieldName("user")]
        public ParseUser User
        {
            get { return GetProperty<ParseUser>(); }
            set { SetProperty(value);}
        }

        [ParseFieldName("loginProvider")]
        public string LoginProvider
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("providerKey")]
        public string ProviderKey
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
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
