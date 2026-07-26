using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DesignationAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TransactionThresholdAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class DesignationAppService : IDesignationAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Designation> _designationRepository;
        private readonly IRepository<TransactionThreshold> _transactionThresholdRepository;

        public DesignationAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Designation> designationRepository,
           IRepository<TransactionThreshold> transactionThresholdRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (designationRepository == null)
                throw new ArgumentNullException(nameof(designationRepository));

            if (transactionThresholdRepository == null)
                throw new ArgumentNullException(nameof(transactionThresholdRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _designationRepository = designationRepository;
            _transactionThresholdRepository = transactionThresholdRepository;
        }

        public DesignationDTO AddNewDesignation(DesignationDTO designationDTO, ServiceHeader serviceHeader)
        {
            if (designationDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var designation = DesignationFactory.CreateDesignation(designationDTO.ParentId, designationDTO.Description, designationDTO.Remarks);

                    designation.CreatedBy = serviceHeader.ApplicationUserName;

                    if (designationDTO.IsLocked)
                        designation.Lock();
                    else designation.UnLock();

                    _designationRepository.Add(designation, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return designation.ProjectedAs<DesignationDTO>();
                }
            }
            else return null;
        }

        public bool UpdateDesignation(DesignationDTO designationDTO, ServiceHeader serviceHeader)
        {
            if (designationDTO == null || designationDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _designationRepository.Get(designationDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = DesignationFactory.CreateDesignation(designationDTO.ParentId, designationDTO.Description, designationDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);


                    if (designationDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _designationRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<DesignationDTO> FindDesignations(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DesignationSpecifications.ParentDesignations();

                ISpecification<Designation> spec = filter;

                var designations = _designationRepository.AllMatching(spec, serviceHeader, c => c.Children);

                if (designations != null && designations.Any())
                {
                    return designations.ProjectedAsCollection<DesignationDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<DesignationDTO> FindDesignations(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DesignationSpecifications.DefaultSpec();

                ISpecification<Designation> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var designationPagedCollection = _designationRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (designationPagedCollection != null)
                {
                    var pageCollection = designationPagedCollection.PageCollection.ProjectedAsCollection<DesignationDTO>();

                    var itemsCount = designationPagedCollection.ItemsCount;

                    return new PageCollectionInfo<DesignationDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<DesignationDTO> FindDesignations(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DesignationSpecifications.DesignationFullText(text);

                ISpecification<Designation> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var designationPagedCollection = _designationRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (designationPagedCollection != null)
                {
                    var pageCollection = designationPagedCollection.PageCollection.ProjectedAsCollection<DesignationDTO>();

                    var itemsCount = designationPagedCollection.ItemsCount;

                    return new PageCollectionInfo<DesignationDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<DesignationDTO> FindDesignations(ServiceHeader serviceHeader, bool updateDepth = false, bool traverseTree = true)
        {
            var designations = FindDesignations(serviceHeader);

            if (designations != null && designations.Any())
            {
                var designationsDTOList = new List<DesignationDTO>();

                Action<DesignationDTO> traverse = null;

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

                    var designationDTO = new DesignationDTO();
                    designationDTO.Id = node.Id;
                    designationDTO.ParentId = node.ParentId;
                    designationDTO.Description = node.Description;
                    designationDTO.Depth = depth;
                    designationDTO.Remarks = node.Remarks;
                    designationDTO.IsLocked = node.IsLocked;
                    designationDTO.CreatedDate = node.CreatedDate;

                    designationsDTOList.Add(designationDTO);

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
                    foreach (var c in designations)
                    {
                        traverse(c);
                    }
                }
                else
                {
                    designationsDTOList = designations;
                }

                return designationsDTOList;
            }
            else return null;
        }

        public DesignationDTO FindDesignation(Guid designationId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var persisted = _designationRepository.Get(designationId, serviceHeader);

                if (persisted != null)
                {
                    return new DesignationDTO
                    {
                        Id = persisted.Id,
                        ParentId = persisted.ParentId,
                        Description = persisted.Description,
                        IsLocked = persisted.IsLocked,
                        CreatedDate = persisted.CreatedDate,
                        Remarks = persisted.Remarks
                    };
                }
                else return null;
            }
        }

        public bool UpdateTransactionThresholdCollection(Guid designationId, List<TransactionThresholdDTO> transactionThresholdCollection, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existingTransactionThresholdCollection = FindTransactionThresholdCollection(designationId, serviceHeader);

                if (existingTransactionThresholdCollection != null && existingTransactionThresholdCollection.Any())
                {
                    var oldSet = from c in existingTransactionThresholdCollection ?? new List<TransactionThresholdDTO> { } select c;

                    var newSet = from c in transactionThresholdCollection ?? new List<TransactionThresholdDTO> { } select c;

                    var commonSet = oldSet.Intersect(newSet, new TransactionThresholdDTOEqualityComparer());

                    var insertSet = newSet.Except(commonSet, new TransactionThresholdDTOEqualityComparer());

                    var deleteSet = oldSet.Except(commonSet, new TransactionThresholdDTOEqualityComparer());

                    foreach (var item in insertSet)
                    {
                        var transactionThreshold = TransactionThresholdFactory.CreateTransactionThreshold(designationId, item.Type, item.Threshold);

                        transactionThreshold.CreatedBy = serviceHeader.ApplicationUserName;

                        _transactionThresholdRepository.Add(transactionThreshold, serviceHeader);
                    }

                    foreach (var item in deleteSet)
                    {
                        var transactionThreshold = _transactionThresholdRepository.Get(item.Id, serviceHeader);

                        if (transactionThreshold != null)
                        {
                            _transactionThresholdRepository.Remove(transactionThreshold, serviceHeader);
                        }
                    }
                }
                else
                {
                    foreach (var item in transactionThresholdCollection)
                    {
                        var transactionThreshold = TransactionThresholdFactory.CreateTransactionThreshold(designationId, item.Type, item.Threshold);

                        transactionThreshold.CreatedBy = serviceHeader.ApplicationUserName;

                        _transactionThresholdRepository.Add(transactionThreshold, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<TransactionThresholdDTO> FindTransactionThresholdCollection(Guid designationId, ServiceHeader serviceHeader)
        {
            if (designationId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = TransactionThresholdSpecifications.TransactionThresholdWithDesignationId(designationId);

                    ISpecification<TransactionThreshold> spec = filter;

                    var transactionThresholdCollection = _transactionThresholdRepository.AllMatching(spec, serviceHeader);

                    if (transactionThresholdCollection != null)
                    {
                        return transactionThresholdCollection.ProjectedAsCollection<TransactionThresholdDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<TransactionThresholdDTO> FindTransactionThresholdCollection(Guid designationId, int transactionThresholdType, ServiceHeader serviceHeader)
        {
            if (designationId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = TransactionThresholdSpecifications.TransactionThresholdWithDesignationIdAndType(designationId, transactionThresholdType);

                    ISpecification<TransactionThreshold> spec = filter;

                    var transactionThresholdCollection = _transactionThresholdRepository.AllMatching(spec, serviceHeader);

                    if (transactionThresholdCollection != null)
                    {
                        return transactionThresholdCollection.ProjectedAsCollection<TransactionThresholdDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool ValidateTransactionThreshold(Guid designationId, int transactionThresholdType, decimal transactionAmount, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var transactionThresholdCollection = FindTransactionThresholdCollection(designationId, transactionThresholdType, serviceHeader);

            if (transactionThresholdCollection != null && transactionThresholdCollection.Any())
            {
                result = transactionThresholdCollection.Any(x => x.Threshold >= transactionAmount);
            }

            return result;
        }
    }
}
