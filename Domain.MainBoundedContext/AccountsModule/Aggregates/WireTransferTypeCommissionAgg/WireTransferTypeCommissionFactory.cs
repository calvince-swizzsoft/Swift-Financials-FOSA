using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeCommissionAgg
{
    public static class WireTransferTypeCommissionFactory
    {
        public static WireTransferTypeCommission CreateWireTransferTypeCommission(Guid wireTransferTypeId, Guid commissionId)
        {
            var wireTransferTypeCommission = new WireTransferTypeCommission();

            wireTransferTypeCommission.GenerateNewIdentity();

            wireTransferTypeCommission.WireTransferTypeId = wireTransferTypeId;

            wireTransferTypeCommission.CommissionId = commissionId;

            wireTransferTypeCommission.CreatedDate = DateTime.Now;

            return wireTransferTypeCommission;
        }
    }
}
