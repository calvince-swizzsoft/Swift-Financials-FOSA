using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentPeriodAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentEntryAgg
{
    public class DataAttachmentEntry : Domain.Seedwork.Entity
    {
        public Guid DataAttachmentPeriodId { get; set; }

        public virtual DataAttachmentPeriod DataAttachmentPeriod { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public byte TransactionType { get; set; }

        public int SequenceNumber { get; set; }

        public decimal NewAmount { get; set; }

        public decimal CurrentAmount { get; set; }

        public decimal NewBalance { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal NewAbility { get; set; }

        public decimal CurrentAbility { get; set; }

        public string Remarks { get; set; }

        

        
    }
}
