using Domain.MainBoundedContext.AccountsModule.Aggregates.ChannelVerificationChallengeAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.AccountsModule
{
    class ChannelVerificationChallengeEntityConfiguration : EntityTypeConfiguration<ChannelVerificationChallenge>
    {
        public ChannelVerificationChallengeEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.CodeHash).HasMaxLength(128).IsRequired();

            ToTable(string.Format("{0}ChannelVerificationChallenges", DefaultSettings.Instance.TablePrefix));
        }
    }
}
