
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class PaymentVoucherDTO : BindingModelBase<PaymentVoucherDTO>
    {
        public PaymentVoucherDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Cheque Book")]
        [ValidGuid]
        public Guid ChequeBookId { get; set; }

        [DataMember]
        [Display(Name = "Cheque Book Type")]
        public int ChequeBookType { get; set; }

        [DataMember]
        [Display(Name = "Cheque Book Type")]
        public string ChequeBookTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChequeBookType), ChequeBookType) ? EnumHelper.GetDescription((ChequeBookType)ChequeBookType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Cheque Book Serial Number")]
        public int ChequeBookSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Cheque Book Serial Number")]
        public string PaddedChequeBookSerialNumber
        {
            get
            {
                return string.Format("{0}", ChequeBookSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Customer Account")]
        public Guid ChequeBookCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Cheque Book Is Active?")]
        public bool ChequeBookIsActive { get; set; }

        [DataMember]
        [Display(Name = "Cheque Book Is Locked?")]
        public bool ChequeBookIsLocked { get; set; }

        [DataMember]
        [Display(Name = "Voucher Number")]
        public Guid VoucherNumber { get; set; }

        [DataMember]
        [Display(Name = "Voucher Number")]
        public string PaddedVoucherNumber
        {
            get
            {
                return string.Format("{0}", VoucherNumber).PadLeft(6, '0');
            }
        }

        [DataMember]
        [Display(Name = "Payee")]
        [Required]
        public string Payee { get; set; }

        [DataMember]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Cheque amount must be greater than zero!")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Write Date")]
        [CustomValidation(typeof(PaymentVoucherDTO), "ValidateWriteDate", ErrorMessage = "Stale and/or post-dated payment vouchers not accepted!")]
        public DateTime? WriteDate { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        [Required]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(PaymentVoucherStatus), Status) ? EnumHelper.GetDescription((PaymentVoucherStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Paid/Flagged By")]
        public string PaidBy { get; set; }

        [DataMember]
        [Display(Name = "Paid/Flagged Date")]
        public DateTime? PaidDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Management Action")]
        public int ManagementAction { get; set; }

        [DataMember]
        [Display(Name = "InvoiceNo")]
        public int InvoiceNo { get; set; }

        [DataMember]
        [Display(Name = "VendorNo")]
        public int VendorNo { get; set; }

        [DataMember]
        [ValidGuid]
        [Display(Name = "InvoiceId")]
        public Guid InvoiceId { get; set; }


        [DataMember]
        [ValidGuid]
        [Display(Name = "VendorId")]
        public Guid VendorId { get; set; }


        [DataMember]
        [Display(Name = "BankLinkageChartOfAccountId")]
        public Guid BankLinkageChartOfAccountId { get; set; }


        [DataMember]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        public static ValidationResult ValidateWriteDate(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as PaymentVoucherDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be PaymentVoucherDTO");

            var writeDate = bindingModel.WriteDate ?? DateTime.Today;

            var period = Math.Abs(UberUtil.GetPeriod(DefaultSettings.Instance.ServerDate, writeDate));

            if (bindingModel.WriteDate > DefaultSettings.Instance.ServerDate || period > 6)
                return new ValidationResult("Stale and/or post-dated payment vouchers not accepted!");

            return ValidationResult.Success;
        }
    }
}
