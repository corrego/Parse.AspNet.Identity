using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Parse.AspNet.Identity
{
    public class ParseRoleStore : IRoleStore<ParseIdentityRole, string>
    {
        private readonly string adminRoleName;

        public ParseRoleStore(string adminRoleName = "Admin")
        {
            this.adminRoleName = adminRoleName;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(ParseIdentityRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            var roleAcl = new ParseACL();
            // set public read
            roleAcl.PublicReadAccess = true;
            // set write for the admin role
            ParseRole adminRole = await GetAdminRole();
            if (adminRole != null)
            {
                roleAcl.SetRoleWriteAccess(adminRole, true);
            }

            // create a new role object to be sure it's pristine
            var nrole = new ParseRole(role.Name, roleAcl);

            await nrole.SaveAsync();
        }

        public async Task UpdateAsync(ParseIdentityRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            await role.Role.SaveAsync();
        }

        public Task DeleteAsync(ParseIdentityRole role)
        {
            throw new NotImplementedException();
        }

        public async Task<ParseIdentityRole> FindByIdAsync(string roleId)
        {
            ParseQuery<ParseRole> query = from role in ParseRole.Query
                where role.ObjectId == roleId
                select role;

            ParseRole r = await query.FirstOrDefaultAsync();

            if (r != null)
            {
                var appRole = new ParseIdentityRole();
                appRole.Role = r;
                return appRole;
            }

            return null;
        }

        public async Task<ParseIdentityRole> FindByNameAsync(string roleName)
        {
            ParseQuery<ParseRole> query = from role in ParseRole.Query
                where role.Name == roleName
                select role;

            ParseRole r = await query.FirstOrDefaultAsync();

            if (r != null)
            {
                var appRole = new ParseIdentityRole();
                appRole.Role = r;
                return appRole;
            }

            return null;
        }

        /// <summary>
        ///     Gets the Admin role
        /// </summary>
        /// <returns></returns>
        private async Task<ParseRole> GetAdminRole()
        {
            // todo: must be configurable somewhere
            ParseQuery<ParseRole> query = from role in ParseRole.Query
                where role.Name == adminRoleName
                select role;

            ParseRole r = await query.FirstOrDefaultAsync();
            return r;
        }
    }
}