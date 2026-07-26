using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.TruncatedChequeAgg
{
    public static class TruncatedChequeFactory
    {
        public static TruncatedCheque CreateTruncatedCheque(Guid electronicJournalId, Guid? paymentVoucherId, string voucherTypeCode, decimal value, string amountEntryMethod, string destinationAccountBank, string destinationAccountBranch, string destinationAccountAccount, string destinationAccountCheckDigit, string destinationAccountCurrencyCode, string filler, string collectionAccountDetail, string presentingBank, string presentingBranch, string serialNumber, string documentReferenceNumber, int frontImage1Size, string frontImage1Signature, int frontImage2Size, string frontImage2Signature, int rearImageSize, string rearImageSignature, string remarks)
        {
            var truncatedCheque = new TruncatedCheque();

            truncatedCheque.GenerateNewIdentity();

            truncatedCheque.ElectronicJournalId = electronicJournalId;

            truncatedCheque.PaymentVoucherId = (paymentVoucherId != null && paymentVoucherId != Guid.Empty) ? paymentVoucherId : null;

            truncatedCheque.VoucherTypeCode = voucherTypeCode;

            truncatedCheque.Value = value;

            truncatedCheque.AmountEntryMethod = amountEntryMethod;

            truncatedCheque.DestinationAccountBank = destinationAccountBank;

            truncatedCheque.DestinationAccountBranch = destinationAccountBranch;

            truncatedCheque.DestinationAccountAccount = destinationAccountAccount;

            truncatedCheque.DestinationAccountCheckDigit = destinationAccountCheckDigit;

            truncatedCheque.DestinationAccountCurrencyCode = destinationAccountCurrencyCode;

            truncatedCheque.Filler = filler;

            truncatedCheque.CollectionAccountDetail = collectionAccountDetail;

            truncatedCheque.PresentingBank = presentingBank;

            truncatedCheque.PresentingBranch = presentingBranch;

            truncatedCheque.SerialNumber = serialNumber;

            truncatedCheque.DocumentReferenceNumber = documentReferenceNumber;

            truncatedCheque.FrontImage1Size = frontImage1Size;

            truncatedCheque.FrontImage1Signature = frontImage1Signature;

            truncatedCheque.FrontImage2Size = frontImage2Size;

            truncatedCheque.FrontImage2Signature = frontImage2Signature;

            truncatedCheque.RearImageSize = rearImageSize;

            truncatedCheque.RearImageSignature = rearImageSignature;

            truncatedCheque.Remarks = remarks;

            truncatedCheque.CreatedDate = DateTime.Now;

            return truncatedCheque;
        }
    }
}
