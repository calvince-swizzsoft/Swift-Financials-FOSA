using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ICashTransferRequestService
    {
        #region Cash Transfer Request

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<CashTransferRequestDTO> AddCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> AcknowledgeCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO, int cashTransferRequestAcknowledgeOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CashTransferRequestDTO>> FindCashTransferRequestsAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<CashTransferRequestDTO>> FindCashTransferRequestsByFilterInPageAsync(Guid employeeId, DateTime startDate, DateTime endDate, int status, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<CashTransferRequestDTO>> FindCashTransferRequestsByStatusAndFilterInPageAsync(string text, DateTime startDate, DateTime endDate, int status, int customerFilter, int pageIndex, int pageSize);


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<CashTransferRequestDTO> FindCashTransferRequestAsync(Guid cashTransferRequestId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CashTransferRequestDTO>> FindMatureCashTransferRequestsByEmployeeIdAsync(Guid employeeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UtilizeCashTransferRequestAsync(Guid cashTransferRequestId);

        #endregion
    }
}
