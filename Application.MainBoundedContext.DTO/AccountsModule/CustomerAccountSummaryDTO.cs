using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CustomerAccountSummaryDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Customer")]
        public Guid CustomerId { get; set; }

        [Display(Name = "Customer Type")]
        public byte CustomerType { get; set; }

        [Display(Name = "Customer Type")]
        public string CustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), (int)CustomerType) ? EnumHelper.GetDescription((CustomerType)CustomerType) : string.Empty;
            }
        }

        [Display(Name = "Customer Serial Number")]
        public int CustomerSerialNumber { get; set; }

        [Display(Name = "Customer Salutation")]
        public byte CustomerIndividualSalutation { get; set; }

        [Display(Name = "Customer Salutation")]
        public string CustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), (int)CustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerIndividualSalutation) : string.Empty;
            }
        }

        [Display(Name = "Customer First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [Display(Name = "Customer Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [Display(Name = "Group Name")]
        public string CustomerNonIndividualDescription { get; set; }

        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [Display(Name = "Personal File Number")]
        public string CustomerReference3 { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", CustomerIndividualSalutationDescription, CustomerIndividualFirstName, CustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [Display(Name = "Full Account Number")]
        public string FullAccountNumber
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}",
                            BranchCode.ToString().PadLeft(3, '0'),
                            CustomerSerialNumber.ToString().PadLeft(7, '0'),
                            CustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            CustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [Display(Name = "Branch")]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch Code")]
        public short BranchCode { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [Display(Name = "Product Code")]
        public byte CustomerAccountTypeProductCode { get; set; }

        [Display(Name = "Target Product")]
        public Guid CustomerAccountTypeTargetProductId { get; set; }

        [Display(Name = "Target Product Code")]
        public short CustomerAccountTypeTargetProductCode { get; set; }

        [Display(Name = "Scored Loan Disbursement Product Code")]
        public short ScoredLoanDisbursementProductCode { get; set; }

        [Display(Name = "Scored Loan Limit")]
        public decimal ScoredLoanLimit { get; set; }

        [Display(Name = "Scored Loan Limit Remarks")]
        public string ScoredLoanLimitRemarks { get; set; }

        [Display(Name = "Scored Loan Limit Date")]
        public DateTime? ScoredLoanLimitDate { get; set; }

        [Display(Name = "Status")]
        public byte Status { get; set; }

        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerAccountStatus), (int)Status) ? EnumHelper.GetDescription((CustomerAccountStatus)Status) : string.Empty;
            }
        }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Record Status")]
        public byte RecordStatus { get; set; }

        [Display(Name = "Record Status")]
        public string RecordStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RecordStatus), (int)RecordStatus) ? EnumHelper.GetDescription((RecordStatus)RecordStatus) : string.Empty;
            }
        }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
