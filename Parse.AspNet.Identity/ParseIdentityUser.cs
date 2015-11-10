using Microsoft.AspNet.Identity;

namespace Parse.AspNet.Identity
{
    public class ParseIdentityUser : IUser<string>
    {
        internal ParseUser User { get; set; }
        internal string SessionToken { get; set; }

        public ParseIdentityUser()
        {
            User = new ParseUser();

            // defaults
            PhoneNumber = null;
            PhoneNumberConfirmed = false;
        }

        public virtual string Email
        {
            get { return User.Email; }
            set { User.Email = value; }
        }

        public virtual string Password
        {
            set { User.Password = value; }
        }

        public virtual string PhoneNumber
        {
            get { return Get<string>("phoneNumber"); }
            set { User["phoneNumber"] = value; }
        }

        public virtual bool PhoneNumberConfirmed
        {
            get { return Get<bool>("phoneNumberConfirmed"); }
            set { User["phoneNumberConfirmed"] = value; }
        }

        public virtual string Id
        {
            get { return User.ObjectId; }
        }

        public virtual string UserName
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