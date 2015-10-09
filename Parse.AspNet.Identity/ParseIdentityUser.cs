using Microsoft.AspNet.Identity;

namespace Parse.AspNet.Identity
{
    public class ParseIdentityUser : IUser<string>
    {
        internal ParseUser User { get; set; }

        public ParseIdentityUser()
        {
            User = new ParseUser();

            // defaults
            PhoneNumber = null;
            PhoneNumberConfirmed = false;
        }

        public string Email
        {
            get { return User.Email; }

            set { User.Email = value; }
        }


        [ParseFieldName("phoneNumber")]
        public string PhoneNumber
        {
            get { return Get<string>("phoneNumber"); }

            set { User["phoneNumber"] = value; }
        }

        [ParseFieldName("phoneNumberConfirmed")]
        public bool PhoneNumberConfirmed
        {
            get { return Get<bool>("phoneNumberConfirmed"); }

            set { User["phoneNumberConfirmed"] = value; }
        }

        public string Id
        {
            get { return User.ObjectId; }
        }

        public string UserName
        {
            get { return User.Username; }

            set { User.Username = value; }
        }

        protected T Get<T>(string key)
        {
            T res;
            if (User.TryGetValue(key, out res))
            {
                return res;
            }

            return default(T);
        }

        protected void Set<T>(string key, T value)
        {
            User[key] = value;
        }
    }
}