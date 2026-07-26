using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.Seedwork;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentEntryAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentPeriodAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public class DataAttachmentPeriodAppService : IDataAttachmentPeriodAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<DataAttachmentPeriod> _dataAttachmentPeriodRepository;
        private readonly IRepository<DataAttachmentEntry> _dataAttachmentEntryRepository;
        private readonly IAppCache _appCache;

        public DataAttachmentPeriodAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<DataAttachmentPeriod> dataAttachmentPeriodRepository,
           IRepository<DataAttachmentEntry> dataAttachmentEntryRepository,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (dataAttachmentPeriodRepository == null)
                throw new ArgumentNullException(nameof(dataAttachmentPeriodRepository));

            if (dataAttachmentEntryRepository == null)
                throw new ArgumentNullException(nameof(dataAttachmentEntryRepository));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _dataAttachmentPeriodRepository = dataAttachmentPeriodRepository;
            _dataAttachmentEntryRepository = dataAttachmentEntryRepository;
            _appCache = appCache;
        }

        public DataAttachmentPeriodDTO AddNewDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, ServiceHeader serviceHeader)
        {
            if (dataAttachmentPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    // get the specification
                    var filter = DataAttachmentPeriodSpecifications.DataAttachmentPeriodWithPostingPeriodIdAndMonth(dataAttachmentPeriodDTO.PostingPeriodId, dataAttachmentPeriodDTO.Month);

                    ISpecification<DataAttachmentPeriod> spec = filter;

                    //Query this criteria
                    var dataAttachmentPeriods = _dataAttachmentPeriodRepository.AllMatching(spec, serviceHeader);

                    if (dataAttachmentPeriods != null && dataAttachmentPeriods.Any(x => x.Status == (int)DataAttachmentPeriodStatus.Closed || x.Status == (int)DataAttachmentPeriodStatus.Open))
                    {
                        dataAttachmentPeriodDTO.ErrorMessageResult = string.Format("Sorry, but there is already an open/closed data period for the selected month!");
                        return dataAttachmentPeriodDTO;
                    }
                    else
                    {
                        var dataAttachmentPeriod = DataAttachmentPeriodFactory.CreateDataAttachmentPeriod(dataAttachmentPeriodDTO.PostingPeriodId, dataAttachmentPeriodDTO.Month, dataAttachmentPeriodDTO.Remarks);

                        dataAttachmentPeriod.Status = (int)DataAttachmentPeriodStatus.Open;
                        dataAttachmentPeriod.CreatedBy = serviceHeader.ApplicationUserName;

                        _dataAttachmentPeriodRepository.Add(dataAttachmentPeriod, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return dataAttachmentPeriod.ProjectedAs<DataAttachmentPeriodDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, ServiceHeader serviceHeader)
        {
            if (dataAttachmentPeriodDTO == null || dataAttachmentPeriodDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _dataAttachmentPeriodRepository.Get(dataAttachmentPeriodDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    // get the specification
                    var filter = DataAttachmentPeriodSpecifications.DataAttachmentPeriodWithPostingPeriodIdAndMonth(dataAttachmentPeriodDTO.PostingPeriodId, dataAttachmentPeriodDTO.Month);

                    ISpecification<DataAttachmentPeriod> spec = filter;

                    //Query this criteria
                    var dataAttachmentPeriods = _dataAttachmentPeriodRepository.AllMatching(spec, serviceHeader);

                    bool isSuccessful = (bool)default;

                    if (dataAttachmentPeriods != null && dataAttachmentPeriods.Any(x => x.Id != persisted.Id && x.Status == (int)DataAttachmentPeriodStatus.Closed || x.Status == (int)DataAttachmentPeriodStatus.Suspended))
                    {
                        dataAttachmentPeriodDTO.ErrorMessageResult = string.Format("Sorry, but there is already a closed/suspended data period for the selected month!");

                        isSuccessful = false;

                        return isSuccessful;
                    }
                    else
                    {
                        var current = DataAttachmentPeriodFactory.CreateDataAttachmentPeriod(persisted.PostingPeriodId, dataAttachmentPeriodDTO.Month, dataAttachmentPeriodDTO.Remarks);

                        current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                        current.Status = persisted.Status;
                        current.CreatedBy = persisted.CreatedBy;


                        _dataAttachmentPeriodRepository.Merge(persisted, current, serviceHeader);

                        // Activate?
                        if (dataAttachmentPeriodDTO.IsActive && !persisted.IsActive)
                            ActivateDataAttachmentPeriod(persisted.Id, serviceHeader);

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
                else return false;
            }
        }

        public bool CloseDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO, ServiceHeader serviceHeader)
        {
            if (dataAttachmentPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _dataAttachmentPeriodRepository.Get(dataAttachmentPeriodDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status == (int)DataAttachmentPeriodStatus.Open)
                    {
                        persisted.Status = (int)DataAttachmentPeriodStatus.Closed;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public DataAttachmentEntryDTO AddNewDataAttachmentEntry(DataAttachmentEntryDTO dataAttachmentEntryDTO, ServiceHeader serviceHeader)
        {
            if (dataAttachmentEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var dataAttachmentEntry = DataAttachmentEntryFactory.CreateDataAttachmentEntry(dataAttachmentEntryDTO.DataAttachmentPeriodId, dataAttachmentEntryDTO.CustomerAccountId, dataAttachmentEntryDTO.TransactionType, dataAttachmentEntryDTO.SequenceNumber, dataAttachmentEntryDTO.NewAmount, dataAttachmentEntryDTO.CurrentAmount, dataAttachmentEntryDTO.NewBalance, dataAttachmentEntryDTO.CurrentBalance, dataAttachmentEntryDTO.NewAbility, dataAttachmentEntryDTO.CurrentAbility, dataAttachmentEntryDTO.Remarks);

                    dataAttachmentEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _dataAttachmentEntryRepository.Add(dataAttachmentEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return dataAttachmentEntry.ProjectedAs<DataAttachmentEntryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveDataAttachmentEntries(List<DataAttachmentEntryDTO> dataAttachmentEntryDTOs, ServiceHeader serviceHeader)
        {
            if (dataAttachmentEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in dataAttachmentEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _dataAttachmentEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _dataAttachmentEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<DataAttachmentPeriodDTO> FindDataAttachmentPeriods(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var dataAttachmentPeriods = _dataAttachmentPeriodRepository.GetAll(serviceHeader);

                if (dataAttachmentPeriods != null && dataAttachmentPeriods.Any())
                {
                    return dataAttachmentPeriods.ProjectedAsCollection<DataAttachmentPeriodDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<DataAttachmentPeriodDTO> FindDataAttachmentPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DataAttachmentPeriodSpecifications.DefaultSpec();

                ISpecification<DataAttachmentPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var dataAttachmentPeriodPagedCollection = _dataAttachmentPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (dataAttachmentPeriodPagedCollection != null)
                {
                    var pageCollection = dataAttachmentPeriodPagedCollection.PageCollection.ProjectedAsCollection<DataAttachmentPeriodDTO>();

                    var itemsCount = dataAttachmentPeriodPagedCollection.ItemsCount;

                    return new PageCollectionInfo<DataAttachmentPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<DataAttachmentPeriodDTO> FindDataAttachmentPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DataAttachmentPeriodSpecifications.DataAttachmentPeriodFullText(text);

                ISpecification<DataAttachmentPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var dataAttachmentPeriodCollection = _dataAttachmentPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (dataAttachmentPeriodCollection != null)
                {
                    var pageCollection = dataAttachmentPeriodCollection.PageCollection.ProjectedAsCollection<DataAttachmentPeriodDTO>();

                    var itemsCount = dataAttachmentPeriodCollection.ItemsCount;

                    return new PageCollectionInfo<DataAttachmentPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<DataAttachmentPeriodDTO> FindDataAttachmentPeriods(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DataAttachmentPeriodSpecifications.DataAttachmentPeriodFullText(status, startDate, endDate, text);

                ISpecification<DataAttachmentPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var dataAttachmentPeriodPagedCollection = _dataAttachmentPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (dataAttachmentPeriodPagedCollection != null)
                {
                    var pageCollection = dataAttachmentPeriodPagedCollection.PageCollection.ProjectedAsCollection<DataAttachmentPeriodDTO>();

                    var itemsCount = dataAttachmentPeriodPagedCollection.ItemsCount;

                    return new PageCollectionInfo<DataAttachmentPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public DataAttachmentPeriodDTO FindDataAttachmentPeriod(Guid dataAttachmentPeriodId, ServiceHeader serviceHeader)
        {
            if (dataAttachmentPeriodId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var dataAttachmentPeriod = _dataAttachmentPeriodRepository.Get(dataAttachmentPeriodId, serviceHeader);

                    if (dataAttachmentPeriod != null)
                    {
                        return dataAttachmentPeriod.ProjectedAs<DataAttachmentPeriodDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<DataAttachmentEntryDTO> FindDataAttachmentEntriesByDataAttachmentPeriodIdAndCustomerAccountId(Guid dataAttachmentPeriodId, Guid customerAccountId, ServiceHeader serviceHeader)
        {
            if (dataAttachmentPeriodId != null && dataAttachmentPeriodId != Guid.Empty && customerAccountId != null && customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    // get the specification
                    var filter = DataAttachmentEntrySpecifications.DataAttachmentEntryWithDataAttachmentPeriodIdAndCustomerAccountId(dataAttachmentPeriodId, customerAccountId);

                    ISpecification<DataAttachmentEntry> spec = filter;

                    //Query this criteria
                    var dataAttachmentEntries = _dataAttachmentEntryRepository.AllMatching(spec, serviceHeader);

                    if (dataAttachmentEntries != null && dataAttachmentEntries.Any())
                    {
                        return dataAttachmentEntries.ProjectedAsCollection<DataAttachmentEntryDTO>();
                    }
                    else // no results
                        return null;
                }
            }
            else // no results 
                return null;
        }

        public PageCollectionInfo<DataAttachmentEntryDTO> FindDataAttachmentEntriesByDataAttachmentPeriodId(Guid dataAttachmentPeriodId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (dataAttachmentPeriodId != null && dataAttachmentPeriodId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = DataAttachmentEntrySpecifications.DataAttachmentEntryFullText(dataAttachmentPeriodId, text);

                    ISpecification<DataAttachmentEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var dataAttachmentEntryPagedCollection = _dataAttachmentEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (dataAttachmentEntryPagedCollection != null)
                    {
                        var pageCollection = dataAttachmentEntryPagedCollection.PageCollection.ProjectedAsCollection<DataAttachmentEntryDTO>();

                        var itemsCount = dataAttachmentEntryPagedCollection.ItemsCount;

                        return new PageCollectionInfo<DataAttachmentEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public DataAttachmentPeriodDTO FindCurrentDataAttachmentPeriod(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DataAttachmentPeriodSpecifications.CurrentDataAttachmentPeriod();

                ISpecification<DataAttachmentPeriod> spec = filter;

                var dataAttachmentPeriods = _dataAttachmentPeriodRepository.AllMatching(spec, serviceHeader);

                if (dataAttachmentPeriods != null && dataAttachmentPeriods.Any() && dataAttachmentPeriods.Count() == 1)
                {
                    var dataAttachmentPeriod = dataAttachmentPeriods.SingleOrDefault();

                    if (dataAttachmentPeriod != null)
                    {
                        return dataAttachmentPeriod.ProjectedAs<DataAttachmentPeriodDTO>();
                    }
                    else return null;
                }
                else return null;
            }
        }

        public DataAttachmentPeriodDTO FindCachedCurrentDataAttachmentPeriod(ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<DataAttachmentPeriodDTO>(string.Format("CurrentDataAttachmentPeriod_{0}", serviceHeader.ApplicationDomainName), () =>
            {
                return FindCurrentDataAttachmentPeriod(serviceHeader);
            });
        }

        private bool ActivateDataAttachmentPeriod(Guid dataAttachmentPeriodId, ServiceHeader serviceHeader)
        {
            if (dataAttachmentPeriodId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _dataAttachmentPeriodRepository.Get(dataAttachmentPeriodId, serviceHeader);

                if (persisted != null)
                {
                    persisted.Activate();

                    var otherPeriods = _dataAttachmentPeriodRepository.GetAll(serviceHeader);

                    foreach (var item in otherPeriods)
                    {
                        if (item.Id != persisted.Id)
                        {
                            var dataAttachmentPeriod = _dataAttachmentPeriodRepository.Get(item.Id, serviceHeader);

                            dataAttachmentPeriod.DeActivate();
                        }
                    }

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }
    }
}
