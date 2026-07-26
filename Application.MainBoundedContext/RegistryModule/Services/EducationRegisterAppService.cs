using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EducationAttendeeAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EducationRegisterAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class EducationRegisterAppService : IEducationRegisterAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<EducationRegister> _educationRegisterRepository;
        private readonly IRepository<EducationAttendee> _educationAttendeeRepository;

        public EducationRegisterAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<EducationRegister> educationRegisterRepository,
           IRepository<EducationAttendee> educationAttendeeRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (educationRegisterRepository == null)
                throw new ArgumentNullException(nameof(educationRegisterRepository));

            if (educationAttendeeRepository == null)
                throw new ArgumentNullException(nameof(educationAttendeeRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _educationRegisterRepository = educationRegisterRepository;
            _educationAttendeeRepository = educationAttendeeRepository;
        }

        public EducationRegisterDTO AddNewEducationRegister(EducationRegisterDTO educationRegisterDTO, ServiceHeader serviceHeader)
        {
            if (educationRegisterDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var duration = new Duration(educationRegisterDTO.DurationStartDate, educationRegisterDTO.DurationEndDate);

                    var educationRegister = EducationRegisterFactory.CreateEducationRegister(educationRegisterDTO.PostingPeriodId, educationRegisterDTO.EducationVenueId, educationRegisterDTO.Description, duration, educationRegisterDTO.Remarks);

                    educationRegister.CreatedBy = serviceHeader.ApplicationUserName;

                    _educationRegisterRepository.Add(educationRegister, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return educationRegister.ProjectedAs<EducationRegisterDTO>();
                }
            }
            else return null;
        }

        public bool UpdateEducationRegister(EducationRegisterDTO educationRegisterDTO, ServiceHeader serviceHeader)
        {
            if (educationRegisterDTO == null || educationRegisterDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _educationRegisterRepository.Get(educationRegisterDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var duration = new Duration(educationRegisterDTO.DurationStartDate, educationRegisterDTO.DurationEndDate);

                    var current = EducationRegisterFactory.CreateEducationRegister(educationRegisterDTO.PostingPeriodId, educationRegisterDTO.EducationVenueId, educationRegisterDTO.Description, duration, educationRegisterDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;
                    

                    _educationRegisterRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public EducationAttendeeDTO AddNewEducationAttendee(EducationAttendeeDTO educationAttendeeDTO, ServiceHeader serviceHeader)
        {
            if (educationAttendeeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var similarMembers = _educationAttendeeRepository.AllMatchingCount(EducationAttendeeSpecifications.EducationAttendeeWithCustomerId(educationAttendeeDTO.CustomerId, educationAttendeeDTO.EducationRegisterPostingPeriodId), serviceHeader);

                    if (similarMembers > 0)
                        throw new InvalidOperationException("Sorry, but the selected customer is already linked to an attendance register for the specified posting period!");

                    var educationAttendee = EducationAttendeeFactory.CreateEducationAttendee(educationAttendeeDTO.EducationRegisterId, educationAttendeeDTO.CustomerId, educationAttendeeDTO.Remarks);

                    educationAttendee.CreatedBy = serviceHeader.ApplicationUserName;

                    _educationAttendeeRepository.Add(educationAttendee, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return educationAttendee.ProjectedAs<EducationAttendeeDTO>();
                }
            }
            else return null;
        }

        public bool RemoveEducationAttendees(List<EducationAttendeeDTO> educationAttendeeDTOs, ServiceHeader serviceHeader)
        {
            if (educationAttendeeDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in educationAttendeeDTOs)
                {
                    var persisted = _educationAttendeeRepository.Get(item.Id, serviceHeader);

                    if (persisted != null)
                    {
                        _educationAttendeeRepository.Remove(persisted, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool UpdateEducationAttendeeCollection(Guid educationRegisterId, List<EducationAttendeeDTO> educationAttendeeCollection, ServiceHeader serviceHeader)
        {
            if (educationRegisterId != null && educationAttendeeCollection != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _educationRegisterRepository.Get(educationRegisterId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindEducationAttendees(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var educationAttendee = _educationAttendeeRepository.Get(item.Id, serviceHeader);

                                if (educationAttendee != null)
                                {
                                    _educationAttendeeRepository.Remove(educationAttendee, serviceHeader);
                                }
                            }
                        }

                        if (educationAttendeeCollection.Any())
                        {
                            foreach (var item in educationAttendeeCollection)
                            {
                                var educationAttendee = EducationAttendeeFactory.CreateEducationAttendee(persisted.Id, item.CustomerId, item.Remarks);

                                educationAttendee.CreatedBy = serviceHeader.ApplicationUserName;

                                _educationAttendeeRepository.Add(educationAttendee, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<EducationRegisterDTO> FindEducationRegisters(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var educationRegisters = _educationRegisterRepository.GetAll(serviceHeader);

                if (educationRegisters != null && educationRegisters.Any())
                {
                    return educationRegisters.ProjectedAsCollection<EducationRegisterDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<EducationRegisterDTO> FindEducationRegisters(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EducationRegisterSpecifications.EducationRegisterFullText(text);

                ISpecification<EducationRegister> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var educationRegisterPagedCollection = _educationRegisterRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (educationRegisterPagedCollection != null)
                {
                    var pageCollection = educationRegisterPagedCollection.PageCollection.ProjectedAsCollection<EducationRegisterDTO>();

                    var itemsCount = educationRegisterPagedCollection.ItemsCount;

                    return new PageCollectionInfo<EducationRegisterDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public EducationRegisterDTO FindEducationRegister(Guid educationRegisterId, ServiceHeader serviceHeader)
        {
            if (educationRegisterId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var educationRegister = _educationRegisterRepository.Get(educationRegisterId, serviceHeader);

                    if (educationRegister != null)
                    {
                        return educationRegister.ProjectedAs<EducationRegisterDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<EducationAttendeeDTO> FindEducationAttendees(Guid educationRegisterId, ServiceHeader serviceHeader)
        {
            if (educationRegisterId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = EducationAttendeeSpecifications.EducationAttendeeWithEducationRegisterId(educationRegisterId);

                    ISpecification<EducationAttendee> spec = filter;

                    var educationAttendees = _educationAttendeeRepository.AllMatching(spec, serviceHeader);

                    if (educationAttendees != null)
                    {
                        return educationAttendees.ProjectedAsCollection<EducationAttendeeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<EducationAttendeeDTO> FindEducationAttendees(Guid educationRegisterId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (educationRegisterId != null && educationRegisterId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = EducationAttendeeSpecifications.EducationAttendeeFullText(educationRegisterId, text);

                    ISpecification<EducationAttendee> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var educationAttendeePagedCollection = _educationAttendeeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (educationAttendeePagedCollection != null)
                    {
                        var pageCollection = educationAttendeePagedCollection.PageCollection.ProjectedAsCollection<EducationAttendeeDTO>();

                        var itemsCount = educationAttendeePagedCollection.ItemsCount;

                        return new PageCollectionInfo<EducationAttendeeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
