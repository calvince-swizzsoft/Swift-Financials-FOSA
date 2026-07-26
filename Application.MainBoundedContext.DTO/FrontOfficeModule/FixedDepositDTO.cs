using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;




namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class FixedDepositDTO : BindingModelBase<FixedDepositDTO>
    {
        public FixedDepositDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        [ValidGuid]
        public Guid? FixedDepositTypeId { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string FixedDepositTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        public bool BranchCompanyRecoverArrearsOnCashDeposit { get; set; }

        [DataMember]
        public bool BranchCompanyRecoverArrearsOnExternalChequeClearance { get; set; }

        [DataMember]
        public bool BranchCompanyRecoverArrearsOnFixedDepositPayment { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        [ValidGuid]
        public Guid CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Branch Id")]
        public Guid CustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int CustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int CustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public int CustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Target Product Id")]
        public Guid CustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Target Product Code")]
        public int CustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
        public string CustomerAccountFullAccountNumber
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}",
                            CustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                            CustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                            CustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            CustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid CustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int CustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerAccountCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), CustomerAccountCustomerType) ? EnumHelper.GetDescription((CustomerType)CustomerAccountCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Status")]
        public int CustomerAccountStatus { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string CustomerAccountStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerAccountStatus), CustomerAccountStatus) ? EnumHelper.GetDescription((CustomerAccountStatus)CustomerAccountStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int CustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string CustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string CustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string CustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string PaddedCustomerAccountCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", CustomerAccountCustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string CustomerAccountCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Group Name")]
        public string CustomerAccountCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Registration Number")]
        public string CustomerAccountCustomerNonIndividualRegistrationNumber { get; set; }

        [DataMember]
        [Display(Name = "Personal Identification Number")]
        public string CustomerAccountCustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Date Established")]
        public DateTime? CustomerAccountCustomerNonIndividualDateEstablished { get; set; }
        
        [DataMember]
        [Display(Name = "Customer Identity Card Number")]
        public string CustomerAccountCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerAccountCustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerAccountCustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", CustomerAccountCustomerIndividualSalutationDescription, CustomerAccountCustomerIndividualFirstName, CustomerAccountCustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CustomerAccountCustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerAccountCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Identification Number")]
        public string CustomerAccountCustomerIdentificationNumber
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerAccountCustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = CustomerAccountCustomerIndividualIdentityCardNumber;
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CustomerAccountCustomerNonIndividualRegistrationNumber;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Product Name")]
        public string ProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account")]
        public Guid ProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Code")]
        public int ProductChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Name")]
        public string ProductChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public int Category { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public string CategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(FixedDepositCategory), Category) ? EnumHelper.GetDescription((FixedDepositCategory)Category) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Maturity Action")]
        public int MaturityAction { get; set; }

        [DataMember]
        [Display(Name = "Maturity Action")]
        public string MaturityActionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(FixedDepositMaturityAction), Category) ? EnumHelper.GetDescription((FixedDepositMaturityAction)Category) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Value")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Value must be greater than zero!")]
        public decimal Value { get; set; }

        [DataMember]
        [Display(Name = "Term (Months)")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Term must be greater than zero!")]
        public int Term { get; set; }

        [DataMember]
        [Display(Name = "Annual Percentage Rate")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Rate must be greater than zero!")]
        public double Rate { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        [Required]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(FixedDepositStatus), Status) ? EnumHelper.GetDescription((FixedDepositStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Maturity Date")]
        public DateTime MaturityDate { get; set; }

        [DataMember]
        [Display(Name = "Expected Interest")]
        public decimal ExpectedInterest { get; set; }

        [DataMember]
        [Display(Name = "Total Expected")]
        public decimal TotalExpected { get; set; }

        //added parameter

        [DataMember]
        [Display(Name = "Available Balance")]
        public decimal AvailableBalance { get; set; }


        [DataMember]
        [Display(Name = "Is Payable?")]
        public bool IsPayable
        {
            get
            {
                var result = default(bool);

                result = this.Status == (int)FixedDepositStatus.Running && this.MaturityDate <= DefaultSettings.Instance.ServerDate;

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Paid By")]
        public string PaidBy { get; set; }

        [DataMember]
        [Display(Name = "Paid Date")]
        public DateTime? PaidDate { get; set; }

        [DataMember]
        [Display(Name = "Verified/Rejected By")]
        public string AuditedBy { get; set; }

        [DataMember]
        [Display(Name = "Verification/Rejection Remarks")]
        public string AuditRemarks { get; set; }

        [DataMember]
        [Display(Name = "Verified/Rejected Date")]
        public DateTime? AuditedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }


        public List<FixedDepositPayableDTO> FixedDepositPayables { get; set; }
        public FixedDepositPayableDTO FixedDepositPayableDTO { get; set; }

        //new property
        public int FixedDepositAuthOption { get; set; }
        public int ModuleNavigationItemCode { get; set; }
        public bool HasErrors { get; set; }
        public List<string> ErrorMessages { get; set; }

        public List<FixedDepositDTO> FixedDeposits { get; set; } = new List<FixedDepositDTO>();
        public List<Guid> SelectedFixedDeposits { get; set; }


        public string errormassage;
        public Guid PayableId { get; set; }



    }
}
