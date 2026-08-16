using Infrastructure.Crosscutting.Framework.Utils;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SwiftFinancials.Utility.Identity
{
    // Schema-only mirror of DistributedServices.MainBoundedContext/Identity/ApplicationDbContext.cs
    // — see ApplicationUser.cs for why. No custom migrations configuration (same as
    // WebApplication1's copy), so EF's default CreateDatabaseIfNotExists initializer applies —
    // Database.Initialize(force: true) is enough to bootstrap a fresh AuthStore.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        { }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable(string.Format("{0}AspNetUsers", DefaultSettings.Instance.TablePrefix));
            modelBuilder.Entity<IdentityRole>().ToTable(string.Format("{0}AspNetRoles", DefaultSettings.Instance.TablePrefix));
            modelBuilder.Entity<IdentityUserRole>().ToTable(string.Format("{0}AspNetUserRoles", DefaultSettings.Instance.TablePrefix));
            modelBuilder.Entity<IdentityUserClaim>().ToTable(string.Format("{0}AspNetUserClaims", DefaultSettings.Instance.TablePrefix));
            modelBuilder.Entity<IdentityUserLogin>().ToTable(string.Format("{0}AspNetUserLogins", DefaultSettings.Instance.TablePrefix));
        }
    }
}
