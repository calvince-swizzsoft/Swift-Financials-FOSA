using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class AreaBoardMember: ValueObject<AreaBoardMember>
    {
        public string IdentityCardNumber { get; private set; }

        public string Name { get; private set; }

        public string TscNumber { get; private set; }

        public DateTime? SignatureDate { get; private set; }

        private AreaBoardMember() { }

        public AreaBoardMember(string identityCardNumber, string name, string tscNumber, DateTime? signatureDate)
        {
            this.IdentityCardNumber = identityCardNumber;
            this.Name = name;
            this.TscNumber = tscNumber;
            this.SignatureDate = signatureDate;
        }
    }
}
