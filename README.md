Parse.AspNet.Identity
=====================

Asp.Net Identity provider for Parse.com user management. Implements a custom `UserStore`, `UserManager` and `RoleManager` that uses the Parse backend to store and retrieve user information. Right now it supports basic authentication and user creation and not much else, but I will add more features as I need them. You're welcome to contribute too!

# What's New
## v0.5
- Breaking change: ParseApplicationUserBase is now called ParseIdentityUser to keep the naming convention and to avoid confusion between Identity implementations. Fixes issue #2
- Fixed NotImplementedException bug introduced by Asp.Net Identity 2.x that now calls GetAccessFailedCountAsync all the time. Fixes issue #3.

## v0.4
- Roles support!

# Features
 - User creation, with support for extra custom fields
 - User log in/log out
 - Roles through Parse.com permission system
 
What's Missing (almost everything else, but especially the following)
- User info editing
- Password changing
- External login support

# How to Use

Install the Nuget package.

```
PM> Install-Package Parse.AspNet.Identity 
```

Or fork the repository, build, and add as a reference to your project. Note that it also requires the Parse Nuget package.

As of now, I have only tested it with ASP.NET MVC 5. 

## User Management and Role checking
Make the following changes to the following files:

1. Models/IdentityModels.cs: Replace the whole `ApplicationUser` class with the following:
```
public class ApplicationUser : Parse.AspNet.Identity.ParseIdentityUser
{
    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    {
        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        // Add custom user claims here
        return userIdentity;
    }

    // This is the way to add new properties. Not really POCO but it's a start
    public string Age
    {
        get { return Get<string>("age"); }

        set { Set("age", value); }
    }
}
```

2. IdentityConfig.cs: Have the `ApplicationUserManager` class derive from `ParseUserManager<ApplicationUser>`. In the `Create` static method, initialize the manager with a `ParseUserStore` as in `var manager = new ApplicationUserManager(new ParseUserStore<ApplicationUser>());`

3. Add the Parse initialization call (`ParseClient.Initialize()`) where it makes sense (I usually put it in Startup.cs)

4. Remember to add `using Parse.AspNet.Identity` clauses as needed.

## Role Management
Role Management in Parse is a little bit more complicated due to the ACL system. In order to make it work, you need to have an existing Admin role (name it any way you want). Add at least one user to this role using other means.

The reason for this is because you need permission over your roles to be able to add users. For this, the RoleManager will add the admin role you specified to all new roles' ACL. This will allow your users in the admin role to add other users to these new roles.

Make the following changes in these files:

1. IdentityConfig.cs: Add/Replace your `ApplicationRoleManager` with the following
```
public class ApplicationRoleManager : RoleManager<ParseIdentityRole>
{
    public ApplicationRoleManager(IRoleStore<ParseIdentityRole, string> roleStore)
        : base(roleStore)
    {
    }

    public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
    {
        return new ApplicationRoleManager(new ParseRoleStore("Name of your admin role"));
    }
}

```

Hopefully you will have now a Parse-enabled application. Please report any issues you might encounter.
