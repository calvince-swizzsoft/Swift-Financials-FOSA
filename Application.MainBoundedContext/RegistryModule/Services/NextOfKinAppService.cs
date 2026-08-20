using Application.MainBoundedContext.DTO.RegistryModule;
using Domain.MainBoundedContext.RegistryModule.Aggregates.NextOfKinAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    // Domain layer (NextOfKin/NextOfKinFactory/NextOfKinSpecifications,
    // NextOfKinEntityConfiguration) and the AutoMapper NextOfKin ->
    // NextOfKinDTO map were already fully built but had no proper app
    // service — the only thing wired to the domain before this was
    // WebApplication1/Services/NextOfKinService.cs, a raw-ADO.NET class
    // living in the presentation project, bypassing the repository/DbContext
    // scope/AutoMapper layers this app's other modules use. Rebuilt here
    // against IRepository<NextOfKin>/IDbContextScopeFactory, same shape as
    // CustomerDocumentAppService, so NextOfKinController depends on a real
    // ICustomerDocumentAppService-style app service instead. The 100%
    // nomination-cap validation (a customer's next-of-kin percentages can't
    // sum past 100%) is ported over from that raw-SQL class since it's
    // genuine business logic, not a persistence detail.
    public class NextOfKinAppService : INextOfKinAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<NextOfKin> _nextOfKinRepository;

        public NextOfKinAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<NextOfKin> nextOfKinRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (nextOfKinRepository == null)
                throw new ArgumentNullException(nameof(nextOfKinRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _nextOfKinRepository = nextOfKinRepository;
        }

        public List<NextOfKinDTO> FindNextOfKins(Guid customerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<NextOfKin> spec = NextOfKinSpecifications.NextOfKinWithCustomerId(customerId);

                return _nextOfKinRepository.AllMatching<NextOfKinDTO>(spec, serviceHeader);
            }
        }

        public NextOfKinDTO FindNextOfKin(Guid nextOfKinId, ServiceHeader serviceHeader)
        {
            if (nextOfKinId == Guid.Empty)
                return null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _nextOfKinRepository.Get<NextOfKinDTO>(nextOfKinId, serviceHeader);
            }
        }

        public NextOfKinDTO AddNewNextOfKin(NextOfKinDTO nextOfKinDTO, ServiceHeader serviceHeader)
        {
            if (nextOfKinDTO == null)
                throw new ArgumentNullException(nameof(nextOfKinDTO));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var currentTotal = TotalNominatedPercentage(nextOfKinDTO.CustomerId, null, serviceHeader);
                AssertWithinAllocation(currentTotal, nextOfKinDTO.NominatedPercentage);

                var address = new Address(
                    nextOfKinDTO.AddressAddressLine1, nextOfKinDTO.AddressAddressLine2, nextOfKinDTO.AddressStreet,
                    nextOfKinDTO.AddressPostalCode, nextOfKinDTO.AddressCity, nextOfKinDTO.AddressEmail,
                    nextOfKinDTO.AddressLandLine, nextOfKinDTO.AddressMobileLine);

                var nextOfKin = NextOfKinFactory.CreateNextOfKin(
                    nextOfKinDTO.CustomerId, nextOfKinDTO.Salutation, nextOfKinDTO.FirstName, nextOfKinDTO.LastName,
                    nextOfKinDTO.IdentityCardType, nextOfKinDTO.IdentityCardNumber, nextOfKinDTO.Gender,
                    nextOfKinDTO.Relationship, address, nextOfKinDTO.NominatedPercentage, nextOfKinDTO.Remarks);

                nextOfKin.CreatedBy = serviceHeader.ApplicationUserName;

                _nextOfKinRepository.Add(nextOfKin, serviceHeader);

                dbContextScope.SaveChanges(serviceHeader);

                return nextOfKin.ProjectedAs<NextOfKinDTO>();
            }
        }

        public bool UpdateNextOfKin(NextOfKinDTO nextOfKinDTO, ServiceHeader serviceHeader)
        {
            if (nextOfKinDTO == null)
                throw new ArgumentNullException(nameof(nextOfKinDTO));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _nextOfKinRepository.Get(nextOfKinDTO.Id, serviceHeader);

                if (persisted == null)
                    return false;

                var currentTotal = TotalNominatedPercentage(nextOfKinDTO.CustomerId, nextOfKinDTO.Id, serviceHeader);
                AssertWithinAllocation(currentTotal, nextOfKinDTO.NominatedPercentage);

                var address = new Address(
                    nextOfKinDTO.AddressAddressLine1, nextOfKinDTO.AddressAddressLine2, nextOfKinDTO.AddressStreet,
                    nextOfKinDTO.AddressPostalCode, nextOfKinDTO.AddressCity, nextOfKinDTO.AddressEmail,
                    nextOfKinDTO.AddressLandLine, nextOfKinDTO.AddressMobileLine);

                var current = NextOfKinFactory.CreateNextOfKin(
                    nextOfKinDTO.CustomerId, nextOfKinDTO.Salutation, nextOfKinDTO.FirstName, nextOfKinDTO.LastName,
                    nextOfKinDTO.IdentityCardType, nextOfKinDTO.IdentityCardNumber, nextOfKinDTO.Gender,
                    nextOfKinDTO.Relationship, address, nextOfKinDTO.NominatedPercentage, nextOfKinDTO.Remarks);

                current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                current.CreatedBy = persisted.CreatedBy;

                _nextOfKinRepository.Merge(persisted, current, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool RemoveNextOfKin(Guid nextOfKinId, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _nextOfKinRepository.Get(nextOfKinId, serviceHeader);

                if (persisted == null)
                    return false;

                _nextOfKinRepository.Remove(persisted, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        // Sums NominatedPercentage for every OTHER next-of-kin already on
        // file for this customer — "other" excludes excludeId so an update
        // can validate against the record's own new value without double
        // counting its previous one.
        private double TotalNominatedPercentage(Guid customerId, Guid? excludeId, ServiceHeader serviceHeader)
        {
            ISpecification<NextOfKin> spec = NextOfKinSpecifications.NextOfKinWithCustomerId(customerId);

            var existing = _nextOfKinRepository.AllMatching(spec, serviceHeader);

            return existing
                .Where(x => !excludeId.HasValue || x.Id != excludeId.Value)
                .Sum(x => x.NominatedPercentage);
        }

        private static void AssertWithinAllocation(double currentTotal, double additionalPercentage)
        {
            var newTotal = currentTotal + additionalPercentage;

            if (newTotal > 100)
            {
                var remaining = 100 - currentTotal;
                throw new InvalidOperationException(
                    $"Cannot save next of kin. Current total: {currentTotal:0.##}%, new total would be: {newTotal:0.##}%, remaining capacity: {remaining:0.##}%.");
            }
        }
    }
}
