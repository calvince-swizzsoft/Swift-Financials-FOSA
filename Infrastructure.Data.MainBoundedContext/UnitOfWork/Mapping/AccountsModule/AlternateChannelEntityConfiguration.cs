using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class AlternateChannelEntityConfiguration : EntityTypeConfiguration<AlternateChannel>
    {
        public AlternateChannelEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true }));

            Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.ValidFrom).HasColumnType("date");
            Property(x => x.Expires).HasColumnType("date");

            Property(x => x.CardNumber).HasMaxLength(256);
            Property(x => x.Remarks).HasMaxLength(256);
            Property(x => x.ModifiedBy).HasMaxLength(256);
            Property(x => x.RecruitedBy).HasMaxLength(256);
            

            Property(t => t.Type).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_AlternateChannel_Type")));
            Property(t => t.CardNumber).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_AlternateChannel_CardNumber")));
            Property(t => t.IsThirdPartyNotified).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_AlternateChannel_IsThirdPartyNotified")));
            Property(t => t.RecordStatus).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_AlternateChannel_RecordStatus")));

            ToTable(string.Format("{0}AlternateChannels", DefaultSettings.Instance.TablePrefix));
        }
    }
}
