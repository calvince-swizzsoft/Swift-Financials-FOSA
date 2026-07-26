using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ISalesOrderService
    {
        #region SalesOrderDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<SalesOrderDTO> AddSalesOrderAsync(SalesOrderDTO salesOrderDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateSalesOrderAsync(SalesOrderDTO salesOrderDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<SalesOrderDTO>> FindSalesOrdersAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<SalesOrderDTO>> FindSalesOrdersInPageAsync(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<SalesOrderDTO>> FindSalesOrdersByFilterInPageAsync(int itemType, string filter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<SalesOrderDTO> FindSalesOrderAsync(Guid salesOrderId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<SalesOrderDTO>> FindSalesOrderByCodeAsync(string code);

        #endregion

        #region SalesOrderEntryDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<SalesOrderEntryDTO> AddSalesOrderEntryAsync(SalesOrderEntryDTO salesOrderEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<SalesOrderEntryDTO>> FindSalesOrderEntriesBySalesOrderIdAsync(Guid salesOrderId);
        #endregion
    }
}
