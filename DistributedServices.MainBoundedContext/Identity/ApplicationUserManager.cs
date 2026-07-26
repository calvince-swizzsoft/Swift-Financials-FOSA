using Microsoft.AspNet.Identity;
using System;
using System.Configuration;

namespace DistributedServices.MainBoundedContext.Identity
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = bool.Parse(ConfigurationManager.AppSettings["AllowOnlyAlphanumericUserNames"]),
                RequireUniqueEmail = bool.Parse(ConfigurationManager.AppSettings["RequireUniqueEmail"])
            };

            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = Convert.ToInt32(ConfigurationManager.AppSettings["RequiredPasswordLength"]),
                RequireNonLetterOrDigit = bool.Parse(ConfigurationManager.AppSettings["PasswordRequireNonLetterOrDigit"]),
                RequireDigit = bool.Parse(ConfigurationManager.AppSettings["PasswordRequireDigit"]),
                RequireLowercase = bool.Parse(ConfigurationManager.AppSettings["PasswordRequireLowercase"]),
                RequireUppercase = bool.Parse(ConfigurationManager.AppSettings["PasswordRequireUppercase"]),
            };

            // Configure user lockout defaults
            this.UserLockoutEnabledByDefault = bool.Parse(ConfigurationManager.AppSettings["UserLockoutEnabledByDefault"]);
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"]));
            this.MaxFailedAccessAttemptsBeforeLockout = Convert.ToInt32(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"]);

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            this.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            this.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
        }
    }
}