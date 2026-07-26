using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanGuarantorAttachmentHistoryEntryDTO : BindingModelBase<LoanGuarantorAttachmentHistoryEntryDTO>
    {
        public LoanGuarantorAttachmentHistoryEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Loan Guarantor Attachment History")]
        public Guid LoanGuarantorAttachmentHistoryId { get; set; }

        [DataMember]
        [Display(Name = "Loan Guarantor")]
        public Guid LoanGuarantorId { get; set; }

        [DataMember]
        [Display(Name = "Loan Number")]
        public int LoanGuarantorLoanCaseCaseNumber { get; set; }

        [DataMember]
        [Display(Name = "Loan Number")]
        public string PaddedLoanGuarantorLoanCaseCaseNumber
        {
            get
            {
                return string.Format("{0}", LoanGuarantorLoanCaseCaseNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Destination Customer Account")]
        public Guid DestinationCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid DestinationCustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int DestinationCustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product")]
        public Guid DestinationCustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public int DestinationCustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Product Name")]
        public string DestinationCustomerAccountCustomerAccountTypeTargetProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account")]
        public Guid DestinationCustomerAccountCustomerAccountTypeTargetProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public int DestinationCustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
        public string FullAccountNumber
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}",
                    DestinationCustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                    DestinationCustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                    DestinationCustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                    DestinationCustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid DestinationCustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int DestinationCustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int DestinationCustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string DestinationCustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), DestinationCustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)DestinationCustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string DestinationCustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string DestinationCustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", DestinationCustomerAccountCustomerIndividualSalutationDescription, DestinationCustomerAccountCustomerIndividualFirstName, DestinationCustomerAccountCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string DestinationCustomerAccountCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string DestinationCustomerAccountCustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string DestinationCustomerAccountCustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string DestinationCustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string DestinationCustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string DestinationCustomerAccountCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Principal Attached")]
        public decimal PrincipalAttached { get; set; }

        [DataMember]
        [Display(Name = "Interest Attached")]
        public decimal InterestAttached { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public Guid AccountAlertCustomerId { get; set; }

        [DataMember]
        public string AccountAlertPrimaryDescription { get; set; }

        [DataMember]
        public string AccountAlertSecondaryDescription { get; set; }

        [DataMember]
        public string AccountAlertReference { get; set; }

        [DataMember]
        public decimal AccountAlertTotalValue { get; set; }
    }
}
