using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CommissionExemptionAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CommissionExemptionEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class CommissionExemptionAppService : ICommissionExemptionAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CommissionExemption> _commissionExemptionRepository;
        private readonly IRepository<CommissionExemptionEntry> _commissionExemptionEntryRepository;
        private readonly IAppCache _appCache;

        public CommissionExemptionAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<CommissionExemption> commissionExemptionRepository,
           IRepository<CommissionExemptionEntry> commissionExemptionEntryRepository,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (commissionExemptionRepository == null)
                throw new ArgumentNullException(nameof(commissionExemptionRepository));

            if (commissionExemptionEntryRepository == null)
                throw new ArgumentNullException(nameof(commissionExemptionEntryRepository));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _commissionExemptionRepository = commissionExemptionRepository;
            _commissionExemptionEntryRepository = commissionExemptionEntryRepository;
            _appCache = appCache;
        }

        public async Task<CommissionExemptionDTO> AddNewCommissionExemptionAsync(CommissionExemptionDTO commissionExemptionDTO, ServiceHeader serviceHeader)
        {
            var commissionExemptionBindingModel = commissionExemptionDTO.ProjectedAs<CommissionExemptionBindingModel>();

            commissionExemptionBindingModel.ValidateAll();

            if (commissionExemptionBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, commissionExemptionBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var filter = CommissionExemptionSpecifications.CommissionExemptionWithCommissionId(commissionExemptionDTO.CommissionId);

                ISpecification<CommissionExemption> spec = filter;

                var commissionExemptions = await _commissionExemptionRepository.AllMatchingAsync(spec, serviceHeader);

                if (commissionExemptions != null && commissionExemptions.Any())
                    throw new InvalidOperationException("Sorry, but the selected charge already exists as an exemption!");
                else
                {
                    var commissionExemption = CommissionExemptionFactory.CreateCommissionExemption(commissionExemptionDTO.CommissionId, commissionExemptionDTO.Description);

                    _commissionExemptionRepository.Add(commissionExemption, serviceHeader);

                    return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? commissionExemption.ProjectedAs<CommissionExemptionDTO>() : null;
                }
            }
        }

        public async Task<bool> UpdateCommissionExemptionAsync(CommissionExemptionDTO commissionExemptionDTO, ServiceHeader serviceHeader)
        {
            var commissionExemptionBindingModel = commissionExemptionDTO.ProjectedAs<CommissionExemptionBindingModel>();

            commissionExemptionBindingModel.ValidateAll();

            if (commissionExemptionBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, commissionExemptionBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _commissionExemptionRepository.GetAsync(commissionExemptionDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = CommissionExemptionFactory.CreateCommissionExemption(commissionExemptionDTO.CommissionId, commissionExemptionDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _commissionExemptionRepository.Merge(persisted, current, serviceHeader);

                    return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
                }
                else throw new InvalidOperationException("Sorry, but the persisted entity could not be identified!");
            }
        }

        public async Task<CommissionExemptionEntryDTO> AddNewCommissionExemptionEntryAsync(CommissionExemptionEntryDTO commissionExemptionEntryDTO, ServiceHeader serviceHeader)
        {
            var commissionExemptionEntryBindingModel = commissionExemptionEntryDTO.ProjectedAs<CommissionExemptionEntryBindingModel>();

            commissionExemptionEntryBindingModel.ValidateAll();

            if (commissionExemptionEntryBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, commissionExemptionEntryBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var similarMembers = await _commissionExemptionEntryRepository.AllMatchingCountAsync(CommissionExemptionEntrySpecifications.CommissionExemptionEntryWithCustomerIdAndCommissionExemptionId(commissionExemptionEntryDTO.CustomerId, commissionExemptionEntryDTO.CommissionExemptionId), serviceHeader);

                if (similarMembers > 0)
                    throw new InvalidOperationException("Sorry, but the selected customer is already linked to exemption!");

                var commissionExemptionEntry = CommissionExemptionEntryFactory.CreateCommissionExemptionEntry(commissionExemptionEntryDTO.CommissionExemptionId, commissionExemptionEntryDTO.CustomerId, commissionExemptionEntryDTO.Remarks);

                commissionExemptionEntry.CreatedBy = serviceHeader.ApplicationUserName;

                _commissionExemptionEntryRepository.Add(commissionExemptionEntry, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? commissionExemptionEntry.ProjectedAs<CommissionExemptionEntryDTO>() : null;
            }
        }

        public async Task<bool> RemoveCommissionExemptionEntriesAsync(List<CommissionExemptionEntryDTO> commissionExemptionEntryDTOs, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in commissionExemptionEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = await _commissionExemptionEntryRepository.GetAsync(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _commissionExemptionEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> UpdateCommissionExemptionEntryCollectionAsync(Guid commissionExemptionId, List<CommissionExemptionEntryDTO> commissionExemptionEntryCollection, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _commissionExemptionRepository.GetAsync(commissionExemptionId, serviceHeader);

                if (persisted != null)
                {
                    var existing = await FindCommissionExemptionEntriesByCommissionExemptionIdAsync(persisted.Id, serviceHeader);

                    if (existing != null && existing.Any())
                    {
                        foreach (var item in existing)
                        {
                            var commissionExemptionEntry = await _commissionExemptionEntryRepository.GetAsync(item.Id, serviceHeader);

                            if (commissionExemptionEntry != null)
                            {
                                _commissionExemptionEntryRepository.Remove(commissionExemptionEntry, serviceHeader);
                            }
                        }
                    }

                    if (commissionExemptionEntryCollection.Any())
                    {
                        foreach (var item in commissionExemptionEntryCollection)
                        {
                            var commissionExemptionEntry = CommissionExemptionEntryFactory.CreateCommissionExemptionEntry(persisted.Id, item.CustomerId, item.Remarks);

                            commissionExemptionEntry.CreatedBy = serviceHeader.ApplicationUserName;

                            _commissionExemptionEntryRepository.Add(commissionExemptionEntry, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) > 0;
            }
        }

        public async Task<List<CommissionExemptionDTO>> FindCommissionExemptionsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _commissionExemptionRepository.GetAllAsync<CommissionExemptionDTO>(serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<CommissionExemptionDTO>> FindCommissionExemptionsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CommissionExemptionSpecifications.CommissionExemptionFullText(text);

                ISpecification<CommissionExemption> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _commissionExemptionRepository.AllMatchingPagedAsync<CommissionExemptionDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<CommissionExemptionDTO> FindCommissionExemptionAsync(Guid commissionExemptionId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _commissionExemptionRepository.GetAsync<CommissionExemptionDTO>(commissionExemptionId, serviceHeader);
            }
        }

        public async Task<List<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCommissionExemptionIdAsync(Guid commissionExemptionId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CommissionExemptionEntrySpecifications.CommissionExemptionEntryWithCommissionExemptionId(commissionExemptionId, string.Empty);

                ISpecification<CommissionExemptionEntry> spec = filter;

                return await _commissionExemptionEntryRepository.AllMatchingAsync<CommissionExemptionEntryDTO>(spec, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCommissionExemptionIdAsync(Guid commissionExemptionId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CommissionExemptionEntrySpecifications.CommissionExemptionEntryWithCommissionExemptionId(commissionExemptionId, text);

                ISpecification<CommissionExemptionEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _commissionExemptionEntryRepository.AllMatchingPagedAsync<CommissionExemptionEntryDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<List<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CommissionExemptionEntrySpecifications.CommissionExemptionEntryWithCustomerId(customerId);

                ISpecification<CommissionExemptionEntry> spec = filter;

                return await _commissionExemptionEntryRepository.AllMatchingAsync<CommissionExemptionEntryDTO>(spec, serviceHeader);
            }
        }

        public bool FetchCachedCustomerCommissionExemptionStatus(CustomerAccountDTO customerAccount, Guid commissionId, ServiceHeader serviceHeader)
        {
            if (customerAccount != null)
            {
                return _appCache.GetOrAdd<bool>(string.Format("CustomerCommissionExemptionStatus_{0}_{1}_{2}", serviceHeader.ApplicationDomainName, customerAccount.CustomerId, commissionId.ToString("D")), () =>
                {
                    return FetchCustomerCommissionExemptionStatus(customerAccount, commissionId, serviceHeader);
                });
            }
            else return false;
        }

        public bool FetchCustomerCommissionExemptionStatus(CustomerAccountDTO customerAccount, Guid commissionId, ServiceHeader serviceHeader)
        {
            if (customerAccount != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var tally = _commissionExemptionEntryRepository.AllMatchingCount(CommissionExemptionEntrySpecifications.CommissionExemptionEntryWithCustomerIdAndCommissionId(customerAccount.CustomerId, commissionId), serviceHeader);

                    return tally > 0;
                }
            }
            else return false;
        }
    }
}
