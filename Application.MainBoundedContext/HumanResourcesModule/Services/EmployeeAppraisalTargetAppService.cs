using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalTargetAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class EmployeeAppraisalTargetAppService : IEmployeeAppraisalTargetAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<EmployeeAppraisalTarget> _employeeAppraisalTargetRepository;

        public EmployeeAppraisalTargetAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<EmployeeAppraisalTarget> employeeAppraisalTargetRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employeeAppraisalTargetRepository == null)
                throw new ArgumentNullException(nameof(employeeAppraisalTargetRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _employeeAppraisalTargetRepository = employeeAppraisalTargetRepository;
        }

        public EmployeeAppraisalTargetDTO AddNewEmployeeAppraisalTarget(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO, ServiceHeader serviceHeader)
        {
            if (employeeAppraisalTargetDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var employeeAppraisalTarget = EmployeeAppraisalTargetFactory.CreateEmployeeAppraisalTarget(employeeAppraisalTargetDTO.ParentId, employeeAppraisalTargetDTO.Type, employeeAppraisalTargetDTO.AnswerType);

                    employeeAppraisalTarget.CreatedBy = serviceHeader.ApplicationUserName;

                    if (employeeAppraisalTargetDTO.Type == (int)EmployeeAppraisalTargetType.HeaderEntry)
                        employeeAppraisalTarget.Description = employeeAppraisalTargetDTO.Description.ToUpper();
                    else
                        employeeAppraisalTarget.Description = employeeAppraisalTargetDTO.Description;

                    if (employeeAppraisalTargetDTO.IsLocked)
                        employeeAppraisalTarget.Lock();
                    else employeeAppraisalTarget.UnLock();

                    _employeeAppraisalTargetRepository.Add(employeeAppraisalTarget, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return employeeAppraisalTarget.ProjectedAs<EmployeeAppraisalTargetDTO>();
                }
            }
            else return null;
        }

        public bool UpdateEmployeeAppraisalTarget(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO, ServiceHeader serviceHeader)
        {
            if (employeeAppraisalTargetDTO == null || employeeAppraisalTargetDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employeeAppraisalTargetRepository.Get(employeeAppraisalTargetDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = EmployeeAppraisalTargetFactory.CreateEmployeeAppraisalTarget(employeeAppraisalTargetDTO.ParentId, employeeAppraisalTargetDTO.Type, employeeAppraisalTargetDTO.AnswerType);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (employeeAppraisalTargetDTO.Type == (int)EmployeeAppraisalTargetType.HeaderEntry)
                        current.Description = employeeAppraisalTargetDTO.Description.ToUpper();
                    else
                        current.Description = employeeAppraisalTargetDTO.Description;

                    if (employeeAppraisalTargetDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _employeeAppraisalTargetRepository.Merge(persisted, current, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return true;
                }
                else return false;
            }
        }

        public List<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargets(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeAppraisalTargetSpecifications.ParentEmployeeAppraisalTargets();

                ISpecification<EmployeeAppraisalTarget> spec = filter;

                var employeeAppraisalTargets = _employeeAppraisalTargetRepository.AllMatching(spec, serviceHeader, c => c.Children);

                if (employeeAppraisalTargets != null && employeeAppraisalTargets.Any())
                {
                    return employeeAppraisalTargets.ProjectedAsCollection<EmployeeAppraisalTargetDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargets(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeAppraisalTargetSpecifications.DefaultSpec();

                ISpecification<EmployeeAppraisalTarget> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeAppraisalTargetPagedCollection = _employeeAppraisalTargetRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeAppraisalTargetPagedCollection != null)
                {
                    var pageCollection = employeeAppraisalTargetPagedCollection.PageCollection.ProjectedAsCollection<EmployeeAppraisalTargetDTO>();

                    var itemsCount = employeeAppraisalTargetPagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeAppraisalTargetDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeAppraisalTargetDTO> FindChildEmployeeAppraisalTargets(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeAppraisalTargetSpecifications.ChildrenEmployeeAppraisalTargets();

                ISpecification<EmployeeAppraisalTarget> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeAppraisalTargetPagedCollection = _employeeAppraisalTargetRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeAppraisalTargetPagedCollection != null)
                {
                    var pageCollection = employeeAppraisalTargetPagedCollection.PageCollection.ProjectedAsCollection<EmployeeAppraisalTargetDTO>();

                    var itemsCount = employeeAppraisalTargetPagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeAppraisalTargetDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<EmployeeAppraisalTargetDTO> FindChildEmployeeAppraisalTargets(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeAppraisalTargetSpecifications.ChildrenEmployeeAppraisalTargets();

                ISpecification<EmployeeAppraisalTarget> spec = filter;

                var employeeTargets = _employeeAppraisalTargetRepository.AllMatching(spec, serviceHeader);

                if (employeeTargets != null && employeeTargets.Any())
                {
                    return employeeTargets.ProjectedAsCollection<EmployeeAppraisalTargetDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargets(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeAppraisalTargetSpecifications.EmployeeAppraisalTargetFullText(text);

                ISpecification<EmployeeAppraisalTarget> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeAppraisalTargetCollection = _employeeAppraisalTargetRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeAppraisalTargetCollection != null)
                {
                    var pageCollection = employeeAppraisalTargetCollection.PageCollection.ProjectedAsCollection<EmployeeAppraisalTargetDTO>();

                    var itemsCount = employeeAppraisalTargetCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeAppraisalTargetDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public EmployeeAppraisalTargetDTO FindEmployeeAppraisalTarget(Guid employeeAppraisalTargetId, ServiceHeader serviceHeader)
        {
            if (employeeAppraisalTargetId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var employeeAppraisalTarget = _employeeAppraisalTargetRepository.Get(employeeAppraisalTargetId, serviceHeader);

                    if (employeeAppraisalTarget != null)
                    {
                        return employeeAppraisalTarget.ProjectedAs<EmployeeAppraisalTargetDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargets(ServiceHeader serviceHeader, bool updateDepth = false, bool traverseTree = true)
        {
            var employeeAppraisalTargets = FindEmployeeAppraisalTargets(serviceHeader);

            if (employeeAppraisalTargets != null && employeeAppraisalTargets.Any())
            {
                var employeeAppraisalTargetsDTOList = new List<EmployeeAppraisalTargetDTO>();

                Action<EmployeeAppraisalTargetDTO> traverse = null;

                /* recursive lambda */
                traverse = (node) =>
                {
                    int depth = 0;

                    var tempNode = node;
                    while (tempNode.Parent != null)
                    {
                        tempNode = tempNode.Parent;
                        depth++;
                    }

                    var employeeAppraisalTargetDTO = new EmployeeAppraisalTargetDTO();
                    employeeAppraisalTargetDTO.Id = node.Id;
                    employeeAppraisalTargetDTO.ParentId = node.ParentId;
                    employeeAppraisalTargetDTO.Description = node.Description;
                    employeeAppraisalTargetDTO.Type = node.Type;
                    employeeAppraisalTargetDTO.AnswerType = node.AnswerType;
                    employeeAppraisalTargetDTO.Depth = depth;
                    employeeAppraisalTargetDTO.IsLocked = node.IsLocked;
                    employeeAppraisalTargetDTO.CreatedDate = node.CreatedDate;
                    employeeAppraisalTargetDTO.CreatedBy = node.CreatedBy;

                    employeeAppraisalTargetsDTOList.Add(employeeAppraisalTargetDTO);

                    if (node.Children != null)
                    {
                        foreach (var item in node.Children)
                        {
                            traverse(item);
                        }
                    }
                };

                if (traverseTree)
                {
                    foreach (var c in employeeAppraisalTargets)
                    {
                        traverse(c);
                    }
                }
                else
                {
                    employeeAppraisalTargetsDTOList = employeeAppraisalTargets;
                }

                return employeeAppraisalTargetsDTOList;
            }
            else return null;
        }
    }
}
