using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ImprestSurrender
{
    public class ImprestSurrenderFactory
    {

        public static ImprestSurrender CreateImprestSurrender(String surrenderNo, DateTime surrenderDate, Decimal amountIssued, Decimal amountSpent, Decimal amountReturned, String status, ServiceHeader serviceHeader)
        {
            var surrender = new ImprestSurrender();

            surrender.SurrenderNo = surrenderNo;

            surrender.SurrenderDate = surrenderDate;

            surrender.AmountIssued = amountIssued;

            surrender.AmountSpent = amountSpent;

            surrender.AmountReturned = amountReturned;

            surrender.Status = status;

            surrender.CreatedDate = DateTime.Now;


            surrender.GenerateNewIdentity();

            

            return surrender;
        }
    }
}
