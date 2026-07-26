using Domain.MainBoundedContext.RegistryModule.Aggregates.FuneralRiderClaimAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class FuneralRiderClaimEntityConfiguration : EntityTypeConfiguration<FuneralRiderClaim>
    {
        public FuneralRiderClaimEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.FileName).HasMaxLength(256);

            Property(x => x.FileTitle).HasMaxLength(256);

            Property(x => x.FileMIMEType).HasMaxLength(256);

            Property(x => x.FileDescription).HasMaxLength(512);

            Ignore(x => x.File);

            Property(x => x.Remarks).HasMaxLength(256);

            Property(x => x.ClaimDate).HasColumnType("date");

            Property(x => x.FuneralRiderClaimant.IdentityCardNumber).HasMaxLength(256);
            Property(x => x.FuneralRiderClaimant.MobileNumber).HasMaxLength(256);
            Property(x => x.FuneralRiderClaimant.Name).HasMaxLength(256);
            Property(x => x.FuneralRiderClaimant.Relationship).HasMaxLength(256);
            Property(x => x.FuneralRiderClaimant.TscNumber).HasMaxLength(256);
            Property(x => x.FuneralRiderClaimant.SignatureDate).HasColumnType("date");

            Property(x => x.ImmediateSuperior.IdentityCardNumber).HasMaxLength(256);
            Property(x => x.ImmediateSuperior.Name).HasMaxLength(256);
            Property(x => x.ImmediateSuperior.SignatureDate).HasColumnType("date");

            Property(x => x.AreaChief.IdentityCardNumber).HasMaxLength(256);
            Property(x => x.AreaChief.Name).HasMaxLength(256);
            Property(x => x.AreaChief.SignatureDate).HasColumnType("date");

            Property(x => x.AreaDelegate.IdentityCardNumber).HasMaxLength(256);
            Property(x => x.AreaDelegate.Name).HasMaxLength(256);
            Property(x => x.AreaDelegate.TscNumber).HasMaxLength(256);
            Property(x => x.AreaDelegate.SignatureDate).HasColumnType("date");

            Property(x => x.AreaBoardMember.IdentityCardNumber).HasMaxLength(256);
            Property(x => x.AreaBoardMember.Name).HasMaxLength(256);
            Property(x => x.AreaBoardMember.TscNumber).HasMaxLength(256);
            Property(x => x.AreaBoardMember.SignatureDate).HasColumnType("date");

            ToTable(string.Format("{0}FuneralRiderClaims", DefaultSettings.Instance.TablePrefix));
        }
    }
}
