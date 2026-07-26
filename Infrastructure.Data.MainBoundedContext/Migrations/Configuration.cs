namespace Infrastructure.Data.MainBoundedContext.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<Infrastructure.Data.MainBoundedContext.UnitOfWork.BoundedContextUnitOfWork>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            CommandTimeout = 3600;
            CodeGenerator = new NonClusteredPrimaryKeyCSharpMigrationCodeGenerator();
            SetSqlGenerator("System.Data.SqlClient", new NonClusteredPrimaryKeySqlMigrationSqlGenerator());
        }

        protected override void Seed(Infrastructure.Data.MainBoundedContext.UnitOfWork.BoundedContextUnitOfWork context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            #region Chart of Accounts agg

            //ChartOfAccount assetChartOfAccount = ChartOfAccountFactory.CreateChartOfAccount(ChartOfAccountType.Asset, ChartOfAccountCategory.HeaderAccount, (int)ChartOfAccountType.Asset, EnumHelper.GetDescription(ChartOfAccountType.Asset), false, false, false, null);

            //ChartOfAccount liabilityChartOfAccount = ChartOfAccountFactory.CreateChartOfAccount(ChartOfAccountType.Liability, ChartOfAccountCategory.HeaderAccount, (int)ChartOfAccountType.Liability, EnumHelper.GetDescription(ChartOfAccountType.Liability), false, false, false, null);

            //ChartOfAccount equityChartOfAccount = ChartOfAccountFactory.CreateChartOfAccount(ChartOfAccountType.Equity, ChartOfAccountCategory.HeaderAccount, (int)ChartOfAccountType.Equity, EnumHelper.GetDescription(ChartOfAccountType.Equity), false, false, false, null);

            //ChartOfAccount incomeChartOfAccount = ChartOfAccountFactory.CreateChartOfAccount(ChartOfAccountType.Income, ChartOfAccountCategory.HeaderAccount, (int)ChartOfAccountType.Income, EnumHelper.GetDescription(ChartOfAccountType.Income), false, false, false, null);

            //ChartOfAccount expenseChartOfAccount = ChartOfAccountFactory.CreateChartOfAccount(ChartOfAccountType.Expense, ChartOfAccountCategory.HeaderAccount, (int)ChartOfAccountType.Expense, EnumHelper.GetDescription(ChartOfAccountType.Expense), false, false, false, null);

            //context.ChartOfAccounts.AddOrUpdate(assetChartOfAccount, liabilityChartOfAccount, equityChartOfAccount, incomeChartOfAccount, expenseChartOfAccount);

            #endregion
        }
    }

    public sealed class AutoConfiguration : MigrateDatabaseToLatestVersion<Infrastructure.Data.MainBoundedContext.UnitOfWork.BoundedContextUnitOfWork, Configuration>
    {
        public AutoConfiguration(bool useSuppliedContext)
            : base(useSuppliedContext)
        { }

        public AutoConfiguration(string connectionStringName)
            : base(connectionStringName)
        { }
    }
}
