using Application.MainBoundedContext.DTO.AdministrationModule;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;

namespace SwiftFinancials.AppServiceContainer.Identity
{
    // Direct identity-store lookup for background plugins — mirrors
    // MembershipService.svc.cs's FindMembershipAsync exactly (see that file's
    // comment on ApplicationUser.cs), since there is no Application.MainBoundedContext
    // app-service equivalent to resolve through Container instead.
    public static class MembershipLookup
    {
        public static UserDTO FindMembership(string id)
        {
            using (var context = new ApplicationDbContext(ConfigurationManager.ConnectionStrings["AuthStore"].ConnectionString))
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                var applicationUser = userManager.FindById(id);

                if (applicationUser == null)
                    return null;

                return new UserDTO
                {
                    FirstName = applicationUser.FirstName,
                    OtherNames = applicationUser.OtherNames,
                    Email = applicationUser.Email,
                    PhoneNumber = applicationUser.PhoneNumber,
                    BranchId = applicationUser.BranchId,
                    LockoutEnabled = applicationUser.LockoutEnabled,
                    CreatedDate = applicationUser.CreatedDate,
                    CustomerId = applicationUser.CustomerId
                };
            }
        }
    }
}
