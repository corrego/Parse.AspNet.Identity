using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Parse.AspNet.Identity
{
    public class ParseUserStore<TUser> : 
        IUserStore<TUser>, 
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser, string>, 
        IUserPasswordStore<TUser, string>, 
        IUserTwoFactorStore<TUser, string>,
        IUserPhoneNumberStore<TUser>,
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser> where TUser : ParseIdentityUser, new()
    {
        public ParseUserStore()
        {
            
        }
        
        public void Dispose()
        {
            
        }

        #region IUserStore
        public virtual async Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            await user.User.SignUpAsync();
        }

        public virtual async Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (!user.User.IsAuthenticated)
            {
                throw new InvalidOperationException("User is not authenticated");
            }

            await user.User.SaveAsync();
        }

        public Task DeleteAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<TUser> FindByIdAsync(string userId)
        {
            // looking for the current logged user?
            if (ParseUser.CurrentUser != null && ParseUser.CurrentUser.ObjectId == userId)
            {
                var session = await ParseSession.GetCurrentSessionAsync();
                return new TUser()
                {
                    User = ParseUser.CurrentUser,
                    SessionToken = session.SessionToken
                };
            }

            // find regular user
            var query = from users in ParseUser.Query
                where users.Get<string>("objectId") == userId
                select users;

            var user = await query.FirstOrDefaultAsync();

            if (user != null)
            {
                var appUser = new TUser {User = user};
                return appUser;
            }

            return null;
        }

        public async Task<TUser> FindByNameAsync(string userName)
        {
            var query = from users in ParseUser.Query
                        where users.Get<string>("username") == userName
                        select users;

            var user = await query.FirstOrDefaultAsync();

            if (user != null)
            {
                var appUser = new TUser {User = user};
                return appUser;
            }

            return null;
        }
        #endregion

        #region IUserLockoutStore
        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public async Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(0);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Returns whether the user can be locked out.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.FromResult(false);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IUserPasswordStore
        public async Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.User.Password = passwordHash;
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IUserTwoFactorStore

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(false);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserPhoneNumberStore

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            user.PhoneNumber = phoneNumber;

            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult(0);
        }

        #endregion

        #region IUserLoginStore

        public async Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var newLogin = new IdentityUserLogin()
            {
                User = user.User,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey
            };

            await newLogin.SaveAsync();
        }

        public async Task RemoveLoginAsync(TUser user, UserLoginInfo loginInfo)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (loginInfo == null)
            {
                throw new ArgumentNullException("loginInfo");
            }

            var query = from login in IdentityUserLogin.Query
                        where login.User.HasSameId(user.User) && login.ProviderKey == loginInfo.ProviderKey && login.LoginProvider == loginInfo.LoginProvider
                        select login;

            var entry = await query.FirstOrDefaultAsync();
            if (entry!=null)
            {
                await entry.DeleteAsync();
            }
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var query = from login in IdentityUserLogin.Query
                where login.User == user.User
                select login;

            var result = await query.FindAsync();
            return result.Select(r => new UserLoginInfo(r.LoginProvider, r.ProviderKey
                )).ToList();
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserRoleStore

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetRolesAsync(TUser user)
        {
            // want to create your roles?
            //var roleACL = new ParseACL();
            //roleACL.PublicReadAccess = true;
            //var nrole = new ParseRole("Admin", roleACL);
            //nrole.Users.Add(user.User);
            //await nrole.SaveAsync();

            // get all roles the current user belongs to
            var query = from role in ParseRole.Query
                where role.Get<IList<string>>("users").Contains(user.Id)
                select role;

            var roles = await query.FindAsync();

            // return the list of names
            return roles.Select(r => r.Name).ToList();
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserEmailStore

        public async Task SetEmailAsync(TUser user, string email)
        {
            user.Email = email;
            await user.User.SaveAsync();
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            //return Task.FromResult(user.)
            // todo: implement
            return Task.FromResult(false);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public async Task<TUser> FindByEmailAsync(string email)
        {
            var query = from users in ParseUser.Query
                        where users.Get<string>("email") == email
                        select users;

            var user = await query.FirstOrDefaultAsync();

            if (user != null)
            {
                var appUser = new TUser { User = user };
                return appUser;
            }

            return null;
        }

        #endregion
    }
}