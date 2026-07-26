using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.TruncatedChequeAgg;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.FrontOfficeModule
{
    class TruncatedChequeEntityConfiguration : EntityTypeConfiguration<TruncatedCheque>
    {
        public TruncatedChequeEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.VoucherTypeCode).HasMaxLength(256);

            Property(x => x.PresentingBank).HasMaxLength(256);
            Property(x => x.PresentingBranch).HasMaxLength(256);

            Property(x => x.DestinationAccountBank).HasMaxLength(256);
            Property(x => x.DestinationAccountBranch).HasMaxLength(256);
            Property(x => x.DestinationAccountAccount).HasMaxLength(256);
            Property(x => x.DestinationAccountCheckDigit).HasMaxLength(256);
            Property(x => x.DestinationAccountCurrencyCode).HasMaxLength(256);

            Property(x => x.DocumentReferenceNumber).HasMaxLength(256);
            Property(x => x.SerialNumber).HasMaxLength(256);
            Property(x => x.Filler).HasMaxLength(256);
            Property(x => x.CollectionAccountDetail).HasMaxLength(256);

            Property(x => x.FrontImage1Signature).HasMaxLength(256);
            Property(x => x.FrontImage2Signature).HasMaxLength(256);
            Property(x => x.RearImageSignature).HasMaxLength(256);

            Property(x => x.UnPaidReason).HasMaxLength(256);
            Property(x => x.AmountEntryMethod).HasMaxLength(256);

            Property(x => x.Remarks).HasMaxLength(256);
            Property(x => x.ProcessedBy).HasMaxLength(256);
            

            Ignore(x => x.FrontImage1);
            Ignore(x => x.FrontImage2);
            Ignore(x => x.RearImage);

            ToTable(string.Format("{0}TruncatedCheques", DefaultSettings.Instance.TablePrefix));
        }
    }
}
