using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequeAgg
{
    public static class ExternalChequeFactory
    {
        public static ExternalCheque CreateExternalCheque(Guid tellerId, Guid? chequeTypeId, Guid? customerAccountId, string number, decimal amount, string drawer, string drawerBank, string drawerBankBranch, DateTime writeDate, DateTime maturityDate)
        {
            var externalCheque = new ExternalCheque();

            externalCheque.GenerateNewIdentity();

            externalCheque.TellerId = tellerId;

            externalCheque.ChequeTypeId = (chequeTypeId != null && chequeTypeId != Guid.Empty) ? chequeTypeId : null;

            externalCheque.CustomerAccountId = (customerAccountId != null && customerAccountId != Guid.Empty) ? customerAccountId : null;

            externalCheque.Number = number;

            externalCheque.Amount = amount;

            externalCheque.Drawer = drawer;

            externalCheque.DrawerBank = drawerBank;

            externalCheque.DrawerBankBranch = drawerBankBranch;

            externalCheque.WriteDate = writeDate;

            externalCheque.MaturityDate = maturityDate;

            externalCheque.CreatedDate = DateTime.Now;

            return externalCheque;
        }
    }
}
