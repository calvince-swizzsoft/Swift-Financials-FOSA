using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertAgg
{
    public static class TextAlertSpecifications
    {
        public static Specification<TextAlert> DefaultSpec()
        {
            Specification<TextAlert> specification = new TrueSpecification<TextAlert>();

            return specification;
        }

        public static ISpecification<TextAlert> TextAlertWithMessageReference(string messageReference)
        {
            Specification<TextAlert> specification = new TrueSpecification<TextAlert>();

            specification &= new DirectSpecification<TextAlert>(x => x.TextMessage.Reference == messageReference);

            return specification;
        }

        public static ISpecification<TextAlert> TextAlertWithDLRStatus(int dlrStatus)
        {
            Specification<TextAlert> specification = new TrueSpecification<TextAlert>();

            specification &= new DirectSpecification<TextAlert>(x => x.TextMessage.DLRStatus == dlrStatus);

            return specification;
        }

        public static ISpecification<TextAlert> TextAlertWithDLRStatusAndOrigin(int dlrStatus, int origin)
        {
            Specification<TextAlert> specification = new TrueSpecification<TextAlert>();

            specification &= new DirectSpecification<TextAlert>(x => x.TextMessage.DLRStatus == dlrStatus && x.TextMessage.Origin == origin);

            return specification;
        }

        public static Specification<TextAlert> TextAlertFullText(int dlrStatus, string text, int daysCap)
        {
            Specification<TextAlert> specification = new DirectSpecification<TextAlert>(x => (SqlFunctions.DateDiff("DD", x.CreatedDate, SqlFunctions.GetDate()) <= daysCap) &&
            x.TextMessage.DLRStatus == dlrStatus);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var recipientSpec = new DirectSpecification<TextAlert>(c => c.TextMessage.Recipient.Contains(text));
                var bodySpec = new DirectSpecification<TextAlert>(c => c.TextMessage.Body.Contains(text));

                specification &= (recipientSpec | bodySpec);
            }

            return specification;
        }

        public static Specification<TextAlert> TextAlertFullText(int dlrStatus, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<TextAlert> specification = new DirectSpecification<TextAlert>(x => x.TextMessage.DLRStatus == dlrStatus && x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var recipientSpec = new DirectSpecification<TextAlert>(c => c.TextMessage.Recipient.Contains(text));
                var bodySpec = new DirectSpecification<TextAlert>(c => c.TextMessage.Body.Contains(text));

                specification &= (recipientSpec | bodySpec);
            }

            return specification;
        }
    }
}
