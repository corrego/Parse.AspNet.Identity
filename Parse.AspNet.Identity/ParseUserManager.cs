using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Parse.AspNet.Identity
{
    public class ParseUserManager<TUser> : UserManager<TUser> where TUser : IdentityUser, IUser<string>
    {
        public ParseUserManager(IUserStore<TUser> store) : base(store)
        {
            //ClaimsIdentityFactory = new ClaimsIdentityFactory<TUser, string>();
        }


        public override async Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            try
            {
                user.User.Password = password;
                await user.User.SignUpAsync();
            }
            catch (ParseException e)
            {
                if (e.Message.Contains("already taken"))
                {
                    // todo: translate messages from resources
                }
                return IdentityResult.Failed(e.Message);
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Implements Parse log in
        /// </summary>
        /// <param name="store"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected override async Task<bool> VerifyPasswordAsync(IUserPasswordStore<TUser, string> store, TUser user, string password)
        {
            try
            {
                await ParseUser.LogInAsync(user.UserName, password);
                // Login was successful.
                return true;
            }
            catch (Exception e)
            {
                // The login failed. Check the error to see why.
                return false;
            }
        }

        public override Task<IdentityResult> AddClaimAsync(string userId, Claim claim)
        {
            return base.AddClaimAsync(userId, claim);
        }

        public override async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            // get user
            var user = await FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User does not exist!");
            }

            // check if password is correct
            if (await VerifyPasswordAsync(null, user, currentPassword))
            {
                // if the password is correct, we can use CurrentUser to change the password
                // We add a try and catch block because there might still be a communication error.
                try
                {
                    ParseUser.CurrentUser.Password = newPassword;
                    await ParseUser.CurrentUser.SaveAsync();

                    return IdentityResult.Success;
                }
                catch (ParseException e)
                {
                    IdentityResult.Failed(e.Message);
                }
            }

            return IdentityResult.Failed("The current password is not correct");
        }

        public override Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        {
            return Task.FromResult(new List<UserLoginInfo>() as IList<UserLoginInfo>);
        }

    }
}