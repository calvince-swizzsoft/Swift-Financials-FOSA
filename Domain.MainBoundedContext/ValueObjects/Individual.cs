using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class Individual : ValueObject<Individual>
    {
        public byte Type { get; set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public byte IdentityCardType { get; private set; }

        public string IdentityCardNumber { get; private set; }

        public string IdentityCardSerialNumber { get; private set; }

        public string PayrollNumbers { get; private set; }

        public byte Salutation { get; private set; }

        public byte Gender { get; private set; }

        public byte MaritalStatus { get; private set; }

        public byte Nationality { get; private set; }

        public DateTime? BirthDate { get; private set; }

        public string EmploymentDesignation { get; private set; }

        public byte? EmploymentTermsOfService { get; private set; }

        public DateTime? EmploymentDate { get; private set; }

        public byte Classification { get; private set; }

        public Individual(int customerType, int type, string firstName, string lastName, int identityCardType, string identityCardNumber, string identityCardSerialNumber, string payrollNumbers, int salutation, int gender, int maritalStatus, int nationality, DateTime? birthDate, string employmentDesignation, int? employmentTermsOfService, DateTime? employmentDate, int classification)
        {
            this.PayrollNumbers = payrollNumbers;

            switch ((CustomerType)customerType)
            {
                case CustomerType.Individual:
                    this.Type = (byte)type;
                    this.FirstName = firstName;
                    this.LastName = lastName;
                    this.IdentityCardType = (byte)identityCardType;
                    this.IdentityCardNumber = identityCardNumber;
                    this.IdentityCardSerialNumber = identityCardSerialNumber;
                    this.Salutation = (byte)salutation;
                    this.Gender = (byte)gender;
                    this.MaritalStatus = (byte)maritalStatus;
                    this.Nationality = (byte)nationality;
                    this.BirthDate = birthDate;
                    this.EmploymentDesignation = employmentDesignation;
                    this.EmploymentTermsOfService = employmentTermsOfService.HasValue ? (byte?)employmentTermsOfService : null;
                    this.EmploymentDate = employmentDate;
                    this.Classification = (byte)classification;
                    break;
                case CustomerType.Partnership:
                case CustomerType.Corporation:
                case CustomerType.MicroCredit:
                default:
                    break;
            }
        }

        private Individual()
        { }
    }
}
