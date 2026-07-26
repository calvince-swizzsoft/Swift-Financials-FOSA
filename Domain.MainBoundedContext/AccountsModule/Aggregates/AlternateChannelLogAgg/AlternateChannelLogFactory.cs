using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogAgg
{
    public static class AlternateChannelLogFactory
    {
        public static AlternateChannelLog CreateAlternateChannelLog(int alternateChannelType, ISO8583 ISO8583, SPARROW SPARROW, WALLET WALLET, string systemTraceAuditNumber)
        {
            var alternateChannelLog = new AlternateChannelLog();

            alternateChannelLog.GenerateNewIdentity();

            alternateChannelLog.AlternateChannelType = (short)alternateChannelType;

            alternateChannelLog.ISO8583 = ISO8583;

            alternateChannelLog.SPARROW = SPARROW;

            alternateChannelLog.WALLET = WALLET;

            alternateChannelLog.SystemTraceAuditNumber = systemTraceAuditNumber;

            alternateChannelLog.CreatedDate = DateTime.Now;

            return alternateChannelLog;
        }

        public static AlternateChannelLog CreateAlternateChannelLog(int alternateChannelType, ISO8583 ISO8583, string systemTraceAuditNumber)
        {
            var alternateChannelLog = new AlternateChannelLog();

            alternateChannelLog.GenerateNewIdentity();

            alternateChannelLog.AlternateChannelType = (short)alternateChannelType;

            alternateChannelLog.ISO8583 = ISO8583;

            alternateChannelLog.SPARROW = new SPARROW(null, null, null, null, null, null, null, 0m);

            alternateChannelLog.WALLET = new WALLET(null, null, null, null, null, null, 0m, null);

            alternateChannelLog.SystemTraceAuditNumber = systemTraceAuditNumber;

            alternateChannelLog.CreatedDate = DateTime.Now;

            return alternateChannelLog;
        }

        public static AlternateChannelLog CreateAlternateChannelLog(int alternateChannelType, SPARROW SPARROW, string systemTraceAuditNumber)
        {
            var alternateChannelLog = new AlternateChannelLog();

            alternateChannelLog.GenerateNewIdentity();

            alternateChannelLog.AlternateChannelType = (short)alternateChannelType;

            alternateChannelLog.ISO8583 = new ISO8583(null, null, null, null, null, 0m);

            alternateChannelLog.SPARROW = SPARROW;

            alternateChannelLog.WALLET = new WALLET(null, null, null, null, null, null, 0m, null);

            alternateChannelLog.SystemTraceAuditNumber = systemTraceAuditNumber;

            alternateChannelLog.CreatedDate = DateTime.Now;

            return alternateChannelLog;
        }

        public static AlternateChannelLog CreateAlternateChannelLog(int alternateChannelType, WALLET WALLET, string systemTraceAuditNumber)
        {
            var alternateChannelLog = new AlternateChannelLog();

            alternateChannelLog.GenerateNewIdentity();

            alternateChannelLog.AlternateChannelType = (short)alternateChannelType;

            alternateChannelLog.ISO8583 = new ISO8583(null, null, null, null, null, 0m);

            alternateChannelLog.SPARROW = new SPARROW(null, null, null, null, null, null, null, 0m);

            alternateChannelLog.WALLET = WALLET;

            alternateChannelLog.SystemTraceAuditNumber = systemTraceAuditNumber;

            alternateChannelLog.CreatedDate = DateTime.Now;

            return alternateChannelLog;
        }
    }
}
