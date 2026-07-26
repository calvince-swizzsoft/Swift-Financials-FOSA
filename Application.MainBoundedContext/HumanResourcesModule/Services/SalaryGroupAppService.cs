using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardEntryAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupEntryAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class SalaryGroupAppService : ISalaryGroupAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<SalaryGroup> _salaryGroupRepository;
        private readonly IRepository<SalaryGroupEntry> _salaryGroupEntryRepository;
        private readonly IRepository<SalaryCard> _salaryCardRepository;
        private readonly IRepository<SalaryCardEntry> _salaryCardEntryRepository;

        public SalaryGroupAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<SalaryGroup> salaryGroupRepository,
           IRepository<SalaryGroupEntry> salaryGroupEntryRepository,
           IRepository<SalaryCard> salaryCardRepository,
           IRepository<SalaryCardEntry> salaryCardEntryRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (salaryGroupRepository == null)
                throw new ArgumentNullException(nameof(salaryGroupRepository));

            if (salaryGroupEntryRepository == null)
                throw new ArgumentNullException(nameof(salaryGroupEntryRepository));

            if (salaryCardRepository == null)
                throw new ArgumentNullException(nameof(salaryCardRepository));

            if (salaryCardEntryRepository == null)
                throw new ArgumentNullException(nameof(salaryCardEntryRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _salaryGroupRepository = salaryGroupRepository;
            _salaryGroupEntryRepository = salaryGroupEntryRepository;
            _salaryCardRepository = salaryCardRepository;
            _salaryCardEntryRepository = salaryCardEntryRepository;
        }

        public SalaryGroupDTO AddNewSalaryGroup(SalaryGroupDTO salaryGroupDTO, ServiceHeader serviceHeader)
        {
            if (salaryGroupDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var salaryGroup = SalaryGroupFactory.CreateSalaryGroup(salaryGroupDTO.Description);

                    _salaryGroupRepository.Add(salaryGroup, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return salaryGroup.ProjectedAs<SalaryGroupDTO>();
                }
            }
            else return null;
        }

        public bool UpdateSalaryGroup(SalaryGroupDTO salaryGroupDTO, ServiceHeader serviceHeader)
        {
            if (salaryGroupDTO == null || salaryGroupDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _salaryGroupRepository.Get(salaryGroupDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = SalaryGroupFactory.CreateSalaryGroup(salaryGroupDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    _salaryGroupRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<SalaryGroupDTO> FindSalaryGroups(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var salaryGroups = _salaryGroupRepository.GetAll(serviceHeader);

                if (salaryGroups != null && salaryGroups.Any())
                {
                    return salaryGroups.ProjectedAsCollection<SalaryGroupDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<SalaryGroupDTO> FindSalaryGroups(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SalaryGroupSpecifications.DefaultSpec();

                ISpecification<SalaryGroup> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var salaryGroupPagedCollection = _salaryGroupRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (salaryGroupPagedCollection != null)
                {
                    var pageCollection = salaryGroupPagedCollection.PageCollection.ProjectedAsCollection<SalaryGroupDTO>();

                    var itemsCount = salaryGroupPagedCollection.ItemsCount;

                    return new PageCollectionInfo<SalaryGroupDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<SalaryGroupDTO> FindSalaryGroups(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SalaryGroupSpecifications.SalaryGroupFullText(text);

                ISpecification<SalaryGroup> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var salaryGroupCollection = _salaryGroupRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (salaryGroupCollection != null)
                {
                    var pageCollection = salaryGroupCollection.PageCollection.ProjectedAsCollection<SalaryGroupDTO>();

                    var itemsCount = salaryGroupCollection.ItemsCount;

                    return new PageCollectionInfo<SalaryGroupDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public SalaryGroupDTO FindSalaryGroup(Guid salaryGroupId, ServiceHeader serviceHeader)
        {
            if (salaryGroupId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var salaryGroup = _salaryGroupRepository.Get(salaryGroupId, serviceHeader);

                    if (salaryGroup != null)
                    {
                        return salaryGroup.ProjectedAs<SalaryGroupDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public SalaryGroupEntryDTO FindSalaryGroupEntry(Guid salaryGroupEntryId, ServiceHeader serviceHeader)
        {
            if (salaryGroupEntryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var salaryGroupEntry = _salaryGroupEntryRepository.Get(salaryGroupEntryId, serviceHeader);

                    if (salaryGroupEntry != null)
                    {
                        return salaryGroupEntry.ProjectedAs<SalaryGroupEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<SalaryGroupEntryDTO> FindSalaryGroupEntriesBySalaryGroupId(Guid salaryGroupId, ServiceHeader serviceHeader)
        {
            if (salaryGroupId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = SalaryGroupEntrySpecifications.SalaryGroupEntryWithSalaryGroupId(salaryGroupId);

                    ISpecification<SalaryGroupEntry> spec = filter;

                    var salaryGroupEntries = _salaryGroupEntryRepository.AllMatching(spec, serviceHeader);

                    if (salaryGroupEntries != null)
                    {
                        return salaryGroupEntries.ProjectedAsCollection<SalaryGroupEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateSalaryGroupEntries(Guid salaryGroupId, List<SalaryGroupEntryDTO> salaryGroupEntries, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existingSalaryGroupEntries = FindSalaryGroupEntriesBySalaryGroupId(salaryGroupId, serviceHeader);

                if (existingSalaryGroupEntries != null && existingSalaryGroupEntries.Any())
                {
                    var oldSet = from c in existingSalaryGroupEntries ?? new List<SalaryGroupEntryDTO> { } select c;

                    var newSet = from c in salaryGroupEntries ?? new List<SalaryGroupEntryDTO> { } select c;

                    var commonSet = oldSet.Intersect(newSet, new SalaryGroupEntryDTOEqualityComparer());

                    var insertSet = salaryGroupEntries.Where(x => x.Id == Guid.Empty);

                    var deleteSet = oldSet.Except(commonSet, new SalaryGroupEntryDTOEqualityComparer());

                    foreach (var item in insertSet)
                    {
                        var charge = new Charge(item.ChargeType, item.ChargePercentage, item.ChargeFixedAmount);

                        var newSalaryGroupEntry = SalaryGroupEntryFactory.CreateSalaryGroupEntry(salaryGroupId, item.SalaryHeadId, charge, item.MinimumValue, item.RoundingType);

                        _salaryGroupEntryRepository.Add(newSalaryGroupEntry, serviceHeader);

                        // get the specification
                        var filter = SalaryCardSpecifications.SalaryCardWithSalaryGroupId(salaryGroupId);

                        ISpecification<SalaryCard> spec = filter;

                        //Query this criteria
                        var salaryCards = _salaryCardRepository.AllMatching(spec, serviceHeader);

                        if (salaryCards != null && salaryCards.Any())
                        {
                            foreach (var salaryCard in salaryCards)
                            {
                                var salaryCardCharge = new Charge(item.ChargeType, item.ChargePercentage, item.ChargeFixedAmount);

                                var newSalaryCardEntry = SalaryCardEntryFactory.CreateSalaryCardEntry(salaryCard.Id, newSalaryGroupEntry.Id, salaryCardCharge);

                                _salaryCardEntryRepository.Add(newSalaryCardEntry, serviceHeader);
                            }
                        }
                    }

                    foreach (var item in deleteSet)
                    {
                        var matches = FindSalaryCardEntriesBySalaryGroupEntry(item.Id, serviceHeader);

                        if (matches != null && matches.Any())
                        {
                            foreach (var mapping in matches)
                            {
                                var persistedSalaryCardEntry = _salaryCardEntryRepository.Get(mapping.Id, serviceHeader);

                                _salaryCardEntryRepository.Remove(persistedSalaryCardEntry, serviceHeader);
                            }
                        }

                        var persistedSalaryGroupEntry = _salaryGroupEntryRepository.Get(item.Id, serviceHeader);

                        _salaryGroupEntryRepository.Remove(persistedSalaryGroupEntry, serviceHeader);
                    }
                }
                else
                {
                    foreach (var item in salaryGroupEntries)
                    {
                        var charge = new Charge(item.ChargeType, item.ChargePercentage, item.ChargeFixedAmount);

                        var salaryGroupEntry = SalaryGroupEntryFactory.CreateSalaryGroupEntry(salaryGroupId, item.SalaryHeadId, charge, item.MinimumValue, item.RoundingType);

                        _salaryGroupEntryRepository.Add(salaryGroupEntry, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        private List<SalaryCardEntryDTO> FindSalaryCardEntriesBySalaryGroupEntry(Guid salaryGroupEntryId, ServiceHeader serviceHeader)
        {
            if (salaryGroupEntryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = SalaryCardEntrySpecifications.SalaryCardEntryWithSalaryGroupEntryId(salaryGroupEntryId);

                    ISpecification<SalaryCardEntry> spec = filter;

                    var salaryCardEntries = _salaryCardEntryRepository.AllMatching(spec, serviceHeader);

                    if (salaryCardEntries != null)
                    {
                        return salaryCardEntries.ProjectedAsCollection<SalaryCardEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
