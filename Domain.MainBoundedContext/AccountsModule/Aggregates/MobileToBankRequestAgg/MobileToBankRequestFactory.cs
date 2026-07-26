using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.MobileToBankRequestAgg
{
    public static class MobileToBankRequestFactory
    {
        public static MobileToBankRequest CreateMobileToBankRequest(Guid? customerAccountId, Guid? chartOfAccountId, string systemTraceAuditNumber, string MSISDN, string businessShortCode, string invoiceNumber, string transID, decimal transAmount, string thirdPartyTransID, string transTime, string billRefNumber, decimal orgAccountBalance, string kycInfo)
        {
            var mobileToBankRequest = new MobileToBankRequest();

            mobileToBankRequest.GenerateNewIdentity();

            mobileToBankRequest.CustomerAccountId = (customerAccountId != null && customerAccountId != Guid.Empty) ? customerAccountId : null;

            mobileToBankRequest.ChartOfAccountId = (chartOfAccountId != null && chartOfAccountId != Guid.Empty) ? chartOfAccountId : null;

            mobileToBankRequest.SystemTraceAuditNumber = systemTraceAuditNumber;

            mobileToBankRequest.MSISDN = MSISDN;

            mobileToBankRequest.BusinessShortCode = businessShortCode;

            mobileToBankRequest.InvoiceNumber = invoiceNumber;

            mobileToBankRequest.TransID = transID;

            mobileToBankRequest.TransAmount = transAmount;

            mobileToBankRequest.ThirdPartyTransID = thirdPartyTransID;

            mobileToBankRequest.TransTime = transTime;

            mobileToBankRequest.BillRefNumber = billRefNumber;

            mobileToBankRequest.OrgAccountBalance = orgAccountBalance;

            mobileToBankRequest.KYCInfo = kycInfo;

            mobileToBankRequest.CreatedDate = DateTime.Now;

            return mobileToBankRequest;
        }
    }
}
