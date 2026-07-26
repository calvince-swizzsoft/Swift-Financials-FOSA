using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.EmailAlertAgg
{
    public static class EmailAlertSpecifications
    {
        public static Specification<EmailAlert> DefaultSpec()
        {
            Specification<EmailAlert> specification = new TrueSpecification<EmailAlert>();

            return specification;
        }

        public static ISpecification<EmailAlert> EmailAlertWithDLRStatus(int dlrStatus)
        {
            Specification<EmailAlert> specification = new TrueSpecification<EmailAlert>();

            specification &= new DirectSpecification<EmailAlert>(x => x.MailMessage.DLRStatus == dlrStatus);

            return specification;
        }

        public static ISpecification<EmailAlert> EmailAlertWithDLRStatusAndOrigin(int dlrStatus, int origin)
        {
            Specification<EmailAlert> specification = new TrueSpecification<EmailAlert>();

            specification &= new DirectSpecification<EmailAlert>(x => x.MailMessage.DLRStatus == dlrStatus && x.MailMessage.Origin == origin);

            return specification;
        }

        public static Specification<EmailAlert> EmailAlertFullText(int dlrStatus, string text, int daysCap)
        {
            Specification<EmailAlert> specification = new DirectSpecification<EmailAlert>(x => (SqlFunctions.DateDiff("DD", x.CreatedDate, SqlFunctions.GetDate()) <= daysCap) &&
            x.MailMessage.DLRStatus == dlrStatus);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var toSpec = new DirectSpecification<EmailAlert>(c => c.MailMessage.To.Contains(text));
                var subjectSpec = new DirectSpecification<EmailAlert>(c => c.MailMessage.Subject.Contains(text));
                var bodySpec = new DirectSpecification<EmailAlert>(c => c.MailMessage.Body.Contains(text));

                specification &= (toSpec | subjectSpec | bodySpec);
            }

            return specification;
        }

        public static Specification<EmailAlert> EmailAlertFullText(int dlrStatus, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<EmailAlert> specification = new DirectSpecification<EmailAlert>(x => x.MailMessage.DLRStatus == dlrStatus && x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var toSpec = new DirectSpecification<EmailAlert>(c => c.MailMessage.To.Contains(text));
                var subjectSpec = new DirectSpecification<EmailAlert>(c => c.MailMessage.Subject.Contains(text));
                var bodySpec = new DirectSpecification<EmailAlert>(c => c.MailMessage.Body.Contains(text));

                specification &= (toSpec | subjectSpec | bodySpec);
            }

            return specification;
        }
    }
}
