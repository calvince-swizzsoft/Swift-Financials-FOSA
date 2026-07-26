using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderEntryAgg;

using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.UnderwritingModule.Services
{
    public class PurchaseOrderAppService : IPurchaseOrderAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<PurchaseOrder> _policyRepository;
        private readonly IRepository<PurchaseOrderEntry> _purchaseOderEntryRepository;

        public PurchaseOrderAppService(IDbContextScopeFactory dbContextScopeFactory, IRepository<PurchaseOrder> policyRepository, IRepository<PurchaseOrderEntry> purchaseOderEntryRepository)
        {
            _dbContextScopeFactory = dbContextScopeFactory ?? throw new ArgumentNullException(nameof(dbContextScopeFactory));
            _policyRepository = policyRepository ?? throw new ArgumentNullException(nameof(policyRepository));

            _purchaseOderEntryRepository = purchaseOderEntryRepository ?? throw new ArgumentNullException(nameof(purchaseOderEntryRepository));

        }

        public async Task<PurchaseOrderDTO> AddNewPurchaseOrderAsync(PurchaseOrderDTO purchaseOrderDTO, ServiceHeader serviceHeader)
        {
            var purchaseOrderBindingModel = purchaseOrderDTO.ProjectedAs<PurchaseOrderBindingModel>();

            purchaseOrderBindingModel.ValidateAll();

            if (purchaseOrderBindingModel.HasErrors)
                throw new InvalidOperationException(string.Join(Environment.NewLine, purchaseOrderBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                purchaseOrderDTO.RecordStatus = (short)PurchaseOrderStatus.Pending;

                var purchaseOrder = PurchaseOrderFactory.CreatePurchaseOrder(
                    purchaseOrderDTO.InventoryDescription,
                    purchaseOrderDTO.OrderQuantity,
                    purchaseOrderDTO.Amount,
                    purchaseOrderDTO.SupplierName,
                    purchaseOrderDTO.SupplierContact,
                    purchaseOrderDTO.PaymentTerms,
                    purchaseOrderDTO.Remarks,
                    purchaseOrderDTO.RecordStatus


                );

                purchaseOrder.CreatedBy = serviceHeader.ApplicationUserName;

                _policyRepository.Add(purchaseOrder, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ?
                    purchaseOrder.ProjectedAs<PurchaseOrderDTO>() :
                    null;
            }
        }


        public async Task<List<PurchaseOrderDTO>> FindPurchaseOrderAsync(string code, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PurchaseOrderSpecifications.PurchaseOrderWithCode(code);

                return await _policyRepository.AllMatchingAsync<PurchaseOrderDTO>(filter, serviceHeader);
            }
        }

        public async Task<PurchaseOrderDTO> FindPurchaseOrderAsync(Guid policyId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _policyRepository.GetAsync<PurchaseOrderDTO>(policyId, serviceHeader);
            }
        }

        public async Task<List<PurchaseOrderDTO>> FindPurchaseOrdersAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _policyRepository.GetAllAsync<PurchaseOrderDTO>(serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<PurchaseOrderDTO>> FindPurchaseOrderAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PurchaseOrderSpecifications.DefaultSpec();

                ISpecification<PurchaseOrder> spec = filter;

                var sortFields = new List<string> { "ClusterId" };

                return await _policyRepository.AllMatchingPagedAsync<PurchaseOrderDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<PurchaseOrderDTO>> FindPurchaseOrdersAsync(int itemType, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PurchaseOrderSpecifications.PurchaseOrderFullText(text);

                ISpecification<PurchaseOrder> spec = filter;

                var sortFields = new List<string> { "ClusterId" };

                return await _policyRepository.AllMatchingPagedAsync<PurchaseOrderDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<bool> UpdatePurchaseOrderAsync(PurchaseOrderDTO purchaseOrderDTO, ServiceHeader serviceHeader)
        {
            var policyBindingModel = purchaseOrderDTO.ProjectedAs<PurchaseOrderBindingModel>();

            policyBindingModel.ValidateAll();

            if (policyBindingModel.HasErrors)
                throw new InvalidOperationException(string.Join(Environment.NewLine, policyBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _policyRepository.GetAsync(purchaseOrderDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = PurchaseOrderFactory.CreatePurchaseOrder(purchaseOrderDTO.InventoryDescription, purchaseOrderDTO.OrderQuantity, purchaseOrderDTO.Amount, purchaseOrderDTO.SupplierName, purchaseOrderDTO.SupplierContact, purchaseOrderDTO.PaymentTerms, purchaseOrderDTO.Remarks, purchaseOrderDTO.RecordStatus);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _policyRepository.Merge(persisted, current, serviceHeader);
                }
                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }


        public async Task<PurchaseOrderEntryDTO> AddNewPurchaseOrderEntryAsync(PurchaseOrderEntryDTO purchaseOrderEntryDTO, ServiceHeader serviceHeader)
        {
            var purchaseOrderEntryBindingModel = purchaseOrderEntryDTO.ProjectedAs<PurchaseOrderEntryBindingModel>();

            purchaseOrderEntryBindingModel.ValidateAll();

            if (purchaseOrderEntryBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, purchaseOrderEntryBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var purchaseOrderEntry = PurchaseOrderEntryFactory.CreatePurchaseOrderEntry(purchaseOrderEntryDTO.PurchaseOrderId, purchaseOrderEntryDTO.InventoryId, purchaseOrderEntryDTO.Quantity, purchaseOrderEntryDTO.AmountPerUnit);
                purchaseOrderEntry.CreatedBy = serviceHeader.ApplicationUserName;

                _purchaseOderEntryRepository.Add(purchaseOrderEntry, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? purchaseOrderEntry.ProjectedAs<PurchaseOrderEntryDTO>() : null;
            }
        }

        public async Task<List<PurchaseOrderEntryDTO>> FindPurchaseOrderEntriesByPurchaseOrderIdAsync(Guid policyId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _purchaseOderEntryRepository.GetAsync<List<PurchaseOrderEntryDTO>>(policyId, serviceHeader);
            }

        }

       


        //public async Task<bool> RemovePurchaseOrderEntryAsync(PurchaseOrderEntryDTO purchaseOrderEntryDTOs, ServiceHeader serviceHeader)
        //{
        //    var purchaseOrderEntryBindingModel = purchaseOrderEntryDTOs.ProjectedAs<PurchaseOrderEntryBindingModel>();

        //    purchaseOrderEntryBindingModel.ValidateAll();

        //    if (purchaseOrderEntryBindingModel.HasErrors)
        //        throw new InvalidOperationException(string.Join(Environment.NewLine, purchaseOrderEntryBindingModel.ErrorMessages));

        //    using (var dbContextScope = _dbContextScopeFactory.Create())
        //    {

        //        if (purchaseOrderEntryDTOs == null)
        //            return false;

        //        using (var dbContextScope = _dbContextScopeFactory.Create())
        //        {
        //            foreach (var item in purchaseOrderEntryDTOs)
        //            {
        //                if (item.Id != null && item.Id != Guid.Empty)
        //                {
        //                    var persisted = _purchaseOderEntryRepository.Get(item.Id, serviceHeader);

        //                    if (persisted != null)
        //                    {
        //                        _purchaseOderEntryRepository.Remove(persisted, serviceHeader);
        //                    }
        //                }
        //            }

        //            return dbContextScope.SaveChanges(serviceHeader) >= 0;
        //        }
        //    }
        //}
    }
}
