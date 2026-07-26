using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ImprestAgg
{
    public class ImprestFactory 
    {

                 public static Imprest CreateImprest(string no, string employeeNo, string employeeName, string purpose, decimal amountRequested, DateTime requestDate, Boolean posted, Guid BankChartOfAccountid, ServiceHeader serviceHeader)
            {
                var imprest = new Imprest();

            imprest.Posted = posted;    
            
            imprest.No = no;

            imprest.EmployeeName = employeeName;

            imprest.EmployeeNo = employeeNo;

            imprest.Purpose = purpose;

            imprest.AmountRequested = amountRequested;

            imprest.RequestDate = requestDate;

            imprest.BankChartOfAccountId = BankChartOfAccountid;


            //imprest.

            imprest.CreatedDate = DateTime.Now;

            imprest.GenerateNewIdentity();

                return imprest;

            }
       



      //public static Imprest UpdateImprest(decimal amountRequested, decimal amountSpent, decimal amountReturned, decimal reimbursementAmount, DateTime surrenderDate, ServiceHeader serviceHeader)
      //{
      //      var imprest = new Imprest();

      //      imprest.AmountRequested = amountRequested;

      //      imprest.AmountSpent = amountSpent;

      //      imprest.AmountReturned = amountReturned;

      //      imprest.ReimbursementAmount = reimbursementAmount;

      //      imprest.SurrenderDate = surrenderDate;

      //      return imprest;
      //  }
    }


}
