using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogArchiveAgg
{
    public static class AlternateChannelLogArchiveFactory
    {
        public static AlternateChannelLogArchive CreateAlternateChannelLogArchive(int alternateChannelType, ISO8583 ISO8583, SPARROW SPARROW, WALLET WALLET, string systemTraceAuditNumber)
        {
            var alternateChannelLogArchive = new AlternateChannelLogArchive();

            alternateChannelLogArchive.GenerateNewIdentity();

            alternateChannelLogArchive.AlternateChannelType = (short)alternateChannelType;

            alternateChannelLogArchive.ISO8583 = ISO8583;

            alternateChannelLogArchive.SPARROW = SPARROW;

            alternateChannelLogArchive.WALLET = WALLET;

            alternateChannelLogArchive.SystemTraceAuditNumber = systemTraceAuditNumber;

            alternateChannelLogArchive.CreatedDate = DateTime.Now;

            return alternateChannelLogArchive;
        }
    }
}
