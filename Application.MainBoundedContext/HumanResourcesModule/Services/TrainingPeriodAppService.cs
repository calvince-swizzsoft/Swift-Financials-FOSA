using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodEntryAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodAgg;
using Domain.Seedwork;
using Numero3.EntityFramework.Interfaces;
using System;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System.Collections.Generic;
using Domain.MainBoundedContext.ValueObjects;
using Application.Seedwork;
using Domain.Seedwork.Specification;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class TrainingPeriodAppService : ITrainingPeriodAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<TrainingPeriod> _trainingPeriodRepository;
        private readonly IRepository<TrainingPeriodEntry> _trainingPeriodEntryRepository;

        public TrainingPeriodAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<TrainingPeriod> trainingPeriodRepository, 
            IRepository<TrainingPeriodEntry> trainingPeriodEntryRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (trainingPeriodRepository == null)
                throw new ArgumentNullException(nameof(trainingPeriodRepository));

            if (trainingPeriodEntryRepository == null)
                throw new ArgumentNullException(nameof(trainingPeriodEntryRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _trainingPeriodRepository = trainingPeriodRepository;
            _trainingPeriodEntryRepository = trainingPeriodEntryRepository;
        }

        #region TrainingPeriodDTO

        public TrainingPeriodDTO AddNewTrainingPeriod(TrainingPeriodDTO trainingPeriodDTO, ServiceHeader serviceHeader)
        {
            if (trainingPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var duration = new Duration(trainingPeriodDTO.DurationStartDate, trainingPeriodDTO.DurationEndDate);

                    var trainingPeriod = TrainingPeriodFactory.CreateTrainingPeriod(trainingPeriodDTO.Description, trainingPeriodDTO.Venue, duration, trainingPeriodDTO.TotalValue, trainingPeriodDTO.DocumentNumber, trainingPeriodDTO.FileName, trainingPeriodDTO.FileTitle, trainingPeriodDTO.FileDescription, trainingPeriodDTO.FileMIMEType, trainingPeriodDTO.Remarks);

                    trainingPeriod.CreatedBy = serviceHeader.ApplicationUserName;

                    _trainingPeriodRepository.Add(trainingPeriod, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return trainingPeriod.ProjectedAs<TrainingPeriodDTO>();
                }
            }
            else return null;
        }

        public bool UpdateTrainingPeriod(TrainingPeriodDTO trainingPeriodDTO, ServiceHeader serviceHeader)
        {
            if (trainingPeriodDTO == null || trainingPeriodDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _trainingPeriodRepository.Get(trainingPeriodDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var duration = new Duration(trainingPeriodDTO.DurationStartDate, trainingPeriodDTO.DurationEndDate);

                    var current = TrainingPeriodFactory.CreateTrainingPeriod(trainingPeriodDTO.Description, trainingPeriodDTO.Venue, duration, trainingPeriodDTO.TotalValue, trainingPeriodDTO.DocumentNumber, trainingPeriodDTO.FileName, trainingPeriodDTO.FileTitle, trainingPeriodDTO.FileDescription, trainingPeriodDTO.FileMIMEType, trainingPeriodDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _trainingPeriodRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public TrainingPeriodDTO FindTrainingPeriod(Guid trainingPeriodId, ServiceHeader serviceHeader)
        {
            if (trainingPeriodId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var trainingPeriod = _trainingPeriodRepository.Get(trainingPeriodId, serviceHeader);

                    if (trainingPeriod != null)
                    {
                        return trainingPeriod.ProjectedAs<TrainingPeriodDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<TrainingPeriodDTO> FindTrainingPeriods(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TrainingPeriodSpecifications.TrainingPeriodWithDurationAndFilterText(startDate, endDate, text);

                ISpecification<TrainingPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var trainingPeriodPagedCollection = _trainingPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (trainingPeriodPagedCollection != null)
                {
                    var pageCollection = trainingPeriodPagedCollection.PageCollection.ProjectedAsCollection<TrainingPeriodDTO>();

                    var itemsCount = trainingPeriodPagedCollection.ItemsCount;

                    return new PageCollectionInfo<TrainingPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        #endregion

        #region TrainingPeriodEntryDTO

        public TrainingPeriodEntryDTO AddNewTrainingPeriodEntry(TrainingPeriodEntryDTO trainingPeriodEntryDTO, ServiceHeader serviceHeader)
        {
            if (trainingPeriodEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var similarMembers = _trainingPeriodEntryRepository.AllMatchingCount(TrainingPeriodEntrySpecifications.TrainingPeriodEntryWithEmployeeIdAndTrainingPeriodId(trainingPeriodEntryDTO.EmployeeId, trainingPeriodEntryDTO.TrainingPeriodId), serviceHeader);

                    if (similarMembers > 0)
                        throw new InvalidOperationException("Sorry, but the selected employee is already linked to training period!");

                    var trainingPeriodEntry = TrainingPeriodEntryFactory.CreateTrainingPeriodEntry(trainingPeriodEntryDTO.EmployeeId, trainingPeriodEntryDTO.TrainingPeriodId);

                    trainingPeriodEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _trainingPeriodEntryRepository.Add(trainingPeriodEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return trainingPeriodEntry.ProjectedAs<TrainingPeriodEntryDTO>();
                }
            }
            else return null;
        }

        public bool UpdateTrainingPeriodEntriesByTrainingPeriodId(Guid trainingId, List<TrainingPeriodEntryDTO> employeeTrainings, ServiceHeader serviceHeader)
        {
            if (trainingId != null && employeeTrainings != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    foreach (var item in employeeTrainings)
                    {
                        if (item.Id == Guid.Empty)
                        {
                            var employeeTraining = TrainingPeriodEntryFactory.CreateTrainingPeriodEntry(item.EmployeeId, trainingId);

                            employeeTraining.CreatedBy = serviceHeader.ApplicationUserName;

                            _trainingPeriodEntryRepository.Add(employeeTraining, serviceHeader);
                        }
                        else
                        {
                            var persisted = _trainingPeriodEntryRepository.Get(item.Id, serviceHeader);

                            var current = TrainingPeriodEntryFactory.CreateTrainingPeriodEntry(item.EmployeeId, trainingId);

                            current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                            _trainingPeriodEntryRepository.Merge(persisted, current, serviceHeader);
                        }
                    }

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public PageCollectionInfo<TrainingPeriodEntryDTO> FindTrainingPeriodEntriesByTrainingPeriodId(Guid trainingId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TrainingPeriodEntrySpecifications.TrainingPeriodEntriesByTrainingPeriodIdWithText(trainingId, text);

                ISpecification<TrainingPeriodEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeTrainingPagedCollection = _trainingPeriodEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeTrainingPagedCollection != null)
                {
                    var pageCollection = employeeTrainingPagedCollection.PageCollection.ProjectedAsCollection<TrainingPeriodEntryDTO>();

                    var itemsCount = employeeTrainingPagedCollection.ItemsCount;

                    return new PageCollectionInfo<TrainingPeriodEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }

                else return null;
            }
        }

        public PageCollectionInfo<TrainingPeriodEntryDTO> FindTrainingPeriodEntriesByEmployeeId(Guid employeeId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TrainingPeriodEntrySpecifications.TrainingPeriodEntriesByEmployeeIdWithText(employeeId, text);

                ISpecification<TrainingPeriodEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeTrainingPagedCollection = _trainingPeriodEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeTrainingPagedCollection != null)
                {
                    var pageCollection = employeeTrainingPagedCollection.PageCollection.ProjectedAsCollection<TrainingPeriodEntryDTO>();

                    var itemsCount = employeeTrainingPagedCollection.ItemsCount;

                    return new PageCollectionInfo<TrainingPeriodEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }

                else return null;
            }
        }

        public bool RemoveTrainingPeriodEntries(List<TrainingPeriodEntryDTO> employeeTrainings, ServiceHeader serviceHeader)
        {
            if (employeeTrainings == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in employeeTrainings)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _trainingPeriodEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _trainingPeriodEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        #endregion
    }
}
