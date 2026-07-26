using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.Seedwork;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.IncomeAdjustmentAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public class IncomeAdjustmentAppService : IIncomeAdjustmentAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<IncomeAdjustment> _incomeAdjustmentRepository;

        public IncomeAdjustmentAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<IncomeAdjustment> incomeAdjustmentRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (incomeAdjustmentRepository == null)
                throw new ArgumentNullException(nameof(incomeAdjustmentRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _incomeAdjustmentRepository = incomeAdjustmentRepository;
        }

        public IncomeAdjustmentDTO AddNewIncomeAdjustment(IncomeAdjustmentDTO incomeAdjustmentDTO, ServiceHeader serviceHeader)
        {
            if (incomeAdjustmentDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    ISpecification<IncomeAdjustment> spec = IncomeAdjustmentSpecifications.IncomeAdjustmentDescription(incomeAdjustmentDTO.Description);

                    var matchedIncomeAdjustment = _incomeAdjustmentRepository.AllMatching(spec, serviceHeader);

                    if (matchedIncomeAdjustment != null && matchedIncomeAdjustment.Any())
                    {
                        //throw new InvalidOperationException(string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode));
                        incomeAdjustmentDTO.ErrorMessageResult = string.Format("Income Adjustment \"{0}\" already exists!", incomeAdjustmentDTO.Description.ToUpper());
                        return incomeAdjustmentDTO;
                    }
                    else
                    {
                        var incomeAdjustment = IncomeAdjustmentFactory.CreateIncomeAdjustment(incomeAdjustmentDTO.Description, incomeAdjustmentDTO.Type);

                        if (incomeAdjustmentDTO.IsLocked)
                            incomeAdjustment.Lock();
                        else incomeAdjustment.UnLock();

                        _incomeAdjustmentRepository.Add(incomeAdjustment, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return incomeAdjustment.ProjectedAs<IncomeAdjustmentDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateIncomeAdjustment(IncomeAdjustmentDTO incomeAdjustmentDTO, ServiceHeader serviceHeader)
        {
            if (incomeAdjustmentDTO == null || incomeAdjustmentDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _incomeAdjustmentRepository.Get(incomeAdjustmentDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = IncomeAdjustmentFactory.CreateIncomeAdjustment(incomeAdjustmentDTO.Description, incomeAdjustmentDTO.Type);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (incomeAdjustmentDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _incomeAdjustmentRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<IncomeAdjustmentDTO> FindIncomeAdjustments(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var incomeAdjustments = _incomeAdjustmentRepository.GetAll(serviceHeader);

                if (incomeAdjustments != null && incomeAdjustments.Any())
                {
                    return incomeAdjustments.ProjectedAsCollection<IncomeAdjustmentDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<IncomeAdjustmentDTO> FindIncomeAdjustments(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = IncomeAdjustmentSpecifications.DefaultSpec();

                ISpecification<IncomeAdjustment> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var incomeAdjustmentPagedCollection = _incomeAdjustmentRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (incomeAdjustmentPagedCollection != null)
                {
                    var pageCollection = incomeAdjustmentPagedCollection.PageCollection.ProjectedAsCollection<IncomeAdjustmentDTO>();

                    var itemsCount = incomeAdjustmentPagedCollection.ItemsCount;

                    return new PageCollectionInfo<IncomeAdjustmentDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<IncomeAdjustmentDTO> FindIncomeAdjustments(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = IncomeAdjustmentSpecifications.IncomeAdjustmentFullText(text);

                ISpecification<IncomeAdjustment> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var incomeAdjustmentCollection = _incomeAdjustmentRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (incomeAdjustmentCollection != null)
                {
                    var pageCollection = incomeAdjustmentCollection.PageCollection.ProjectedAsCollection<IncomeAdjustmentDTO>();

                    var itemsCount = incomeAdjustmentCollection.ItemsCount;

                    return new PageCollectionInfo<IncomeAdjustmentDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public IncomeAdjustmentDTO FindIncomeAdjustment(Guid incomeAdjustmentId, ServiceHeader serviceHeader)
        {
            if (incomeAdjustmentId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var incomeAdjustment = _incomeAdjustmentRepository.Get(incomeAdjustmentId, serviceHeader);

                    if (incomeAdjustment != null)
                    {
                        return incomeAdjustment.ProjectedAs<IncomeAdjustmentDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
