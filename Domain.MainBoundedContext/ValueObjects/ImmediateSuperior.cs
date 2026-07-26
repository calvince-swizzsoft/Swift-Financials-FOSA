using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class ImmediateSuperior : ValueObject<ImmediateSuperior>
    {
        public string IdentityCardNumber { get; private set; }

        public string Name { get; private set; }

        public DateTime? SignatureDate { get; private set; }

        private ImmediateSuperior() { }

        public ImmediateSuperior(string identityCardNumber, string name, DateTime? signatureDate)
        {
            this.IdentityCardNumber = identityCardNumber;
            this.Name = name;
            this.SignatureDate = signatureDate;
        }
    }
}
