using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg
{
    public static class CustomerFactory
    {
        public static Customer CreateCustomer(int type, string personalIdentificationNumber, Individual individual, NonIndividual nonIndividual, Address address, Guid? stationId, string reference1, string reference2, string reference3, string remarks, DateTime? registrationDate, string recruitedBy, Guid? administrativeDivisionId)
        {
            var customer = new Customer();

            customer.GenerateNewIdentity();

            customer.Type = (byte)type;

            customer.PersonalIdentificationNumber = personalIdentificationNumber;

            customer.Individual = individual;

            customer.NonIndividual = nonIndividual;

            customer.Address = address;

            customer.StationId = (stationId != null && stationId != Guid.Empty) ? stationId : null;

            customer.Reference1 = !string.IsNullOrWhiteSpace(reference1) ? reference1 : null;

            customer.Reference2 = !string.IsNullOrWhiteSpace(reference2) ? reference2 : null;

            customer.Reference3 = !string.IsNullOrWhiteSpace(reference3) ? reference3 : null;

            customer.Remarks = remarks;

            customer.RegistrationDate = registrationDate;

            customer.RecruitedBy = recruitedBy;

            customer.CreatedDate = DateTime.Now;

            customer.AdministrativeDivisionId = (administrativeDivisionId != null && administrativeDivisionId != Guid.Empty) ? administrativeDivisionId : null;

            return customer;
        }
    }
}
