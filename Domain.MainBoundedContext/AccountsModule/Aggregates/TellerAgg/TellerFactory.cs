using Domain.MainBoundedContext.ValueObjects;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.TellerAgg
{
    public static class TellerFactory
    {
        public static Teller CreateTeller(int type, Guid? employeeId, Guid? chartOfAccountId, Guid? shortageChartOfAccountId, Guid? excessChartOfAccountId, Guid? floatCustomerAccountId, Guid? commissionCustomerAccountId, string description, Range range, int miniStatementItemsCap, string reference)
        {
            var teller = new Teller();

            teller.GenerateNewIdentity();

            teller.Type = (byte)type;

            switch ((TellerType)type)
            {
                case TellerType.Employee:
                    teller.EmployeeId = (employeeId != null && employeeId != Guid.Empty) ? employeeId : null;
                    teller.ChartOfAccountId = (chartOfAccountId != null && chartOfAccountId != Guid.Empty) ? chartOfAccountId : null;
                    teller.ShortageChartOfAccountId = (shortageChartOfAccountId != null && shortageChartOfAccountId != Guid.Empty) ? shortageChartOfAccountId : null;
                    teller.ExcessChartOfAccountId = (excessChartOfAccountId != null && excessChartOfAccountId != Guid.Empty) ? excessChartOfAccountId : null;
                    break;
                case TellerType.AutomatedTellerMachine:
                case TellerType.InhousePointOfSale:
                    teller.ChartOfAccountId = (chartOfAccountId != null && chartOfAccountId != Guid.Empty) ? chartOfAccountId : null;
                    break;
                case TellerType.AgentPointOfSale:
                    teller.FloatCustomerAccountId = (floatCustomerAccountId != null && floatCustomerAccountId != Guid.Empty) ? floatCustomerAccountId : null;
                    teller.CommissionCustomerAccountId = (commissionCustomerAccountId != null && commissionCustomerAccountId != Guid.Empty) ? commissionCustomerAccountId : null;
                    break;
                default:
                    break;
            }

            teller.Description = description;

            teller.Range = range;

            teller.MiniStatementItemsCap = miniStatementItemsCap;

            teller.Reference = reference;

            teller.CreatedDate = DateTime.Now;

            return teller;
        }
    }
}
