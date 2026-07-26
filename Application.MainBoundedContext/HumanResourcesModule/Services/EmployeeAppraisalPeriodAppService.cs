using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodRecommendationAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class EmployeeAppraisalPeriodAppService : IEmployeeAppraisalPeriodAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<EmployeeAppraisalPeriod> _employeeAppraisalPeriodRepository;
        private readonly IRepository<EmployeeAppraisalPeriodRecommendation> _employeeAppraisalPeriodRecommendationRepository;
        private readonly IEmployeeAppraisalAppService _employeeAppraisalAppService;

        public EmployeeAppraisalPeriodAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<EmployeeAppraisalPeriod> employeeAppraisalPeriodRepository,
            IRepository<EmployeeAppraisalPeriodRecommendation> employeeAppraisalPeriodRecommendationRepository,
            IEmployeeAppraisalAppService employeeAppraisalAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employeeAppraisalPeriodRepository == null)
                throw new ArgumentNullException(nameof(employeeAppraisalPeriodRepository));

            if (employeeAppraisalPeriodRecommendationRepository == null)
                throw new ArgumentNullException(nameof(_employeeAppraisalPeriodRecommendationRepository));

            if (employeeAppraisalAppService == null)
                throw new ArgumentNullException(nameof(employeeAppraisalAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _employeeAppraisalPeriodRepository = employeeAppraisalPeriodRepository;
            _employeeAppraisalPeriodRecommendationRepository = employeeAppraisalPeriodRecommendationRepository;
            _employeeAppraisalAppService = employeeAppraisalAppService;
        }

        #region EmployeeAppraisalPeriodDTO

        public EmployeeAppraisalPeriodDTO AddNewEmployeeAppraisalPeriod(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO, ServiceHeader serviceHeader)
        {
            if (employeeAppraisalPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var duration = new Duration(employeeAppraisalPeriodDTO.DurationStartDate, employeeAppraisalPeriodDTO.DurationEndDate);

                    var employeeAppraisalPeriod = EmployeeAppraisalPeriodFactory.CreateEmployeeAppraisalPeriod(employeeAppraisalPeriodDTO.Description, duration);

                    employeeAppraisalPeriod.CreatedBy = serviceHeader.ApplicationUserName;

                    _employeeAppraisalPeriodRepository.Add(employeeAppraisalPeriod, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return employeeAppraisalPeriod.ProjectedAs<EmployeeAppraisalPeriodDTO>();
                }
            }
            else return null;
        }

        public bool UpdateEmployeeAppraisalPeriod(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO, ServiceHeader serviceHeader)
        {
            if (employeeAppraisalPeriodDTO == null || employeeAppraisalPeriodDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employeeAppraisalPeriodRepository.Get(employeeAppraisalPeriodDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var duration = new Duration(employeeAppraisalPeriodDTO.DurationStartDate, employeeAppraisalPeriodDTO.DurationEndDate);

                    var current = EmployeeAppraisalPeriodFactory.CreateEmployeeAppraisalPeriod(employeeAppraisalPeriodDTO.Description, duration);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    _employeeAppraisalPeriodRepository.Merge(persisted, current, serviceHeader);

                    // Lock?
                    if (employeeAppraisalPeriodDTO.IsLocked && !persisted.IsLocked)
                        LockEmployeeAppraisalPeriod(persisted.Id, serviceHeader);

                    // Activate?
                    if (employeeAppraisalPeriodDTO.IsActive && !persisted.IsActive)
                        ActivateEmployeeAppraisalPeriod(persisted.Id, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriods(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var employeeAppraisalPeriods = _employeeAppraisalPeriodRepository.GetAll(serviceHeader);

                if (employeeAppraisalPeriods != null && employeeAppraisalPeriods.Any())
                {
                    return employeeAppraisalPeriods.ProjectedAsCollection<EmployeeAppraisalPeriodDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeAppraisalPeriodSpecifications.DefaultSpec();

                ISpecification<EmployeeAppraisalPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var EmployeeAppraisalPeriodPagedCollection = _employeeAppraisalPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (EmployeeAppraisalPeriodPagedCollection != null)
                {
                    var pageCollection = EmployeeAppraisalPeriodPagedCollection.PageCollection.ProjectedAsCollection<EmployeeAppraisalPeriodDTO>();

                    var itemsCount = EmployeeAppraisalPeriodPagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeAppraisalPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeAppraisalPeriodSpecifications.EmployeeAppraisalPeriodFullText(text);

                ISpecification<EmployeeAppraisalPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeAppraisalPeriodCollection = _employeeAppraisalPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeAppraisalPeriodCollection != null)
                {
                    var pageCollection = employeeAppraisalPeriodCollection.PageCollection.ProjectedAsCollection<EmployeeAppraisalPeriodDTO>();

                    var itemsCount = employeeAppraisalPeriodCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeAppraisalPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public EmployeeAppraisalPeriodDTO FindCurrentEmployeeAppraisalPeriod(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeAppraisalPeriodSpecifications.CurrentEmployeeAppraisalPeriod();

                ISpecification<EmployeeAppraisalPeriod> spec = filter;

                var employeeAppraisalPeriods = _employeeAppraisalPeriodRepository.AllMatching(spec, serviceHeader);

                if (employeeAppraisalPeriods != null && employeeAppraisalPeriods.Any() && employeeAppraisalPeriods.Count() == 1)
                {
                    var employeeAppraisalPeriod = employeeAppraisalPeriods.SingleOrDefault();

                    if (employeeAppraisalPeriod != null)
                    {
                        return employeeAppraisalPeriod.ProjectedAs<EmployeeAppraisalPeriodDTO>();
                    }
                    else return null;
                }
                else return null;
            }
        }

        private bool ActivateEmployeeAppraisalPeriod(Guid employeeAppraisalPeriodId, ServiceHeader serviceHeader)
        {
            if (employeeAppraisalPeriodId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employeeAppraisalPeriodRepository.Get(employeeAppraisalPeriodId, serviceHeader);

                if (persisted != null)
                {
                    persisted.Activate();

                    var otherPeriods = _employeeAppraisalPeriodRepository.GetAll(serviceHeader);

                    foreach (var item in otherPeriods)
                    {
                        if (item.Id != persisted.Id)
                        {
                            var employeeAppraisalPeriod = _employeeAppraisalPeriodRepository.Get(item.Id, serviceHeader);

                            employeeAppraisalPeriod.DeActivate();
                        }
                    }

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        private bool LockEmployeeAppraisalPeriod(Guid employeeAppraisalPeriodId, ServiceHeader serviceHeader)
        {
            if (employeeAppraisalPeriodId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employeeAppraisalPeriodRepository.Get(employeeAppraisalPeriodId, serviceHeader);

                if (persisted != null)
                {
                    persisted.Lock();

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public EmployeeAppraisalPeriodDTO FindEmployeeAppraisalPeriod(Guid employeeAppraisalPeriodId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _employeeAppraisalPeriodRepository.Get<EmployeeAppraisalPeriodDTO>(employeeAppraisalPeriodId, serviceHeader);
            }
        }

        #endregion

        #region EmployeeAppraisalPeriodRecommendationDTO

        public bool UpdateEmployeeAppraisalPeriodRecommendation(EmployeeAppraisalPeriodRecommendationDTO employeeAppraisalPeriodRecommendationDTO, ServiceHeader serviceHeader)
        {
            var employeeAppraisalPeriodRecommendationBindingModelBindingModel = employeeAppraisalPeriodRecommendationDTO.ProjectedAs<EmployeeAppraisalPeriodRecommendationBindingModel>();

            employeeAppraisalPeriodRecommendationBindingModelBindingModel.ValidateAll();

            var employeeAppraisalPeriod = FindEmployeeAppraisalPeriod(employeeAppraisalPeriodRecommendationDTO.EmployeeAppraisalPeriodId, serviceHeader);

            if (DateTime.Now > employeeAppraisalPeriod.DurationEndDate) throw new InvalidOperationException(string.Format("Duration for {0} is over.", employeeAppraisalPeriod.Description));

            var employeeAppraisals = _employeeAppraisalAppService.FindEmployeeAppraisals(employeeAppraisalPeriodRecommendationDTO.EmployeeId, employeeAppraisalPeriodRecommendationDTO.EmployeeAppraisalPeriodId, serviceHeader);

            if (employeeAppraisals == null) throw new InvalidOperationException("Employee has not completed the self appraisal yet!");

            if (employeeAppraisalPeriodRecommendationBindingModelBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, employeeAppraisalPeriodRecommendationBindingModelBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                if (employeeAppraisalPeriodRecommendationDTO.Id == Guid.Empty)
                {
                    var employeeAppraisalPeriodRecommendation = EmployeeAppraisalPeriodRecommendationFactory.CreateEmployeeAppraisalPeriodRecommendation(employeeAppraisalPeriodRecommendationDTO.EmployeeId, employeeAppraisalPeriodRecommendationDTO.EmployeeAppraisalPeriodId, employeeAppraisalPeriodRecommendationDTO.Recommendation);

                    employeeAppraisalPeriodRecommendation.CreatedBy = serviceHeader.ApplicationUserName;

                    _employeeAppraisalPeriodRecommendationRepository.Add(employeeAppraisalPeriodRecommendation, serviceHeader);
                }
                else
                {
                    var persisted = _employeeAppraisalPeriodRecommendationRepository.Get(employeeAppraisalPeriodRecommendationDTO.Id, serviceHeader);

                    if (persisted != null)
                    {
                        var current = EmployeeAppraisalPeriodRecommendationFactory.CreateEmployeeAppraisalPeriodRecommendation(employeeAppraisalPeriodRecommendationDTO.EmployeeId, employeeAppraisalPeriodRecommendationDTO.EmployeeAppraisalPeriodId, employeeAppraisalPeriodRecommendationDTO.Recommendation);

                        current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                        _employeeAppraisalPeriodRecommendationRepository.Merge(persisted, current, serviceHeader);
                    }
                }
                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public EmployeeAppraisalPeriodRecommendationDTO FindEmployeeAppraisalPeriodRecommendation(Guid employeeId, Guid employeeAppraisalPeriodId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeAppraisalPeriodRecommendationSpecifications.EmployeeAppraisalPeriodRecommendation(employeeId, employeeAppraisalPeriodId);

                ISpecification<EmployeeAppraisalPeriodRecommendation> spec = filter;

                var employeeAppraisalPeriodRecommendation = _employeeAppraisalPeriodRecommendationRepository.AllMatching<EmployeeAppraisalPeriodRecommendationDTO>(spec, serviceHeader);

                return employeeAppraisalPeriodRecommendation.FirstOrDefault();
            }
        }
        #endregion
    }
}
