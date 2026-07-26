using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentPeriodAgg
{
    public static class DataAttachmentPeriodSpecifications
    {
        public static Specification<DataAttachmentPeriod> DefaultSpec()
        {
            Specification<DataAttachmentPeriod> specification = new TrueSpecification<DataAttachmentPeriod>();

            return specification;
        }

        public static ISpecification<DataAttachmentPeriod> DataAttachmentPeriodWithPostingPeriodIdAndMonth(Guid postingPeriodId, int month)
        {
            Specification<DataAttachmentPeriod> specification = new DirectSpecification<DataAttachmentPeriod>(x => x.PostingPeriodId == postingPeriodId && x.Month == month);

            return specification;
        }

        public static Specification<DataAttachmentPeriod> DataAttachmentPeriodFullText(string text)
        {
            Specification<DataAttachmentPeriod> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var remarksSpec = new DirectSpecification<DataAttachmentPeriod>(c => c.Remarks.Contains(text));

                specification &= (remarksSpec);
            }

            return specification;
        }

        public static Specification<DataAttachmentPeriod> CurrentDataAttachmentPeriod()
        {
            return new DirectSpecification<DataAttachmentPeriod>(x => x.Status == (int)DataAttachmentPeriodStatus.Open && x.IsActive);
        }

        public static Specification<DataAttachmentPeriod> DataAttachmentPeriodFullText(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<DataAttachmentPeriod> specification = new DirectSpecification<DataAttachmentPeriod>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var remarksSpec = new DirectSpecification<DataAttachmentPeriod>(c => c.Remarks.Contains(text));

                var createdBySpec = new DirectSpecification<DataAttachmentPeriod>(c => c.CreatedBy.Contains(text));

                specification &= (remarksSpec | createdBySpec);
            }

            return specification;
        }
    }
}
