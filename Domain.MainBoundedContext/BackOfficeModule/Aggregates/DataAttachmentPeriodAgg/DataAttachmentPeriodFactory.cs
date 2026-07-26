using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentPeriodAgg
{
    public static class DataAttachmentPeriodFactory
    {
        public static DataAttachmentPeriod CreateDataAttachmentPeriod(Guid postingPeriodId, int month, string remarks)
        {
            var dataAttachmentPeriod = new DataAttachmentPeriod();

            dataAttachmentPeriod.GenerateNewIdentity();

            dataAttachmentPeriod.PostingPeriodId = postingPeriodId;

            dataAttachmentPeriod.Month = (byte)month;

            dataAttachmentPeriod.Remarks = remarks;

            dataAttachmentPeriod.CreatedDate = DateTime.Now;

            return dataAttachmentPeriod;
        }
    }
}
