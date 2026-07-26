using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;  

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogAgg
{
    public static class AlternateChannelLogSpecifications
    {
        public static Specification<AlternateChannelLog> DefaultSpec()
        {
            Specification<AlternateChannelLog> specification = new TrueSpecification<AlternateChannelLog>();

            return specification;
        }

        public static Specification<AlternateChannelLog> ISO8583(int alternateChannelType, string messageTypeIdentification, string primaryAccountNumber, string systemTraceAuditNumber, string retrievalReferenceNumber, int daysCap)
        {
            Specification<AlternateChannelLog> specification = new DirectSpecification<AlternateChannelLog>(x => (SqlFunctions.DateDiff("DD", x.CreatedDate, SqlFunctions.GetDate()) <= daysCap) &&
            x.AlternateChannelType == alternateChannelType &&
            x.ISO8583.MessageTypeIdentification == messageTypeIdentification &&
            x.ISO8583.PrimaryAccountNumber == primaryAccountNumber &&
            x.ISO8583.SystemTraceAuditNumber == systemTraceAuditNumber &&
            x.ISO8583.RetrievalReferenceNumber == retrievalReferenceNumber);

            return specification;
        }

        public static Specification<AlternateChannelLog> ISO8583(int alternateChannelType, string primaryAccountNumber, string systemTraceAuditNumber, string retrievalReferenceNumber, int daysCap)
        {
            Specification<AlternateChannelLog> specification = new DirectSpecification<AlternateChannelLog>(x => (SqlFunctions.DateDiff("DD", x.CreatedDate, SqlFunctions.GetDate()) <= daysCap) &&
            x.AlternateChannelType == alternateChannelType &&
            x.ISO8583.PrimaryAccountNumber == primaryAccountNumber &&
            x.ISO8583.SystemTraceAuditNumber == systemTraceAuditNumber &&
            x.ISO8583.RetrievalReferenceNumber == retrievalReferenceNumber);

            return specification;
        }

        public static Specification<AlternateChannelLog> ISO8583(int alternateChannelType, string primaryAccountNumber, string retrievalReferenceNumber, int daysCap)
        {
            Specification<AlternateChannelLog> specification = new DirectSpecification<AlternateChannelLog>(x => (SqlFunctions.DateDiff("DD", x.CreatedDate, SqlFunctions.GetDate()) <= daysCap) &&
            x.AlternateChannelType == alternateChannelType &&
            x.ISO8583.PrimaryAccountNumber == primaryAccountNumber &&
            x.ISO8583.RetrievalReferenceNumber == retrievalReferenceNumber);

            return specification;
        }

        public static Specification<AlternateChannelLog> WALLET(string messageTypeIdentification, string primaryAccountNumber, string systemTraceAuditNumber, string retrievalReferenceNumber, int daysCap)
        {
            Specification<AlternateChannelLog> specification = new DirectSpecification<AlternateChannelLog>(x => (SqlFunctions.DateDiff("DD", x.CreatedDate, SqlFunctions.GetDate()) <= daysCap) &&
            (x.AlternateChannelType == (int)AlternateChannelType.Citius || x.AlternateChannelType == (int)AlternateChannelType.PesaPepe) &&
            x.WALLET.MessageTypeIdentification == messageTypeIdentification &&
            x.WALLET.PrimaryAccountNumber == primaryAccountNumber && x.WALLET.SystemTraceAuditNumber == systemTraceAuditNumber);

            return specification;
        }

        public static Specification<AlternateChannelLog> WALLET(string primaryAccountNumber, string systemTraceAuditNumber, string retrievalReferenceNumber, int daysCap)
        {
            Specification<AlternateChannelLog> specification = new DirectSpecification<AlternateChannelLog>(x => (SqlFunctions.DateDiff("DD", x.CreatedDate, SqlFunctions.GetDate()) <= daysCap) &&
            (x.AlternateChannelType == (int)AlternateChannelType.Citius || x.AlternateChannelType == (int)AlternateChannelType.PesaPepe) &&
            x.WALLET.PrimaryAccountNumber == primaryAccountNumber && x.WALLET.SystemTraceAuditNumber == systemTraceAuditNumber);

            return specification;
        }

        public static Specification<AlternateChannelLog> SPARROW(string messageType, string src_imd, string deviceId, string date, string time, string cardNumber, int daysCap)
        {
            Specification<AlternateChannelLog> specification = new DirectSpecification<AlternateChannelLog>(x => (SqlFunctions.DateDiff("DD", x.CreatedDate, SqlFunctions.GetDate()) <= daysCap) &&
            x.AlternateChannelType == (int)AlternateChannelType.Sparrow &&
            x.SPARROW.MessageType == messageType &&
            x.SPARROW.SRCIMD == src_imd &&
            x.SPARROW.DeviceId == deviceId &&
            x.SPARROW.Date == date &&
            x.SPARROW.Time == time &&
            x.SPARROW.CardNumber == cardNumber);

            return specification;
        }

        public static Specification<AlternateChannelLog> SPARROW(string src_imd, string deviceId, string date, string time, string cardNumber, int daysCap)
        {
            Specification<AlternateChannelLog> specification = new DirectSpecification<AlternateChannelLog>(x => (SqlFunctions.DateDiff("DD", x.CreatedDate, SqlFunctions.GetDate()) <= daysCap) &&
            x.AlternateChannelType == (int)AlternateChannelType.Sparrow &&
            x.SPARROW.SRCIMD == src_imd &&
            x.SPARROW.DeviceId == deviceId &&
            x.SPARROW.Date == date &&
            x.SPARROW.Time == time &&
            x.SPARROW.CardNumber == cardNumber);

            return specification;
        }

        public static Specification<AlternateChannelLog> AlternateChannelLog(int alternateChannelType, DateTime startDate, DateTime endDate)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<AlternateChannelLog> specification = new DirectSpecification<AlternateChannelLog>(x => x.AlternateChannelType == alternateChannelType && x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            return specification;
        }

        public static ISpecification<AlternateChannelLog> AlternateChannelLogWithId(params Guid[] alternateChannelLogIds)
        {
            Specification<AlternateChannelLog> specification = new TrueSpecification<AlternateChannelLog>();

            var alternateChannelLogIdSpecs = new List<Specification<AlternateChannelLog>>();

            if (alternateChannelLogIds != null)
            {
                Array.ForEach(alternateChannelLogIds, (alternateChannelLogId) =>
                {
                    var alternateChannelLogIdSpec = new DirectSpecification<AlternateChannelLog>(x => x.Id == alternateChannelLogId);

                    alternateChannelLogIdSpecs.Add(alternateChannelLogIdSpec);
                });

                if (alternateChannelLogIdSpecs.Any())
                {
                    var spec0 = alternateChannelLogIdSpecs[0];

                    for (int i = 1; i < alternateChannelLogIdSpecs.Count; i++)
                    {
                        spec0 |= alternateChannelLogIdSpecs[i];
                    }

                    specification &= (spec0);
                }
            }

            return specification;
        }
    }
}
