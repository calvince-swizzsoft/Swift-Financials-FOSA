using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace SwiftFinancials.Utility.Identity
{
    // Schema-only mirror of DistributedServices.MainBoundedContext/Identity/ApplicationUser.cs
    // (and WebApplication1/Areas/Identity/ApplicationUser.cs) — same shape, so EF creates/migrates
    // the exact same AspNetUsers columns, without SwiftFinancials.Utility depending on
    // DistributedServices.MainBoundedContext. Only used here to bootstrap the identity database;
    // no behavior beyond the shape is needed for that.
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
