using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ElectronicJournalAgg;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Infrastructure.Data.MainBoundedContext.UnitOfWork.Mapping.FrontOfficeModule
{
    class ElectronicJournalEntityConfiguration : EntityTypeConfiguration<ElectronicJournal>
    {
        public ElectronicJournalEntityConfiguration()
        {
            HasKey(x => x.Id);

            Property(t => t.SequentialId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsClustered = true, IsUnique = true })); Property(x => x.CreatedBy).HasMaxLength(256);

            Property(x => x.HeaderRecord.Filler).HasMaxLength(256);
            Property(x => x.HeaderRecord.FileSerialNumber).HasMaxLength(256);
            Property(x => x.HeaderRecord.FileType).HasMaxLength(256);
            Property(x => x.HeaderRecord.LastFileIndicator).HasMaxLength(256);
            Property(x => x.HeaderRecord.PresentingOrganisation).HasMaxLength(256);
            Property(x => x.HeaderRecord.PresentingOrganisationBank).HasMaxLength(256);
            Property(x => x.HeaderRecord.PresentingOrganisationClearingCentre).HasMaxLength(256);
            Property(x => x.HeaderRecord.ReceivingOrganisation).HasMaxLength(256);
            Property(x => x.HeaderRecord.ReceivingOrganisationBank).HasMaxLength(256);
            Property(x => x.HeaderRecord.ReceivingOrganisationClearingCentre).HasMaxLength(256);
            Property(x => x.HeaderRecord.DateOfFileExchange).HasColumnType("date");

            Property(x => x.TrailerRecord.Bank).HasMaxLength(256);
            Property(x => x.TrailerRecord.ClearingCentre).HasMaxLength(256);
            Property(x => x.TrailerRecord.Organisation).HasMaxLength(256);
            Property(x => x.TrailerRecord.RecordType).HasMaxLength(256);
            Property(x => x.TrailerRecord.Filler).HasMaxLength(256);

            Property(x => x.FileName).HasMaxLength(256);
            Property(x => x.ClosedBy).HasMaxLength(256);
            

            ToTable(string.Format("{0}ElectronicJournals", DefaultSettings.Instance.TablePrefix));
        }
    }
}
