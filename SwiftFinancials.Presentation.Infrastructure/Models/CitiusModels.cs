using System.ComponentModel;

namespace SwiftFinancials.Presentation.Infrastructure.Models
{
    public class CitiusViewModel
    {
        public string Data { get; set; }

        public override string ToString()
        {
            return Data;
        }
    }

    public enum FUNCTIONCODE
    {
        [Description("Search Member")]
        SearchMember = 1,
        [Description("Get Accounts")]
        GetAccounts = 2,
        [Description("Accept Saving Deposit")]
        AcceptSavingDeposit = 5,
        [Description("Accept Loan Installment")]
        AcceptLoanInstallment = 6,
        [Description("Accept Saving Withdraw")]
        AcceptSavingWithdraw = 7,
        [Description("Get All Branches")]
        GetAllBranches = 11,
        [Description("Get Account Info")]
        GetAccountInfo = 12,
        [Description("Get Mini Statement")]
        GetMiniStatement = 13,
        [Description("Create New Customer")]
        CreateNewCustomer = 14,
        [Description("Validate CBS User")]
        ValidateCBSUser = 15,
        [Description("Send OTP")]
        SendOTP = 19,
        [Description("Get Schemes")]
        GetSchemes = 20,
    }
}
