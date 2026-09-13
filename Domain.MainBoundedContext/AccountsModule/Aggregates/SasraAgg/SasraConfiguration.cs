using System;
using Domain.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateAgg;
namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg
{
    public class SasraInstitutionProfile : Entity
    {
        public int SingletonKey { get; set; }
        public string Profile { get; set; }
        public string InstitutionName { get; set; }
        public string RegistrationNumber { get; set; }
        public int Revision { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    // Each record and its line/account tree is an immutable draft revision.
    public class SasraTemplateVersion : Entity
    {
        public string Profile { get; set; }
        public string ReportCode { get; set; }
        public string Version { get; set; }
        public int Revision { get; set; }
        public string SourceUrl { get; set; }
        public string WorkbookSha256 { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public Guid RootTemplateId { get; set; }
        public virtual ReportTemplate RootTemplate { get; set; }
    }
    public class SasraLineDefinition : Entity
    {
        public Guid VersionId { get; set; }
        public virtual SasraTemplateVersion Version { get; set; }
        public Guid ReportTemplateId { get; set; }
        public virtual ReportTemplate ReportTemplate { get; set; }
        public int Position { get; set; }
        public string Code { get; set; }
        public string Source { get; set; }
        public string Sheet { get; set; }
        public int Sign { get; set; }
    }
}
