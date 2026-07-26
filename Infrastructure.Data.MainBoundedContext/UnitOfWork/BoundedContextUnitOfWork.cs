using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork
{
    public class BoundedContextUnitOfWork : DbContext
    {
        public BoundedContextUnitOfWork()
        { }

        public BoundedContextUnitOfWork(string nameOrConnectionString)
            : base(nameOrConnectionString)
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // force datetime2(2)
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2").HasPrecision(2));

            // Remove unused conventions
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            // Add entity configurations in a structured way using 'TypeConfiguration’ classes
            modelBuilder.Configurations.AddFromAssembly(typeof(BoundedContextUnitOfWork).Assembly);
        }
    }
}
