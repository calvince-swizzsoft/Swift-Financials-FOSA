using System.ComponentModel;

namespace SwiftFinancials.Presentation.Infrastructure.Models
{
    public class InternetBankingViewModel
    {
        public string Data { get; set; }

        public override string ToString()
        {
            return Data;
        }
    }

    public enum RequestType
    {
        [Description("Authentication")]
        Authentication = 1,
        [Description("Account Statement")]
        AccountStatement = 2
    }
}
