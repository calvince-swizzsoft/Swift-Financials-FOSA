using Domain.MainBoundedContext.RegistryModule.Aggregates.PopulationRegisterQueryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.RegistryModule
{
    class PopulationRegisterQueryEntityConfiguration : EntityTypeConfiguration<PopulationRegisterQuery>
    {
        public PopulationRegisterQueryEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.IdentityNumber).HasMaxLength(256);
            Property(x => x.IdentitySerialNumber).HasMaxLength(256);
            Property(x => x.IDNumber).HasMaxLength(256);
            Property(x => x.SerialNumber).HasMaxLength(256);
            Property(x => x.Gender).HasMaxLength(10);
            Property(x => x.FirstName).HasMaxLength(256);
            Property(x => x.OtherName).HasMaxLength(256);
            Property(x => x.Surname).HasMaxLength(256);
            Property(x => x.Pin).HasMaxLength(16);
            Property(x => x.Citizenship).HasMaxLength(256);
            Property(x => x.Family).HasMaxLength(256);
            Property(x => x.Clan).HasMaxLength(256);
            Property(x => x.EthnicGroup).HasMaxLength(256);
            Property(x => x.Occupation).HasMaxLength(256);
            Property(x => x.PlaceOfBirth).HasMaxLength(1000);
            Property(x => x.PlaceOfDeath).HasMaxLength(1000);
            Property(x => x.PlaceOfLive).HasMaxLength(1000);
            Property(x => x.RegOffice).HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(256);
            Property(x => x.AuthorizedBy).HasMaxLength(256);
            Property(x => x.AuthorizationRemarks).HasMaxLength(256);
            Property(x => x.CreatedBy).HasMaxLength(256);
            Property(x => x.PlaceOfLive).HasMaxLength(1000);

            Property(x => x.DateOfBirth).HasColumnType("date");
            Property(x => x.DateOfDeath).HasColumnType("date");
            Property(x => x.DateOfIssue).HasColumnType("date");
            Property(x => x.DateOfExpiry).HasColumnType("date");

            Ignore(x => x.Photo);
            Ignore(x => x.PhotoFromPassport);
            Ignore(x => x.Signature);
            Ignore(x => x.Fingerprint);

            Property(t => t.Status).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_PopulationRegisterQuery_Status")));

            ToTable(string.Format("{0}PopulationRegisterQueries", DefaultSettings.Instance.TablePrefix));
        }
    }
}
