using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class ConditionalLendingAppService : IConditionalLendingAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ConditionalLending> _conditionalLendingRepository;
        private readonly IRepository<ConditionalLendingEntry> _conditionalLendingEntryRepository;

        public ConditionalLendingAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<ConditionalLending> conditionalLendingRepository,
           IRepository<ConditionalLendingEntry> conditionalLendingEntryRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (conditionalLendingRepository == null)
                throw new ArgumentNullException(nameof(conditionalLendingRepository));

            if (conditionalLendingEntryRepository == null)
                throw new ArgumentNullException(nameof(conditionalLendingEntryRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _conditionalLendingRepository = conditionalLendingRepository;
            _conditionalLendingEntryRepository = conditionalLendingEntryRepository;
        }

        public async Task<ConditionalLendingDTO> AddNewConditionalLendingAsync(ConditionalLendingDTO conditionalLendingDTO, ServiceHeader serviceHeader)
        {
            var conditionalLendingBindingModel = conditionalLendingDTO.ProjectedAs<ConditionalLendingBindingModel>();

            conditionalLendingBindingModel.ValidateAll();

            if (conditionalLendingBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, conditionalLendingBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // get the specification
                var filter = ConditionalLendingSpecifications.ConditionalLendingWithLoanProductId(conditionalLendingDTO.LoanProductId);

                ISpecification<ConditionalLending> spec = filter;

                //Query this criteria
                var conditionalLendings = await _conditionalLendingRepository.AllMatchingAsync(spec, serviceHeader);

                if (conditionalLendings != null && conditionalLendings.Any())
                {
                    conditionalLendingDTO.ErrorMessageResult = string.Format("Sorry, but the selected loan product already exists in conditional lendings!");
                    return conditionalLendingDTO;
                }
                else
                {
                    var conditionalLending = ConditionalLendingFactory.CreateConditionalLending(conditionalLendingDTO.LoanProductId, conditionalLendingDTO.Description);

                    conditionalLending.CreatedBy = serviceHeader.ApplicationUserName;

                    _conditionalLendingRepository.Add(conditionalLending, serviceHeader);

                    return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? conditionalLending.ProjectedAs<ConditionalLendingDTO>() : null;
                }
            }
        }

        public async Task<bool> UpdateConditionalLendingAsync(ConditionalLendingDTO conditionalLendingDTO, ServiceHeader serviceHeader)
        {
            var conditionalLendingBindingModel = conditionalLendingDTO.ProjectedAs<ConditionalLendingBindingModel>();

            conditionalLendingBindingModel.ValidateAll();

            if (conditionalLendingBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, conditionalLendingBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _conditionalLendingRepository.GetAsync(conditionalLendingDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = ConditionalLendingFactory.CreateConditionalLending(conditionalLendingDTO.LoanProductId, conditionalLendingDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    _conditionalLendingRepository.Merge(persisted, current, serviceHeader);

                    return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
                }
                else throw new InvalidOperationException("Sorry, but the persisted entity could not be identified!");
            }
        }

        public async Task<ConditionalLendingEntryDTO> AddNewConditionalLendingEntryAsync(ConditionalLendingEntryDTO conditionalLendingEntryDTO, ServiceHeader serviceHeader)
        {
            var conditionalLendingEntryBindingModel = conditionalLendingEntryDTO.ProjectedAs<ConditionalLendingEntryBindingModel>();

            conditionalLendingEntryBindingModel.ValidateAll();

            if (conditionalLendingEntryBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, conditionalLendingEntryBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var similarMembers = await _conditionalLendingEntryRepository.AllMatchingCountAsync(ConditionalLendingEntrySpecifications.ConditionalLendingEntryWithCustomerIdAndConditionalLendingId(conditionalLendingEntryDTO.CustomerId, conditionalLendingEntryDTO.ConditionalLendingId), serviceHeader);

                if (similarMembers > 0)
                    throw new InvalidOperationException("Sorry, but the selected customer is already linked to conditional lending!");

                var conditionalLendingEntry = ConditionalLendingEntryFactory.CreateConditionalLendingEntry(conditionalLendingEntryDTO.ConditionalLendingId, conditionalLendingEntryDTO.CustomerId, conditionalLendingEntryDTO.Remarks);

                conditionalLendingEntry.CreatedBy = serviceHeader.ApplicationUserName;

                _conditionalLendingEntryRepository.Add(conditionalLendingEntry, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? conditionalLendingEntry.ProjectedAs<ConditionalLendingEntryDTO>() : null;
            }
        }

        public async Task<bool> RemoveConditionalLendingEntriesAsync(List<ConditionalLendingEntryDTO> conditionalLendingEntryDTOs, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in conditionalLendingEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = await _conditionalLendingEntryRepository.GetAsync(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _conditionalLendingEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> UpdateConditionalLendingEntryCollectionAsync(Guid conditionalLendingId, List<ConditionalLendingEntryDTO> conditionalLendingEntryCollection, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _conditionalLendingRepository.GetAsync(conditionalLendingId, serviceHeader);

                if (persisted != null)
                {
                    var existing = await FindConditionalLendingEntriesByConditionalLendingIdAsync(persisted.Id, serviceHeader);

                    if (existing != null && existing.Any())
                    {
                        foreach (var item in existing)
                        {
                            var conditionalLendingEntry = await _conditionalLendingEntryRepository.GetAsync(item.Id, serviceHeader);

                            if (conditionalLendingEntry != null)
                            {
                                _conditionalLendingEntryRepository.Remove(conditionalLendingEntry, serviceHeader);
                            }
                        }
                    }

                    if (conditionalLendingEntryCollection.Any())
                    {
                        foreach (var item in conditionalLendingEntryCollection)
                        {
                            var conditionalLendingEntry = ConditionalLendingEntryFactory.CreateConditionalLendingEntry(persisted.Id, item.CustomerId, item.Remarks);

                            conditionalLendingEntry.CreatedBy = serviceHeader.ApplicationUserName;

                            _conditionalLendingEntryRepository.Add(conditionalLendingEntry, serviceHeader);
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<List<ConditionalLendingDTO>> FindConditionalLendingsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _conditionalLendingRepository.GetAllAsync<ConditionalLendingDTO>(serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<ConditionalLendingDTO>> FindConditionalLendingsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ConditionalLendingSpecifications.ConditionalLendingFullText(text);

                ISpecification<ConditionalLending> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _conditionalLendingRepository.AllMatchingPagedAsync<ConditionalLendingDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<ConditionalLendingDTO> FindConditionalLendingAsync(Guid conditionalLendingId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _conditionalLendingRepository.GetAsync<ConditionalLendingDTO>(conditionalLendingId, serviceHeader);
            }
        }

        public async Task<List<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByConditionalLendingIdAsync(Guid conditionalLendingId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ConditionalLendingEntrySpecifications.ConditionalLendingEntryWithConditionalLendingId(conditionalLendingId, string.Empty);

                ISpecification<ConditionalLendingEntry> spec = filter;

                return await _conditionalLendingEntryRepository.AllMatchingAsync<ConditionalLendingEntryDTO>(spec, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByConditionalLendingIdAsync(Guid conditionalLendingId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ConditionalLendingEntrySpecifications.ConditionalLendingEntryWithConditionalLendingId(conditionalLendingId, text);

                ISpecification<ConditionalLendingEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _conditionalLendingEntryRepository.AllMatchingPagedAsync<ConditionalLendingEntryDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<List<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ConditionalLendingEntrySpecifications.ConditionalLendingEntryWithCustomerId(customerId);

                ISpecification<ConditionalLendingEntry> spec = filter;

                return await _conditionalLendingEntryRepository.AllMatchingAsync<ConditionalLendingEntryDTO>(spec, serviceHeader);
            }
        }

        public async Task<bool> FetchCustomerConditionalLendingStatusAsync(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _conditionalLendingEntryRepository.AllMatchingCountAsync(ConditionalLendingEntrySpecifications.ConditionalLendingEntryWithCustomerIdAndLoanProductId(customerId, loanProductId), serviceHeader) > 0;
            }
        }
    }
}
