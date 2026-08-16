using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace SwiftFinancials.AppServiceContainer.Identity
{
    // Schema-only mirror of DistributedServices.MainBoundedContext/Identity/ApplicationUser.cs
    // (and WebApplication1/Areas/Identity/ApplicationUser.cs) — same shape, so EF sees the exact
    // same AspNetUsers columns, without depending on DistributedServices.MainBoundedContext.
    // Used by background plugins that need a direct identity lookup (e.g. AccountAlertDispatcher's
    // membership-triggered alerts) with no Application.MainBoundedContext app-service equivalent —
    // that logic lives inline in the legacy WCF MembershipService.svc.cs against its own
    // ApplicationUserManager, not behind any app service.
    public class ApplicationUser : IdentityUser
    {
        public Guid? BranchId { get; set; }

        public string FirstName { get; set; }

        public string OtherNames { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Today;

        public Guid? CustomerId { get; set; }

        public Guid? EmployeeId { get; set; }

        public DateTime? LastPasswordChangedDate { get; set; }
    }
}
