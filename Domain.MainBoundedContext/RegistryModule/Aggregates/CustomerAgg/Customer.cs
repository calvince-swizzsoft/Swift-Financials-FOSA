using Domain.MainBoundedContext.RegistryModule.Aggregates.AdministrativeDivisionAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.StationAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg
{
    public class Customer : Entity
    {
        public Guid? StationId { get; set; }

        public virtual Station Station { get; private set; }

        public byte Type { get; set; }

        public int SerialNumber { get; set; }

        public string PersonalIdentificationNumber { get; set; }

        public virtual Individual Individual { get; set; }

        public virtual NonIndividual NonIndividual { get; set; }

        public virtual Address Address { get; set; }

        public Guid? PassportImageId { get; set; }

        public Guid? SignatureImageId { get; set; }

        public Guid? IdentityCardFrontSideImageId { get; set; }

        public Guid? IdentityCardBackSideImageId { get; set; }

        public Guid? BiometricFingerprintImageId { get; set; }

        public Guid? BiometricFingerprintTemplateId { get; set; }

        public byte BiometricFingerprintTemplateFormat { get; set; }

        public Guid? BiometricFingerVeinTemplateId { get; set; }

        public byte BiometricFingerVeinTemplateFormat { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public virtual Image Passport { get; set; }

        public virtual Image Signature { get; set; }

        public virtual Image IdentityCardFrontSide { get; set; }

        public virtual Image IdentityCardBackSide { get; set; }

        public string Reference1 { get; set; }

        public string Reference2 { get; set; }

        public string Reference3 { get; set; }

        public string Remarks { get; set; }

        public bool IsDefaulter { get; set; }

        public bool IsLocked { get; private set; }

        public bool InhibitGuaranteeing { get; private set; }

        public string RecruitedBy { get; set; }

        public byte RecordStatus { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid? AdministrativeDivisionId { get; set; }

        public virtual AdministrativeDivision AdministrativeDivision { get; private set; }

        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }

        public void LockGuaranteeing()
        {
            if (!InhibitGuaranteeing)
                this.InhibitGuaranteeing = true;
        }

        public void UnlockGuaranteeing()
        {
            if (InhibitGuaranteeing)
                this.InhibitGuaranteeing = false;
        }



        
    }
}
