using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public Guid? BranchId { get; set; }

        public string FirstName { get; set; }

        public string OtherNames { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Today;

        public Guid? CustomerId { get; set; }

        public Guid? EmployeeId { get; set; }

        public DateTime? LastPasswordChangedDate { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            userIdentity.AddClaim(new Claim(ApplicationUserProperties.BranchId, BranchId.ToString()));

            userIdentity.AddClaim(new Claim(ApplicationUserProperties.CustomerId, CustomerId.ToString()));

            userIdentity.AddClaim(new Claim(ApplicationUserProperties.FullName, string.Format("{0} {1}", FirstName, OtherNames)));

            return userIdentity;
        }
    }

    public class ApplicationUserProperties
    {
        public const string BranchId = "BranchId";

        public const string CustomerId = "CustomerId";

        public const string FullName = "FullName";
    }
}