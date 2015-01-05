using Microsoft.AspNet.Identity;

namespace Parse.AspNet.Identity
{
    internal class ParseIdentityRole : IRole<string>
    {
        public ParseIdentityRole()
        {
            Role = new ParseRole();
        }

        internal ParseRole Role { get; set; }

        public string Id
        {
            get { return Role.ObjectId; }
        }

        public string Name
        {
            get { return Role.Name; }
            set { Role.Name = value; }
        }
    }
}