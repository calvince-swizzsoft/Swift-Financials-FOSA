using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.InHouseChequeAgg
{
    public static class InHouseChequeFactory
    {
        public static InHouseCheque CreateInHouseCheque(Guid branchId, Guid? chequeTypeId, Guid debitChartOfAccountId, Guid? debitCustomerAccountId, int funding, decimal amount, string payee, string reference, bool chargeable)
        {
            var inHouseCheque = new InHouseCheque();

            inHouseCheque.GenerateNewIdentity();

            inHouseCheque.BranchId = branchId;

            inHouseCheque.ChequeTypeId = (chequeTypeId != null && chequeTypeId != Guid.Empty) ? chequeTypeId : null;

            inHouseCheque.DebitChartOfAccountId = debitChartOfAccountId;

            inHouseCheque.DebitChartOfAccountId = debitChartOfAccountId;

            inHouseCheque.DebitCustomerAccountId = debitCustomerAccountId;

            inHouseCheque.Funding = (byte)funding;

            inHouseCheque.Amount = amount;

            inHouseCheque.Payee = payee;

            inHouseCheque.Reference = reference;

            inHouseCheque.Chargeable = debitCustomerAccountId != null ? true : chargeable;

            inHouseCheque.CreatedDate = DateTime.Now;

            return inHouseCheque;
        }
    }
}
