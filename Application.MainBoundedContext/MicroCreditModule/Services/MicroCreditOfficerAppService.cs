using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using Application.Seedwork;
using Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditOfficerAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.MicroCreditModule.Services
{
    public class MicroCreditOfficerAppService : IMicroCreditOfficerAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<MicroCreditOfficer> _microCreditOfficerRepository;

        public MicroCreditOfficerAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<MicroCreditOfficer> microCreditOfficerRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (microCreditOfficerRepository == null)
                throw new ArgumentNullException(nameof(microCreditOfficerRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _microCreditOfficerRepository = microCreditOfficerRepository;
        }

        public MicroCreditOfficerDTO AddNewMicroCreditOfficer(MicroCreditOfficerDTO microCreditOfficerDTO, ServiceHeader serviceHeader)
        {
            if (microCreditOfficerDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    // get the specification
                    var filter = MicroCreditOfficerSpecifications.MicroCreditOfficerWithEmployeeId(microCreditOfficerDTO.EmployeeId);

                    ISpecification<MicroCreditOfficer> spec = filter;

                    //Query this criteria
                    var microCreditOfficers = _microCreditOfficerRepository.AllMatching(spec, serviceHeader);

                    if (microCreditOfficers != null && microCreditOfficers.Any())
                    {
                        microCreditOfficerDTO.errormassage = string.Format(("Sorry, but the selected employee already exists as a microcredit officer!"));
                        return microCreditOfficerDTO;

                    }
                       


                    else
                    {
                        var microCreditOfficer = MicroCreditOfficerFactory.CreateMicroCreditOfficer(microCreditOfficerDTO.EmployeeId, microCreditOfficerDTO.Remarks);

                        microCreditOfficer.CreatedBy = serviceHeader.ApplicationUserName;

                        if (microCreditOfficerDTO.IsLocked)
                            microCreditOfficer.Lock();
                        else microCreditOfficer.UnLock();

                        _microCreditOfficerRepository.Add(microCreditOfficer, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return microCreditOfficer.ProjectedAs<MicroCreditOfficerDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateMicroCreditOfficer(MicroCreditOfficerDTO microCreditOfficerDTO, ServiceHeader serviceHeader)
        {
            if (microCreditOfficerDTO == null || microCreditOfficerDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _microCreditOfficerRepository.Get(microCreditOfficerDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = MicroCreditOfficerFactory.CreateMicroCreditOfficer(persisted.EmployeeId, microCreditOfficerDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;
                    
                    if (microCreditOfficerDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _microCreditOfficerRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<MicroCreditOfficerDTO> FindMicroCreditOfficers(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var microCreditOfficers = _microCreditOfficerRepository.GetAll(serviceHeader);

                if (microCreditOfficers != null && microCreditOfficers.Any())
                {
                    return microCreditOfficers.ProjectedAsCollection<MicroCreditOfficerDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<MicroCreditOfficerDTO> FindMicroCreditOfficers(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = MicroCreditOfficerSpecifications.DefaultSpec();

                ISpecification<MicroCreditOfficer> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var microCreditOfficerPagedCollection = _microCreditOfficerRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (microCreditOfficerPagedCollection != null)
                {
                    var pageCollection = microCreditOfficerPagedCollection.PageCollection.ProjectedAsCollection<MicroCreditOfficerDTO>();

                    var itemsCount = microCreditOfficerPagedCollection.ItemsCount;

                    return new PageCollectionInfo<MicroCreditOfficerDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<MicroCreditOfficerDTO> FindMicroCreditOfficers(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = MicroCreditOfficerSpecifications.MicroCreditOfficerFullText(text);

                ISpecification<MicroCreditOfficer> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var microCreditOfficerPagedCollection = _microCreditOfficerRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (microCreditOfficerPagedCollection != null)
                {
                    var pageCollection = microCreditOfficerPagedCollection.PageCollection.ProjectedAsCollection<MicroCreditOfficerDTO>();

                    var itemsCount = microCreditOfficerPagedCollection.ItemsCount;

                    return new PageCollectionInfo<MicroCreditOfficerDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public MicroCreditOfficerDTO FindMicroCreditOfficer(Guid microCreditOfficerId, ServiceHeader serviceHeader)
        {
            if (microCreditOfficerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var microCreditOfficer = _microCreditOfficerRepository.Get(microCreditOfficerId, serviceHeader);

                    if (microCreditOfficer != null)
                    {
                        return microCreditOfficer.ProjectedAs<MicroCreditOfficerDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
