using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace SwiftFinancials.Presentation.Infrastructure.Models
{
    public class InstantPaymentNotification
    {
        public string Id { get; set; }

        public string MSISDN { get; set; }

        public string BusinessShortCode { get; set; }

        public string InvoiceNumber { get; set; }

        public string TransID { get; set; }

        public string TransAmount { get; set; }

        public string ThirdPartyTransID { get; set; }

        public string TransTime { get; set; }

        public string BillRefNumber { get; set; }

        public string OrgAccountBalance { get; set; }

        public string AppDomainName { get; set; }

        public List<KYCInfo> KYCInfoList { get; set; }

        public override string ToString()
        {
            var javaScriptSerializer = new JavaScriptSerializer();

            return javaScriptSerializer.Serialize(this);
        }
    }

    public class KYCInfo
    {
        public string KYCName { get; set; }

        public string KYCValue { get; set; }
    }
}
