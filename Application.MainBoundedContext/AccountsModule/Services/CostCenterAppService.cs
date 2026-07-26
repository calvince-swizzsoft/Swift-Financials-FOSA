using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CostCenterAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class CostCenterAppService : ICostCenterAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CostCenter> _costCenterRepository;

        public CostCenterAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<CostCenter> costCenterRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (costCenterRepository == null)
                throw new ArgumentNullException(nameof(costCenterRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _costCenterRepository = costCenterRepository;
        }

        public CostCenterDTO AddNewCostCenter(CostCenterDTO costCenterDTO, ServiceHeader serviceHeader)
        {
            if (costCenterDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {

                    ISpecification<CostCenter> spec = CostCenterSpecifications.CostCenterWithCostCenter(costCenterDTO.Description);

                    var matchedCostCenters = _costCenterRepository.AllMatching(spec, serviceHeader);

                    if (matchedCostCenters != null && matchedCostCenters.Any())
                    {
                        //throw new InvalidOperationException(string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode));
                        costCenterDTO.ErrorMessageResult = string.Format("Sorry, but Cost Center \"{0}\" already exists!", costCenterDTO.Description.ToUpper());
                        return costCenterDTO;
                    }
                    else
                    {
                        var costCenter = CostCenterFactory.CreateCostCenter(costCenterDTO.Description);

                        if (costCenterDTO.IsLocked)
                            costCenter.Lock();
                        else costCenter.UnLock();

                        _costCenterRepository.Add(costCenter, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return costCenter.ProjectedAs<CostCenterDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateCostCenter(CostCenterDTO costCenterDTO, ServiceHeader serviceHeader)
        {
            if (costCenterDTO == null || costCenterDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {

                var persisted = _costCenterRepository.Get(costCenterDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = CostCenterFactory.CreateCostCenter(costCenterDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (costCenterDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _costCenterRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<CostCenterDTO> FindCostCenters(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var costCenters = _costCenterRepository.GetAll(serviceHeader);

                if (costCenters != null && costCenters.Any())
                {
                    return costCenters.ProjectedAsCollection<CostCenterDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<CostCenterDTO> FindCostCenters(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CostCenterSpecifications.DefaultSpec();

                ISpecification<CostCenter> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var costCenterPagedCollection = _costCenterRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (costCenterPagedCollection != null)
                {
                    var pageCollection = costCenterPagedCollection.PageCollection.ProjectedAsCollection<CostCenterDTO>();

                    var itemsCount = costCenterPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CostCenterDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<CostCenterDTO> FindCostCenters(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CostCenterSpecifications.CostCenterFullText(text);

                ISpecification<CostCenter> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var costCenterCollection = _costCenterRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (costCenterCollection != null)
                {
                    var pageCollection = costCenterCollection.PageCollection.ProjectedAsCollection<CostCenterDTO>();

                    var itemsCount = costCenterCollection.ItemsCount;

                    return new PageCollectionInfo<CostCenterDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public CostCenterDTO FindCostCenter(Guid costCenterId, ServiceHeader serviceHeader)
        {
            if (costCenterId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var costCenter = _costCenterRepository.Get(costCenterId, serviceHeader);

                    if (costCenter != null)
                    {
                        return costCenter.ProjectedAs<CostCenterDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
