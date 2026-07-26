
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class TellerDTO : BindingModelBase<TellerDTO>
    {
        public TellerDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(TellerType), Type) ? EnumHelper.GetDescription((TellerType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Code")]
        public int Code { get; set; }

        [DataMember]
        [Display(Name = "Code")]
        public string PaddedCode
        {
            get
            {
                return string.Format("{0}", Code).PadLeft(3, '0');
            }
        }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Lower Limit")]
        public decimal RangeLowerLimit { get; set; }

        [DataMember]
        [Display(Name = "Upper Limit")]
        public decimal RangeUpperLimit { get; set; }

        [DataMember]
        [Display(Name = "Mini Statement Items Cap")]
        public int MiniStatementItemsCap { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Employee")]
        //[ValidGuid]
        public Guid? EmployeeId { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid EmployeeCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int EmployeeCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string EmployeeCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), EmployeeCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)EmployeeCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string EmployeeCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string EmployeeCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Employee Name")]
        public string EmployeeCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", EmployeeCustomerIndividualSalutationDescription, EmployeeCustomerIndividualFirstName, EmployeeCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Employee Branch")]
        public Guid EmployeeBranchId { get; set; }

        [DataMember]
        [Display(Name = "Employee Branch")]
        public string EmployeeBranchDescription { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
        public Guid? ChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Type")]
        public int ChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Code")]
        public int ChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Name")]
        public string ChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Name")]
        public string ChartOfAccountName
        {
            get
            {
                return (ChartOfAccountId != null && ChartOfAccountId != Guid.Empty) ?
                    string.Format("{0}-{1} {2}", ChartOfAccountAccountType.FirstDigit(), ChartOfAccountAccountCode, ChartOfAccountAccountName) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "G/L Account Cost Center")]
        public Guid? ChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Cost Center")]
        public string ChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Shortage G/L Account")]
        [ValidGuid]
        public Guid? ShortageChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Shortage G/L Account Type")]
        public int ShortageChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Shortage G/L Account Code")]
        public int ShortageChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Shortage G/L Account Name")]
        public string ShortageChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Shortage G/L Account Name")]
        public string ShortageChartOfAccountName
        {
            get
            {
                return (ShortageChartOfAccountId != null && ShortageChartOfAccountId != Guid.Empty) ?
                    string.Format("{0}-{1} {2}", ShortageChartOfAccountAccountType.FirstDigit(), ShortageChartOfAccountAccountCode, ShortageChartOfAccountAccountName) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Shortage G/L Account Cost Center")]
        public Guid? ShortageChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Shortage G/L Account Cost Center")]
        public string ShortageChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Excess G/L Account")]
        //[ValidGuid]
        public Guid? ExcessChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Excess G/L Account Type")]
        public int ExcessChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Excess G/L Account Code")]
        public int ExcessChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Excess G/L Account Name")]
        public string ExcessChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Excess G/L Account Name")]
        public string ExcessChartOfAccountName
        {
            get
            {
                return (ExcessChartOfAccountId != null && ExcessChartOfAccountId != Guid.Empty) ?
                    string.Format("{0}-{1} {2}", ExcessChartOfAccountAccountType.FirstDigit(), ExcessChartOfAccountAccountCode, ExcessChartOfAccountAccountName) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Excess G/L Account Cost Center")]
        public Guid? ExcessChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Excess G/L Account Cost Center")]
        public string ExcessChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Float Customer Account")]

        public Guid? FloatCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Float Customer")]
        public Guid FloatCustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Float Customer Type")]
        public int FloatCustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Float Customer Type")]
        public string FloatCustomerAccountCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), FloatCustomerAccountCustomerType) ? EnumHelper.GetDescription((CustomerType)FloatCustomerAccountCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Float Branch")]
        public Guid FloatCustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Float Branch Code")]
        public int FloatCustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Float Customer Account Product")]
        public Guid FloatCustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Float Product Code")]
        public int FloatCustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Float Customer Account Product Code")]
        public int FloatCustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Float Customer Account Status")]
        public int FloatCustomerAccountStatus { get; set; }

        [DataMember]
        [Display(Name = "Float Customer Account Status")]
        public string FloatCustomerAccountStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerAccountStatus), FloatCustomerAccountStatus) ? EnumHelper.GetDescription((CustomerAccountStatus)FloatCustomerAccountStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Float Customer Account Record Status")]
        public int FloatCustomerAccountRecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Float Customer Account Record Status")]
        public string FloatCustomerAccountRecordStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RecordStatus), FloatCustomerAccountRecordStatus) ? EnumHelper.GetDescription((RecordStatus)FloatCustomerAccountRecordStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Float Customer Account Remarks")]
        public string FloatCustomerAccountRemarks { get; set; }

        [DataMember]
        [Display(Name = "Float Serial Number")]
        public int FloatCustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Float Full Account Number")]
        public string FloatFullAccountNumber
        {
            get
            {
                return (FloatCustomerAccountId != null && FloatCustomerAccountId != Guid.Empty) ? string.Format("{0}-{1}-{2}-{3}",
                            FloatCustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                            FloatCustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                            FloatCustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            FloatCustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0')) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Float Salutation")]
        public int FloatCustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Float Salutation")]
        public string FloatCustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), FloatCustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)FloatCustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Float First Name")]
        public string FloatCustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Float Other Names")]
        public string FloatCustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Float Identity Card Number")]
        public string FloatCustomerAccountCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Float Group Name")]
        public string FloatCustomerAccountCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Float Registration Number")]
        public string FloatCustomerAccountCustomerNonIndividualRegistrationNumber { get; set; }

        [DataMember]
        [Display(Name = "Float Personal Identification Number")]
        public string FloatCustomerAccountCustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Float Date Established")]
        public DateTime? FloatCustomerAccountCustomerNonIndividualDateEstablished { get; set; }

        [DataMember]
        [Display(Name = "Float Customer Name")]
        public string FloatCustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)FloatCustomerAccountCustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", FloatCustomerAccountCustomerIndividualSalutationDescription, FloatCustomerAccountCustomerIndividualFirstName, FloatCustomerAccountCustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = FloatCustomerAccountCustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Float Mobile Line")]
        public string FloatCustomerAccountCustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Float E-mail")]
        public string FloatCustomerAccountCustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Float Account Number")]
        public string FloatCustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Float Float Membership Number")]
        public string FloatCustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Float Float Personal File Number")]
        public string FloatCustomerAccountCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Commission Customer Account")]

        public Guid? CommissionCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Commission Customer")]
        public Guid CommissionCustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Commission Customer Type")]
        public int CommissionCustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Commission Customer Type")]
        public string CommissionCustomerAccountCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), CommissionCustomerAccountCustomerType) ? EnumHelper.GetDescription((CustomerType)CommissionCustomerAccountCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Commission Branch")]
        public Guid CommissionCustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Commission Branch Code")]
        public int CommissionCustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Commission Customer Account Product")]
        public Guid CommissionCustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Commission Product Code")]
        public int CommissionCustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Commission Customer Account Product Code")]
        public int CommissionCustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Commission Customer Account Status")]
        public int CommissionCustomerAccountStatus { get; set; }

        [DataMember]
        [Display(Name = "Commission Customer Account Status")]
        public string CommissionCustomerAccountStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerAccountStatus), CommissionCustomerAccountStatus) ? EnumHelper.GetDescription((CustomerAccountStatus)CommissionCustomerAccountStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Commission Customer Account Record Status")]
        public int CommissionCustomerAccountRecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Commission Customer Account Record Status")]
        public string CommissionCustomerAccountRecordStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RecordStatus), CommissionCustomerAccountRecordStatus) ? EnumHelper.GetDescription((RecordStatus)CommissionCustomerAccountRecordStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Commission Customer Account Remarks")]
        public string CommissionCustomerAccountRemarks { get; set; }

        [DataMember]
        [Display(Name = "Commission Serial Number")]
        public int CommissionCustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Commission Full Account Number")]
        public string CommissionFullAccountNumber
        {
            get
            {
                return (CommissionCustomerAccountId != null && CommissionCustomerAccountId != Guid.Empty) ? string.Format("{0}-{1}-{2}-{3}",
                            CommissionCustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                            CommissionCustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                            CommissionCustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            CommissionCustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0')) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Commission Salutation")]
        public int CommissionCustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Commission Salutation")]
        public string CommissionCustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CommissionCustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CommissionCustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Commission First Name")]
        public string CommissionCustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Commission Other Names")]
        public string CommissionCustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Commission Identity Card Number")]
        public string CommissionCustomerAccountCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Commission Group Name")]
        public string CommissionCustomerAccountCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Commission Registration Number")]
        public string CommissionCustomerAccountCustomerNonIndividualRegistrationNumber { get; set; }

        [DataMember]
        [Display(Name = "Commission Personal Identification Number")]
        public string CommissionCustomerAccountCustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Commission Date Established")]
        public DateTime? CommissionCustomerAccountCustomerNonIndividualDateEstablished { get; set; }

        [DataMember]
        [Display(Name = "Commission Customer Name")]
        public string CommissionCustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CommissionCustomerAccountCustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", CommissionCustomerAccountCustomerIndividualSalutationDescription, CommissionCustomerAccountCustomerIndividualFirstName, CommissionCustomerAccountCustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CommissionCustomerAccountCustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Commission Mobile Line")]
        public string CommissionCustomerAccountCustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Commission E-mail")]
        public string CommissionCustomerAccountCustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Commission Account Number")]
        public string CommissionCustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Commission Commission Membership Number")]
        public string CommissionCustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Commission Commission Personal File Number")]
        public string CommissionCustomerAccountCustomerReference3 { get; set; }

        public int CommissionCustomerAccountCustomerFilter { get; set; }

        [DataMember]
        [Display(Name = "Commission Customer Account Product Code")]
        public string CustomerFilterDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerFilter), CommissionCustomerAccountCustomerAccountTypeProductCode) ? EnumHelper.GetDescription((CustomerFilter)CommissionCustomerAccountCustomerFilter) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Application User Name")]
        public string ApplicationUserName { get; set; }

        [DataMember]
        [Display(Name = "Book Balance")]
        public decimal BookBalance { get; set; }

        [DataMember]
        [Display(Name = "Total Credits")]
        public decimal TotalCredits { get; set; }

        [DataMember]
        [Display(Name = "Total Debits")]
        public decimal TotalDebits { get; set; }

        [DataMember]
        [Display(Name = "Opening Balance")]
        public decimal OpeningBalance { get; set; }

        [DataMember]
        [Display(Name = "Closing Balance")]
        public decimal ClosingBalance { get; set; }


        [DataMember]
        [Display(Name = "Items Count")]
        public int ItemsCount { get; set; }

        [DataMember]
        [Display(Name = "Total Cheques")]
        public decimal TellerTotalCheques { get; set; }

       
    }
}
