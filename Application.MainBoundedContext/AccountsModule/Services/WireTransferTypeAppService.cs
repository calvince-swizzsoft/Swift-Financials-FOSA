using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeCommissionAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class WireTransferTypeAppService : IWireTransferTypeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<WireTransferType> _wireTransferTypeRepository;
        private readonly IRepository<WireTransferTypeCommission> _wireTransferTypeCommissionRepository;

        public WireTransferTypeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<WireTransferType> wireTransferTypeRepository,
           IRepository<WireTransferTypeCommission> wireTransferTypeCommissionRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (wireTransferTypeRepository == null)
                throw new ArgumentNullException(nameof(wireTransferTypeRepository));

            if (wireTransferTypeCommissionRepository == null)
                throw new ArgumentNullException(nameof(wireTransferTypeCommissionRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _wireTransferTypeRepository = wireTransferTypeRepository;
            _wireTransferTypeCommissionRepository = wireTransferTypeCommissionRepository;
        }

        public WireTransferTypeDTO AddNewWireTransferType(WireTransferTypeDTO wireTransferTypeDTO, ServiceHeader serviceHeader)
        {
            if (wireTransferTypeDTO != null && wireTransferTypeDTO.ChartOfAccountId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    ISpecification<WireTransferType> spec = WireTransferTypeSpecifications.WireTransferTypeDescription(wireTransferTypeDTO.Description);

                    var matchedunPayReason = _wireTransferTypeRepository.AllMatching(spec, serviceHeader);

                    if (matchedunPayReason != null && matchedunPayReason.Any())
                    {
                        //throw new InvalidOperationException(string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode));
                        wireTransferTypeDTO.ErrorMessageResult = string.Format("Sorry, but Wire Transfer Type \"{0}\" already exists!", wireTransferTypeDTO.Description.ToUpper());
                        return wireTransferTypeDTO;
                    }
                    else
                    {
                        var wireTransferType = WireTransferTypeFactory.CreateWireTransferType(wireTransferTypeDTO.ChartOfAccountId, wireTransferTypeDTO.Description, wireTransferTypeDTO.TransactionOwnership);

                        if (wireTransferTypeDTO.IsLocked)
                            wireTransferType.Lock();
                        else wireTransferType.UnLock();

                        _wireTransferTypeRepository.Add(wireTransferType, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return wireTransferType.ProjectedAs<WireTransferTypeDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateWireTransferType(WireTransferTypeDTO wireTransferTypeDTO, ServiceHeader serviceHeader)
        {
            if (wireTransferTypeDTO == null || wireTransferTypeDTO.Id == Guid.Empty || wireTransferTypeDTO.ChartOfAccountId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _wireTransferTypeRepository.Get(wireTransferTypeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = WireTransferTypeFactory.CreateWireTransferType(wireTransferTypeDTO.ChartOfAccountId, wireTransferTypeDTO.Description, wireTransferTypeDTO.TransactionOwnership);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);


                    if (wireTransferTypeDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _wireTransferTypeRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<WireTransferTypeDTO> FindWireTransferTypes(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<WireTransferType> spec = WireTransferTypeSpecifications.DefaultSpec();

                var wireTransferTypes = _wireTransferTypeRepository.AllMatching(spec, serviceHeader);

                if (wireTransferTypes != null && wireTransferTypes.Any())
                {
                    return wireTransferTypes.ProjectedAsCollection<WireTransferTypeDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<WireTransferTypeDTO> FindWireTransferTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = WireTransferTypeSpecifications.DefaultSpec();

                ISpecification<WireTransferType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var wireTransferTypeCollection = _wireTransferTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (wireTransferTypeCollection != null)
                {
                    var pageCollection = wireTransferTypeCollection.PageCollection.ProjectedAsCollection<WireTransferTypeDTO>();

                    var itemsCount = wireTransferTypeCollection.ItemsCount;

                    return new PageCollectionInfo<WireTransferTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<WireTransferTypeDTO> FindWireTransferTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = WireTransferTypeSpecifications.WireTransferTypeFullText(text);

                ISpecification<WireTransferType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var wireTransferTypeCollection = _wireTransferTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (wireTransferTypeCollection != null)
                {
                    var pageCollection = wireTransferTypeCollection.PageCollection.ProjectedAsCollection<WireTransferTypeDTO>();

                    var itemsCount = wireTransferTypeCollection.ItemsCount;

                    return new PageCollectionInfo<WireTransferTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public WireTransferTypeDTO FindWireTransferType(Guid wireTransferTypeId, ServiceHeader serviceHeader)
        {
            if (wireTransferTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var wireTransferType = _wireTransferTypeRepository.Get(wireTransferTypeId, serviceHeader);

                    if (wireTransferType != null)
                    {
                        return wireTransferType.ProjectedAs<WireTransferTypeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CommissionDTO> FindCommissions(Guid wireTransferTypeId, ServiceHeader serviceHeader)
        {
            if (wireTransferTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = WireTransferTypeCommissionSpecifications.WireTransferTypeCommissionWithWireTransferTypeId(wireTransferTypeId);

                    ISpecification<WireTransferTypeCommission> spec = filter;

                    var wireTransferTypeCommissions = _wireTransferTypeCommissionRepository.AllMatching(spec, serviceHeader);

                    if (wireTransferTypeCommissions != null)
                    {
                        var projection = wireTransferTypeCommissions.ProjectedAsCollection<WireTransferTypeCommissionDTO>();

                        return (from p in projection select p.Commission).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateCommissions(Guid wireTransferTypeId, List<CommissionDTO> commissionDTOs, ServiceHeader serviceHeader)
        {
            if (wireTransferTypeId != null && commissionDTOs != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _wireTransferTypeRepository.Get(wireTransferTypeId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = WireTransferTypeCommissionSpecifications.WireTransferTypeCommissionWithWireTransferTypeId(wireTransferTypeId);

                        ISpecification<WireTransferTypeCommission> spec = filter;

                        var wireTransferTypeCommissions = _wireTransferTypeCommissionRepository.AllMatching(spec, serviceHeader);

                        if (wireTransferTypeCommissions != null)
                        {
                            wireTransferTypeCommissions.ToList().ForEach(x => _wireTransferTypeCommissionRepository.Remove(x, serviceHeader));
                        }

                        if (commissionDTOs.Any())
                        {
                            foreach (var item in commissionDTOs)
                            {
                                var wireTransferTypeCommission = WireTransferTypeCommissionFactory.CreateWireTransferTypeCommission(persisted.Id, item.Id);
                                wireTransferTypeCommission.CreatedBy = serviceHeader.ApplicationUserName;

                                _wireTransferTypeCommissionRepository.Add(wireTransferTypeCommission, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }
    }
}
