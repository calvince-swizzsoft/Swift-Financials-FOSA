using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Application.MainBoundedContext.UnderwritingModule.Services;
using Domain.MainBoundedContext.InventoryModule.Aggregates.ReceiptAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.SalesOrderEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.CustomerModule.Services
{
    public class SalesOrderAppService : ISalesOrderAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<SalesOrder> _salesOrderRepository;
        private readonly IRepository<SalesOrderEntry> _salesOrderEntryRepository;

        public SalesOrderAppService(IDbContextScopeFactory dbContextScopeFactory, IRepository<SalesOrder> salesOrderRepository, IRepository<SalesOrderEntry> salesOrderEntryRepository)
        {
            _dbContextScopeFactory = dbContextScopeFactory ?? throw new ArgumentNullException(nameof(dbContextScopeFactory));
            _salesOrderRepository = salesOrderRepository ?? throw new ArgumentNullException(nameof(salesOrderRepository));
            _salesOrderEntryRepository = salesOrderEntryRepository ?? throw new ArgumentNullException(nameof(salesOrderEntryRepository));
        }

        public async Task<SalesOrderDTO> AddNewSalesOrderAsync(SalesOrderDTO salesOrderDTO, ServiceHeader serviceHeader)
        {
            var salesOrderBindingModel = salesOrderDTO.ProjectedAs<SalesOrderBindingModel>();

            salesOrderBindingModel.ValidateAll();

            if (salesOrderBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, salesOrderBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {

                var salesOrder = SalesOrderFactory.CreateReceipt(salesOrderDTO.OrderQuantity, salesOrderDTO.CustomerName, salesOrderDTO.CustomerEmail, salesOrderDTO.CustomerContact, salesOrderDTO.Remarks, salesOrderDTO.InventoryDescription, salesOrderDTO.PaymentTerms, salesOrderDTO.RecordStatus);
                salesOrder.CreatedBy = serviceHeader.ApplicationUserName;

                _salesOrderRepository.Add(salesOrder, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? salesOrder.ProjectedAs<SalesOrderDTO>() : null;
            }
        }

        public async Task<List<SalesOrderDTO>> FindSalesOrderAsync(string code, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SalesOrderSpecifications.SalesOrderWithCode(code);

                return await _salesOrderRepository.AllMatchingAsync<SalesOrderDTO>(filter, serviceHeader);
            }
        }

        public async Task<SalesOrderDTO> FindSalesOrderAsync(Guid salesOrderId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _salesOrderRepository.GetAsync<SalesOrderDTO>(salesOrderId, serviceHeader);
            }
        }

        public async Task<List<SalesOrderDTO>> FindSalesOrdersAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _salesOrderRepository.GetAllAsync<SalesOrderDTO>(serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<SalesOrderDTO>> FindSalesOrdersAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SalesOrderSpecifications.DefaultSpec();

                ISpecification<SalesOrder> spec = filter;

                var sortFields = new List<string> { "ClusterId" };

                return await _salesOrderRepository.AllMatchingPagedAsync<SalesOrderDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<SalesOrderDTO>> FindSalesOrdersAsync(int itemType, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SalesOrderSpecifications.SalesOrderFullText(text);

                ISpecification<SalesOrder> spec = filter;

                var sortFields = new List<string> { "ClusterId" };

                return await _salesOrderRepository.AllMatchingPagedAsync<SalesOrderDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<bool> UpdateSalesOrderAsync(SalesOrderDTO salesOrderDTO, ServiceHeader serviceHeader)
        {
            var salesOrderBindingModel = salesOrderDTO.ProjectedAs<SalesOrderBindingModel>();

            salesOrderBindingModel.ValidateAll();

            if (salesOrderBindingModel.HasErrors)
                throw new InvalidOperationException(string.Join(Environment.NewLine, salesOrderBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _salesOrderRepository.GetAsync(salesOrderDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = SalesOrderFactory.CreateReceipt(salesOrderDTO.OrderQuantity, salesOrderDTO.CustomerName, salesOrderDTO.CustomerEmail, salesOrderDTO.CustomerContact, salesOrderDTO.Remarks, salesOrderDTO.InventoryDescription, salesOrderDTO.PaymentTerms, salesOrderDTO.RecordStatus);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _salesOrderRepository.Merge(persisted, current, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<SalesOrderEntryDTO> AddNewSalesOrderEntryAsync(SalesOrderEntryDTO salesOrderEntryDTO, ServiceHeader serviceHeader)
        {
            var salesOrderEntryBindingModel = salesOrderEntryDTO.ProjectedAs<SalesOrderEntryBindingModel>();

            salesOrderEntryBindingModel.ValidateAll();

            if (salesOrderEntryBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, salesOrderEntryBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {

                var salesOrderEntry = SalesOrderEntryFactory.CreateSalesOrderEntry(salesOrderEntryDTO.SalesOrderId, salesOrderEntryDTO.InventoryId, salesOrderEntryDTO.Quantity, salesOrderEntryDTO.UnitPrice);

                salesOrderEntry.CreatedBy = serviceHeader.ApplicationUserName;

                _salesOrderEntryRepository.Add(salesOrderEntry, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? salesOrderEntry.ProjectedAs<SalesOrderEntryDTO>() : null;
            }
        }

        public async Task<List<SalesOrderEntryDTO>> FindSalesOrderEntriesBySalesOrderIdAsync(Guid salesOrderId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _salesOrderEntryRepository.GetAsync<List<SalesOrderEntryDTO>>(salesOrderId, serviceHeader);
            }

        }

    }
}

