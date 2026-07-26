
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class TruncatedChequeDTO : BindingModelBase<TruncatedChequeDTO>
    {
        public TruncatedChequeDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Electronic Journal")]
        [ValidGuid]
        public Guid ElectronicJournalId { get; set; }

        [DataMember]
        [Display(Name = "Payment Voucher")]
        public Guid? PaymentVoucherId { get; set; }

        [DataMember]
        [Display(Name = "Cheque Book")]
        public Guid PaymentVoucherChequeBookId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        public Guid PaymentVoucherChequeBookCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Reason for Return")]
        public int ReasonForReturnCode { get; set; }

        [DataMember]
        [Display(Name = "Reason for Return")]
        public string ReasonForReturnCodeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(TruncatedChequeReturnCode), ReasonForReturnCode) ? EnumHelper.GetDescription((TruncatedChequeReturnCode)ReasonForReturnCode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Voucher Type Code")]
        public string VoucherTypeCode { get; set; }

        [DataMember]
        [Display(Name = "Value")]
        public decimal Value { get; set; }

        [DataMember]
        [Display(Name = "Amount Entry Method")]
        public string AmountEntryMethod { get; set; }

        [DataMember]
        [Display(Name = "Destination Account Bank")]
        public string DestinationAccountBank { get; set; }

        [DataMember]
        [Display(Name = "Destination Account Branch")]
        public string DestinationAccountBranch { get; set; }

        [DataMember]
        [Display(Name = "Destination Account")]
        public string DestinationAccountAccount { get; set; }

        [DataMember]
        [Display(Name = "Destination Account Check Digit")]
        public string DestinationAccountCheckDigit { get; set; }

        [DataMember]
        [Display(Name = "Destination Account Currency Code")]
        public string DestinationAccountCurrencyCode { get; set; }

        [DataMember]
        [Display(Name = "Filler")]
        public string Filler { get; set; }

        [DataMember]
        [Display(Name = "Collection Account Detail")]
        public string CollectionAccountDetail { get; set; }

        [DataMember]
        [Display(Name = "Presenting Bank")]
        public string PresentingBank { get; set; }

        [DataMember]
        [Display(Name = "Presenting Branch")]
        public string PresentingBranch { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Document Reference Number")]
        public string DocumentReferenceNumber { get; set; }

        [DataMember]
        [Display(Name = "Front Image1")]
        public Guid? FrontImage1Id { get; set; }

        [DataMember]
        [Display(Name = "Front Image1 Size")]
        public int FrontImage1Size { get; set; }

        [DataMember]
        [Display(Name = "Front Image1 Signature")]
        public string FrontImage1Signature { get; set; }

        [DataMember]
        [Display(Name = "Front Image1")]
        public byte[] FrontImage1Buffer { get; set; }

        [DataMember]
        [Display(Name = "Front Image2")]
        public Guid? FrontImage2Id { get; set; }

        [DataMember]
        [Display(Name = "Front Image2 Size")]
        public int FrontImage2Size { get; set; }

        [DataMember]
        [Display(Name = "Front Image2 Signature")]
        public string FrontImage2Signature { get; set; }

        [DataMember]
        [Display(Name = "Front Image2")]
        public byte[] FrontImage2Buffer { get; set; }

        [DataMember]
        [Display(Name = "Rear Image")]
        public Guid? RearImageId { get; set; }

        [Display(Name = "Rear Image Size")]
        public int RearImageSize { get; set; }

        [DataMember]
        [Display(Name = "Rear Image Signature")]
        public string RearImageSignature { get; set; }

        [DataMember]
        [Display(Name = "Rear Image")]
        public byte[] RearImageBuffer { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(TruncatedChequeStatus), Status) ? EnumHelper.GetDescription((TruncatedChequeStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "UnPaid Code")]
        public int UnPaidCode { get; set; }

        [DataMember]
        [Display(Name = "UnPaid Reason")]
        public string UnPaidReason { get; set; }

        [DataMember]
        [Display(Name = "Processed By")]
        public string ProcessedBy { get; set; }

        [DataMember]
        [Display(Name = "Processed Date")]
        public DateTime? ProcessedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "UnPay Reason")]
        public Guid UnPayReasonId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Module Navigation Item Code")]
        public int ModuleNavigationItemCode { get; set; }

        [DataMember]
        [Display(Name = "Truncated Cheque Processing Option")]
        public int TruncatedChequeProcessingOption { get; set; }
    }
}
