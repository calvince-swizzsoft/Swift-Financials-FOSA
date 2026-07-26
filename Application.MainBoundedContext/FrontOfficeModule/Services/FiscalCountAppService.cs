using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FiscalCountAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class FiscalCountAppService : IFiscalCountAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<FiscalCount> _fiscalCountRepository;

        public FiscalCountAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<FiscalCount> fiscalCountRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (fiscalCountRepository == null)
                throw new ArgumentNullException(nameof(fiscalCountRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _fiscalCountRepository = fiscalCountRepository;
        }

        public FiscalCountDTO AddNewFiscalCount(FiscalCountDTO fiscalCountDTO, ServiceHeader serviceHeader)
        {
            if (fiscalCountDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var denomination = new Denomination(fiscalCountDTO.DenominationOneThousandValue, fiscalCountDTO.DenominationFiveHundredValue, fiscalCountDTO.DenominationTwoHundredValue, fiscalCountDTO.DenominationOneHundredValue, fiscalCountDTO.DenominationFiftyValue, fiscalCountDTO.DenominationFourtyValue, fiscalCountDTO.DenominationTwentyValue, fiscalCountDTO.DenominationTenValue, fiscalCountDTO.DenominationFiveValue, fiscalCountDTO.DenominationOneValue, fiscalCountDTO.DenominationFiftyCentValue);

                    var fiscalCount = FiscalCountFactory.CreateFiscalCount(fiscalCountDTO.PostingPeriodId, fiscalCountDTO.BranchId, fiscalCountDTO.ChartOfAccountId, fiscalCountDTO.PrimaryDescription, fiscalCountDTO.SecondaryDescription, fiscalCountDTO.Reference, denomination, fiscalCountDTO.TransactionCode);

                    fiscalCount.CreatedBy = serviceHeader.ApplicationUserName;

                    _fiscalCountRepository.Add(fiscalCount, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return fiscalCount.ProjectedAs<FiscalCountDTO>();
                }
            }
            else return null;
        }

        public bool AddNewFiscalCounts(List<FiscalCountDTO> fiscalCountDTOs, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (fiscalCountDTOs != null && fiscalCountDTOs.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    foreach (var fiscalCountDTO in fiscalCountDTOs)
                    {
                        var denomination = new Denomination(fiscalCountDTO.DenominationOneThousandValue, fiscalCountDTO.DenominationFiveHundredValue, fiscalCountDTO.DenominationTwoHundredValue, fiscalCountDTO.DenominationOneHundredValue, fiscalCountDTO.DenominationFiftyValue, fiscalCountDTO.DenominationFourtyValue, fiscalCountDTO.DenominationTwentyValue, fiscalCountDTO.DenominationTenValue, fiscalCountDTO.DenominationFiveValue, fiscalCountDTO.DenominationOneValue, fiscalCountDTO.DenominationFiftyCentValue);

                        var fiscalCount = FiscalCountFactory.CreateFiscalCount(fiscalCountDTO.PostingPeriodId, fiscalCountDTO.BranchId, fiscalCountDTO.ChartOfAccountId, fiscalCountDTO.PrimaryDescription, fiscalCountDTO.SecondaryDescription, fiscalCountDTO.Reference, denomination, fiscalCountDTO.TransactionCode);

                        fiscalCount.CreatedBy = serviceHeader.ApplicationUserName;

                        _fiscalCountRepository.Add(fiscalCount, serviceHeader);
                    }

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }

            return result;
        }

        public List<FiscalCountDTO> FindFiscalCounts(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var fiscalCounts = _fiscalCountRepository.GetAll(serviceHeader);

                if (fiscalCounts != null && fiscalCounts.Any())
                {
                    return fiscalCounts.ProjectedAsCollection<FiscalCountDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<FiscalCountDTO> FindFiscalCounts(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FiscalCountSpecifications.DefaultSpec();

                ISpecification<FiscalCount> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fiscalCountPagedCollection = _fiscalCountRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fiscalCountPagedCollection != null)
                {
                    var pageCollection = fiscalCountPagedCollection.PageCollection.ProjectedAsCollection<FiscalCountDTO>();

                    var itemsCount = fiscalCountPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FiscalCountDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FiscalCountDTO> FindFiscalCounts(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FiscalCountSpecifications.FiscalCountFullText(text);

                ISpecification<FiscalCount> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fiscalCountPagedCollection = _fiscalCountRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fiscalCountPagedCollection != null)
                {
                    var pageCollection = fiscalCountPagedCollection.PageCollection.ProjectedAsCollection<FiscalCountDTO>();

                    var itemsCount = fiscalCountPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FiscalCountDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FiscalCountDTO> FindFiscalCounts(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (startDate != null && endDate != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    //get the specification
                    var filter = FiscalCountSpecifications.FiscalCountWithDateRangeAndFullText(startDate, endDate, text);

                    ISpecification<FiscalCount> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var fiscalCountPagedCollection = _fiscalCountRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (fiscalCountPagedCollection != null)
                    {
                        var pageCollection = fiscalCountPagedCollection.PageCollection.ProjectedAsCollection<FiscalCountDTO>();

                        var itemsCount = fiscalCountPagedCollection.ItemsCount;

                        return new PageCollectionInfo<FiscalCountDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else // No results
                        return null;
                }
            }
            else
                return null;
        }

        public FiscalCountDTO FindFiscalCount(Guid fiscalCountId, ServiceHeader serviceHeader)
        {
            if (fiscalCountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var fiscalCount = _fiscalCountRepository.Get(fiscalCountId, serviceHeader);

                    if (fiscalCount != null)
                    {
                        return fiscalCount.ProjectedAs<FiscalCountDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool IsEndOfDayExecuted(EmployeeDTO employeeDTO, ServiceHeader serviceHeader)
        {
            if (employeeDTO != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var applicationUserName = employeeDTO.ApplicationUserName ?? serviceHeader.ApplicationUserName;

                    var filter = FiscalCountSpecifications.FiscalCountWithDateRangeAndTransactionCodeAndApplicationUserName(DateTime.Today, DateTime.Today, (int)SystemTransactionCode.TellerEndOfDay, applicationUserName);

                    ISpecification<FiscalCount> spec = filter;

                    var fiscalCountsTally = _fiscalCountRepository.AllMatchingCount(spec, serviceHeader);

                    return fiscalCountsTally != 0;
                }
            }
            else return false;
        }

        public async Task<bool> IsEndOfDayExecutedAsync(EmployeeDTO employeeDTO, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var applicationUserName = employeeDTO.ApplicationUserName ?? serviceHeader.ApplicationUserName;

                var filter = FiscalCountSpecifications.FiscalCountWithDateRangeAndTransactionCodeAndApplicationUserName(DateTime.Today, DateTime.Today, (int)SystemTransactionCode.TellerEndOfDay, applicationUserName);

                ISpecification<FiscalCount> spec = filter;

                var fiscalCountsTally = await _fiscalCountRepository.AllMatchingCountAsync(spec, serviceHeader);

                return fiscalCountsTally != 0;
            }
        }
    }
}
