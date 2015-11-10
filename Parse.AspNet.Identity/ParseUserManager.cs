using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Nito.AsyncEx;

namespace Parse.AspNet.Identity
{
    public class ParseUserManager<TUser> : UserManager<TUser> where TUser : ParseIdentityUser, IUser<string>
    {
        private readonly AsyncLock mutex = new AsyncLock();

        public ParseUserManager(IUserStore<TUser> store) : base(store)
        {
            PasswordHasher = new NullPasswordHasher();
        }

        /// <summary>
        /// Implements password verification using parse. Password verification cannot be done comparing hashes, we must attempt to login against
        /// parse in order to check the password.
        /// Unfortunately, due to parse's library design, verification must be done atomically. This has a big impact on performance because
        /// now all verifications are done in sequence. Hopefully since this is only used for logging in, the real impact will be negigible.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected override async Task<bool> VerifyPasswordAsync(IUserPasswordStore<TUser, string> store, TUser user, string password)
        {
            try
            {
                using (await mutex.LockAsync())
                {
                    var login = await ParseUser.LogInAsync(user.UserName, password);
                    // Login was successful. Get session id
                    var session = await ParseSession.GetCurrentSessionAsync();
                    user.SessionToken = session.SessionToken;
                    return true;
                }
            }
            catch (Exception e)
            {
                // The login failed. Check the error to see why.
                return false;
            }
        }

        /// <summary>
        /// Password changing is rather convoluted. It involves (atomically) becoming the user, logging in to test the current password, changing 
        /// the password and finally re-logging in to obtain the new session token. It must also be done sequentially with scalability penalties.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionToken"></param>
        /// <param name="currentPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<IdentityResult> ChangePasswordAsync(string userId, string sessionToken, string currentPassword, string newPassword)
        {
            var passwordStore = Store as IUserPasswordStore<TUser, string>;
            using (await mutex.LockAsync())
            {
                // become the user
                await ParseUser.BecomeAsync(sessionToken);
                var userName = ParseUser.CurrentUser.Username;
                var user = await FindByIdAsync(userId);

                // verify current password
                try
                {
                    await ParseUser.LogInAsync(userName, currentPassword);
                }
                catch (Exception)
                {
                    return IdentityResult.Failed("Passwords mismatch");
                }

                // change password
                var result = await UpdatePassword(passwordStore, user, newPassword);
                if (!result.Succeeded)
                {
                    return result;
                }

                result = await UpdateAsync(user);

                // parse requires logout-login after a password change. Changing a password invalidates all session tokens associated with that account
                if (result == IdentityResult.Success)
                {
                    await ParseUser.LogInAsync(userName, newPassword);
                }

                return result;
            }
        }

        public override Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            throw new InvalidOperationException("Use ChangePasswordAsync(string userId, string sessionToken, string currentPassword, string newPassword)");
        }

        public override async Task<ClaimsIdentity> CreateIdentityAsync(TUser user, string authenticationType)
        {
            var identity = await base.CreateIdentityAsync(user, authenticationType);
            // add sessiontoken claim
            identity.AddClaim(new Claim("urn:parse-sessionToken", user.SessionToken));

            return identity;
        }
    }
}