using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class NonIndividual : ValueObject<NonIndividual>
    {
        public string Description { get; private set; }

        public string RegistrationNumber { get; private set; }

        public string RegistrationSerialNumber { get; private set; }

        public DateTime? DateEstablished { get; private set; }

        public NonIndividual(int customerType, string description, string registrationNumber, string registrationSerialNumber, DateTime? dateEstablished)
        {
            switch ((CustomerType)customerType)
            {
                case CustomerType.Partnership:
                case CustomerType.Corporation:
                case CustomerType.MicroCredit:
                    this.Description = description;
                    this.RegistrationNumber = registrationNumber;
                    this.RegistrationSerialNumber = registrationSerialNumber;
                    this.DateEstablished = dateEstablished;
                    break;
                case CustomerType.Individual:
                default:
                    break;
            }
        }

        private NonIndividual()
        { }
    }
}
