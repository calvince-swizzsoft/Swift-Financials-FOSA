using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanGuarantorAttachmentHistoryDTO : BindingModelBase<LoanGuarantorAttachmentHistoryDTO>
    {
        public LoanGuarantorAttachmentHistoryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Source Customer Account")]
        public Guid SourceCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid SourceCustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int SourceCustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product")]
        public Guid SourceCustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public int SourceCustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Product Name")]
        public string SourceCustomerAccountCustomerAccountTypeTargetProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account")]
        public Guid SourceCustomerAccountCustomerAccountTypeTargetProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public int SourceCustomerAccountCustomerAccountTypeProductCode { get; set; }
        
        [DataMember]
        [Display(Name = "Full Account Number")]
        public string FullAccountNumber
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}",
                    SourceCustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                    SourceCustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                    SourceCustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                    SourceCustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid SourceCustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int SourceCustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int SourceCustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string SourceCustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), SourceCustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)SourceCustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string SourceCustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string SourceCustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", SourceCustomerAccountCustomerIndividualSalutationDescription, SourceCustomerAccountCustomerIndividualFirstName, SourceCustomerAccountCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string SourceCustomerAccountCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string SourceCustomerAccountCustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string SourceCustomerAccountCustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string SourceCustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string SourceCustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string SourceCustomerAccountCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Principal Balance")]
        public decimal PrincipalBalance { get; set; }

        [DataMember]
        [Display(Name = "Interest Balance")]
        public decimal InterestBalance { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanGuarantorAttachmentHistoryStatus), Status) ? EnumHelper.GetDescription((LoanGuarantorAttachmentHistoryStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
