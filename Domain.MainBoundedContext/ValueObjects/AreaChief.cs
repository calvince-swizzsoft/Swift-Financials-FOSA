using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class AreaChief : ValueObject<AreaChief>
    {
        public string IdentityCardNumber { get; private set; }

        public string Name { get; private set; }

        public DateTime? SignatureDate { get; private set; }

        private AreaChief() { }

        public AreaChief(string identityCardNumber, string name, DateTime? signatureDate)
        {
            this.IdentityCardNumber = identityCardNumber;
            this.Name = name;
            this.SignatureDate = signatureDate;
        }
    }
}
