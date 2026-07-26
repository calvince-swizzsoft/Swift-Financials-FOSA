using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.PopulationRegisterQueryAgg
{
    public class PopulationRegisterQuery : Entity
    {
        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public byte IdentityType { get; set; }

        public string IdentityNumber { get; set; }

        public string IdentitySerialNumber { get; set; }

        public string Remarks { get; set; }

        public string IDNumber { get; set; }

        public string SerialNumber { get; set; }

        public string Gender { get; set; }

        public string FirstName { get; set; }

        public string OtherName { get; set; }

        public string Surname { get; set; }

        public string Pin { get; set; }

        public string Citizenship { get; set; }

        public string Family { get; set; }

        public string Clan { get; set; }

        public string EthnicGroup { get; set; }

        public string Occupation { get; set; }

        public string PlaceOfBirth { get; set; }

        public string PlaceOfDeath { get; set; }

        public string PlaceOfLive { get; set; }

        public string RegOffice { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfBirthFromPassport { get; set; }

        public DateTime? DateOfDeath { get; set; }

        public DateTime? DateOfIssue { get; set; }

        public DateTime? DateOfExpiry { get; set; }

        public Guid? PhotoImageId { get; set; }

        public Guid? PhotoFromPassportImageId { get; set; }

        public Guid? SignatureImageId { get; set; }

        public Guid? FingerprintImageId { get; set; }

        public string ErrorResponse { get; set; }

        public byte Status { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }

        public virtual Image Photo { get; set; }

        public virtual Image PhotoFromPassport { get; set; }

        public virtual Image Signature { get; set; }

        public virtual Image Fingerprint { get; set; }
    }
}
