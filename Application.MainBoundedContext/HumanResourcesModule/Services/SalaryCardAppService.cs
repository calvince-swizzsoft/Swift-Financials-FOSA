using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardEntryAgg;
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
    public class SalaryCardAppService : ISalaryCardAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<SalaryCard> _salaryCardRepository;
        private readonly IRepository<SalaryCardEntry> _salaryCardEntryRepository;
        private readonly ISalaryGroupAppService _salaryGroupAppService;

        public SalaryCardAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<SalaryCard> salaryCardRepository,
           IRepository<SalaryCardEntry> salaryCardEntryRepository,
           ISalaryGroupAppService salaryGroupAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (salaryCardRepository == null)
                throw new ArgumentNullException(nameof(salaryCardRepository));

            if (salaryCardEntryRepository == null)
                throw new ArgumentNullException(nameof(salaryCardEntryRepository));

            if (salaryGroupAppService == null)
                throw new ArgumentNullException(nameof(salaryGroupAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _salaryCardRepository = salaryCardRepository;
            _salaryCardEntryRepository = salaryCardEntryRepository;
            _salaryGroupAppService = salaryGroupAppService;
        }

        public SalaryCardDTO AddNewSalaryCard(SalaryCardDTO salaryCardDTO, ServiceHeader serviceHeader)
        {
            if (salaryCardDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    // get the specification
                    var filter = SalaryCardSpecifications.SalaryCardWithEmployeeId(salaryCardDTO.EmployeeId);

                    ISpecification<SalaryCard> spec = filter;

                    //Query this criteria
                    var salaryCards = _salaryCardRepository.AllMatching(spec, serviceHeader);

                    if (salaryCards != null && salaryCards.Any())
                        return null;
                    else
                    {
                        var salaryCard = SalaryCardFactory.CreateSalaryCard(salaryCardDTO.EmployeeId, salaryCardDTO.SalaryGroupId, salaryCardDTO.TaxExemption, salaryCardDTO.IsTaxExempt, salaryCardDTO.InsuranceReliefAmount, salaryCardDTO.Remarks);

                        salaryCard.CardNumber = _salaryCardRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(CardNumber),0) + 1 AS Expr1 FROM {0}SalaryCards", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();

                        if (salaryCardDTO.IsLocked)
                            salaryCard.Lock();
                        else salaryCard.UnLock();

                        _salaryCardRepository.Add(salaryCard, serviceHeader);

                        var salaryGroupEntries = _salaryGroupAppService.FindSalaryGroupEntriesBySalaryGroupId(salaryCardDTO.SalaryGroupId, serviceHeader);

                        if (salaryGroupEntries != null && salaryGroupEntries.Any())
                        {
                            foreach (var item in salaryGroupEntries)
                            {
                                var charge = new Charge(item.ChargeType, item.ChargePercentage, item.ChargeFixedAmount);

                                var salaryCardEntry = SalaryCardEntryFactory.CreateSalaryCardEntry(salaryCard.Id, item.Id, charge);

                                _salaryCardEntryRepository.Add(salaryCardEntry, serviceHeader);
                            }
                        }

                        dbContextScope.SaveChanges(serviceHeader);

                        return salaryCard.ProjectedAs<SalaryCardDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateSalaryCard(SalaryCardDTO salaryCardDTO, ServiceHeader serviceHeader)
        {
            if (salaryCardDTO == null || salaryCardDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _salaryCardRepository.Get(salaryCardDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = SalaryCardFactory.CreateSalaryCard(persisted.EmployeeId, salaryCardDTO.SalaryGroupId, salaryCardDTO.TaxExemption, salaryCardDTO.IsTaxExempt, salaryCardDTO.InsuranceReliefAmount, salaryCardDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CardNumber = persisted.CardNumber;


                    if (salaryCardDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _salaryCardRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool ResetSalaryCardEntries(SalaryCardDTO salaryCardDTO, ServiceHeader serviceHeader)
        {
            if (salaryCardDTO == null || salaryCardDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _salaryCardRepository.Get(salaryCardDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    if (persisted.SalaryCardEntries != null && persisted.SalaryCardEntries.Any())
                    {
                        persisted.SalaryCardEntries.ToList().ForEach(x =>
                        {
                            _salaryCardEntryRepository.Remove(x, serviceHeader);
                        });
                    }

                    var salaryGroupEntries = _salaryGroupAppService.FindSalaryGroupEntriesBySalaryGroupId(salaryCardDTO.SalaryGroupId, serviceHeader);

                    if (salaryGroupEntries != null && salaryGroupEntries.Any())
                    {
                        foreach (var item in salaryGroupEntries)
                        {
                            var charge = new Charge(item.ChargeType, item.ChargePercentage, item.ChargeFixedAmount);

                            var salaryCardEntry = SalaryCardEntryFactory.CreateSalaryCardEntry(persisted.Id, item.Id, charge);

                            _salaryCardEntryRepository.Add(salaryCardEntry, serviceHeader);
                        }
                    }

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool UpdateSalaryCardEntry(SalaryCardEntryDTO salaryCardEntryDTO, ServiceHeader serviceHeader)
        {
            if (salaryCardEntryDTO == null || salaryCardEntryDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _salaryCardEntryRepository.Get(salaryCardEntryDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var charge = new Charge(salaryCardEntryDTO.ChargeType, salaryCardEntryDTO.ChargePercentage, salaryCardEntryDTO.ChargeFixedAmount);

                    var current = SalaryCardEntryFactory.CreateSalaryCardEntry(persisted.SalaryCardId, persisted.SalaryGroupEntryId, charge);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    
                    _salaryCardEntryRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool ZeroizeOneOffEarnings(Guid salaryCardId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var salaryCardEntries = FindSalaryCardEntriesBySalaryCardId(salaryCardId, serviceHeader);

            if (salaryCardEntries != null && salaryCardEntries.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    foreach (var salaryCardEntry in salaryCardEntries)
                    {
                        switch ((SalaryHeadType)salaryCardEntry.SalaryGroupEntrySalaryHeadType)
                        {
                            case SalaryHeadType.OtherEarning:

                                if (salaryCardEntry.SalaryGroupEntrySalaryHeadIsOneOff)
                                {
                                    var persisted = _salaryCardEntryRepository.Get(salaryCardEntry.Id, serviceHeader);

                                    persisted.Charge = new Charge(salaryCardEntry.SalaryGroupEntryChargeType, 0d, 0m);
                                }

                                break;
                            default:
                                break;
                        }
                    }

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }

            return result;
        }

        public List<SalaryCardDTO> FindSalaryCards(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var salaryCards = _salaryCardRepository.GetAll(serviceHeader);

                if (salaryCards != null && salaryCards.Any())
                {
                    return salaryCards.ProjectedAsCollection<SalaryCardDTO>();
                }
                else return null;
            }
        }
       
        public PageCollectionInfo<SalaryCardDTO> FindSalaryCards(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SalaryCardSpecifications.DefaultSpec();

                ISpecification<SalaryCard> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var salaryCardPagedCollection = _salaryCardRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (salaryCardPagedCollection != null)
                {
                    var pageCollection = salaryCardPagedCollection.PageCollection.ProjectedAsCollection<SalaryCardDTO>();

                    var itemsCount = salaryCardPagedCollection.ItemsCount;

                    return new PageCollectionInfo<SalaryCardDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<SalaryCardDTO> FindSalaryCards(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? SalaryCardSpecifications.DefaultSpec() : SalaryCardSpecifications.SalaryCardFullText(text);

                ISpecification<SalaryCard> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var salaryCardPagedCollection = _salaryCardRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (salaryCardPagedCollection != null)
                {
                    var pageCollection = salaryCardPagedCollection.PageCollection.ProjectedAsCollection<SalaryCardDTO>();

                    var itemsCount = salaryCardPagedCollection.ItemsCount;

                    return new PageCollectionInfo<SalaryCardDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public SalaryCardDTO FindSalaryCard(Guid salaryCardId, ServiceHeader serviceHeader)
        {
            if (salaryCardId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var salaryCard = _salaryCardRepository.Get(salaryCardId, serviceHeader);

                    if (salaryCard != null)
                    {
                        return salaryCard.ProjectedAs<SalaryCardDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public SalaryCardDTO FindSalaryCardByEmployeeId(Guid employeeId, ServiceHeader serviceHeader)
        {
            if (employeeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    // get the specification
                    var filter = SalaryCardSpecifications.SalaryCardWithEmployeeId(employeeId);

                    ISpecification<SalaryCard> spec = filter;

                    //Query this criteria
                    var salaryCards = _salaryCardRepository.AllMatching(spec, serviceHeader);

                    if (salaryCards != null && salaryCards.Any() && salaryCards.Count() == 1)
                    {
                        var salaryCard = salaryCards.SingleOrDefault();

                        if (salaryCard != null)
                        {
                            return salaryCard.ProjectedAs<SalaryCardDTO>();
                        }
                        else return null;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<SalaryCardEntryDTO> FindSalaryCardEntriesBySalaryCardId(Guid salaryCardId, ServiceHeader serviceHeader)
        {
            if (salaryCardId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = SalaryCardEntrySpecifications.SalaryCardEntryWithSalaryCardId(salaryCardId);

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
