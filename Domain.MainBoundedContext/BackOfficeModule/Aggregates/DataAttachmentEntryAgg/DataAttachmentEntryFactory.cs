using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentEntryAgg
{
    public static class DataAttachmentEntryFactory
    {
        public static DataAttachmentEntry CreateDataAttachmentEntry(Guid dataAttachmentPeriodId, Guid customerAccountId, int transactionType, int sequenceNumber, decimal newAmount, decimal currentAmount, decimal newBalance, decimal currentBalance, decimal newAbility, decimal currentAbility, string remarks)
        {
            var dataAttachmentEntry = new DataAttachmentEntry();

            dataAttachmentEntry.GenerateNewIdentity();

            dataAttachmentEntry.DataAttachmentPeriodId = dataAttachmentPeriodId;

            dataAttachmentEntry.CustomerAccountId = customerAccountId;

            dataAttachmentEntry.TransactionType = (byte)transactionType;

            dataAttachmentEntry.SequenceNumber = sequenceNumber;

            dataAttachmentEntry.NewAmount = newAmount;

            dataAttachmentEntry.CurrentAmount = currentAmount;

            dataAttachmentEntry.CurrentBalance = currentBalance;

            dataAttachmentEntry.NewBalance = newBalance;

            dataAttachmentEntry.NewAbility = newAbility;

            dataAttachmentEntry.CurrentAbility = currentAbility;

            dataAttachmentEntry.Remarks = remarks;

            dataAttachmentEntry.CreatedDate = DateTime.Now;

            return dataAttachmentEntry;
        }
    }
}
