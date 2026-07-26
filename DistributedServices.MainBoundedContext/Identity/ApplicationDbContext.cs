using Infrastructure.Crosscutting.Framework.Utils;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DistributedServices.MainBoundedContext.Identity
{
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