Parse.AspNet.Identity
=====================

Asp.Net Identity provider for Parse.com user management. Implements a custom `UserStore` and `UserManager` that uses the Parse backend to store and retrieve user information. Right now it supports basic authentication and user creation and not much else, but I will add more features as I need them. You're welcome to contribute too!

Features
 - User creation, with support for extra custom fields
 - User log in/log out
 
What's Missing (almost everything else, but especially the following)
- User info editing
- Roles support
- Password changing
- External login support

# How to Use

Install the Nuget package.

```
PM> Install-Package Parse.AspNet.Identity 
```

Or fork the repository, build, and add as a reference to your project. Note that it also requires the Parse Nuget package.

As of now, I have only tested it with ASP.NET MVC 5. To use it, make the following changes to the following files:

1. Models/IdentityModels.cs: Replace the whole `ApplicationUser` class with the following:
```
public class ApplicationUser : ParseApplicationUserBase
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

3. Add the Parse initialization call (`ParseClient.Initialize()`) where it makes sense (I put it in Startup.cs)

4. Remember to add `using Parse.AspNet.Identity` clauses as needed.

Hopefully you will have now a Parse-enabled application. Please report any issues you might encounter.
