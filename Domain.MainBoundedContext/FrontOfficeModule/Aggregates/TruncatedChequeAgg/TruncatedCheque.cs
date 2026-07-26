using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentVoucherAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ElectronicJournalAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.TruncatedChequeAgg
{
    public class TruncatedCheque : Domain.Seedwork.Entity
    {
        public Guid ElectronicJournalId { get; set; }

        public virtual ElectronicJournal ElectronicJournal { get; private set; }

        public Guid? PaymentVoucherId { get; set; }

        public virtual PaymentVoucher PaymentVoucher { get; private set; }

        public byte ReasonForReturnCode { get; set; }

        public string VoucherTypeCode { get; set; }

        public decimal Value { get; set; }

        public string AmountEntryMethod { get; set; }

        public string DestinationAccountBank { get; set; }

        public string DestinationAccountBranch { get; set; }

        public string DestinationAccountAccount { get; set; }

        public string DestinationAccountCheckDigit { get; set; }

        public string DestinationAccountCurrencyCode { get; set; }

        public string Filler { get; set; }

        public string CollectionAccountDetail { get; set; }

        public string PresentingBank { get; set; }

        public string PresentingBranch { get; set; }

        public string SerialNumber { get; set; }

        public string DocumentReferenceNumber { get; set; }

        public Guid? FrontImage1Id { get; set; }

        public int FrontImage1Size { get; set; }

        public string FrontImage1Signature { get; set; }

        public virtual Image FrontImage1 { get; set; }

        public Guid? FrontImage2Id { get; set; }

        public int FrontImage2Size { get; set; }

        public string FrontImage2Signature { get; set; }

        public virtual Image FrontImage2 { get; set; }

        public Guid? RearImageId { get; set; }

        public int RearImageSize { get; set; }

        public string RearImageSignature { get; set; }

        public virtual Image RearImage { get; set; }

        public byte Status { get; set; }

        public string Remarks { get; set; }

        public byte UnPaidCode { get; set; }

        public string UnPaidReason { get; set; }

        public string ProcessedBy { get; set; }

        public DateTime? ProcessedDate { get; set; }
    }
}
