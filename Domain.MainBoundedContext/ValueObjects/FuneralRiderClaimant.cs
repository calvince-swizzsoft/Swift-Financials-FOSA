using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class FuneralRiderClaimant : ValueObject<FuneralRiderClaimant>
    {
        public string IdentityCardNumber { get; private set; }

        public string TscNumber { get; private set; }

        public string Name { get; private set; }

        public string MobileNumber { get; private set; }

        public string Relationship { get; private set; }

        public DateTime? SignatureDate { get; private set; }

        private FuneralRiderClaimant() { }

        public FuneralRiderClaimant(string identityCardNumber, string tscNumber, string name, string mobileNumber, string relationship, DateTime? signatureDate)
        {
            this.IdentityCardNumber = identityCardNumber;
            this.TscNumber = tscNumber;
            this.Name = name;
            this.MobileNumber = mobileNumber;
            this.Relationship = relationship;
            this.SignatureDate = signatureDate;
        }
    }
}
