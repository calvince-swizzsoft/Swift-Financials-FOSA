using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IWireTransferTypeService")]
    public interface IWireTransferTypeService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddWireTransferType(WireTransferTypeDTO wireTransferTypeDTO, AsyncCallback callback, Object state);
        WireTransferTypeDTO EndAddWireTransferType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateWireTransferType(WireTransferTypeDTO wireTransferTypeDTO, AsyncCallback callback, Object state);
        bool EndUpdateWireTransferType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWireTransferTypes(AsyncCallback callback, Object state);
        List<WireTransferTypeDTO> EndFindWireTransferTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWireTransferTypesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<WireTransferTypeDTO> EndFindWireTransferTypesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWireTransferTypesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<WireTransferTypeDTO> EndFindWireTransferTypesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWireTransferType(Guid wireTransferTypeId, AsyncCallback callback, Object state);
        WireTransferTypeDTO EndFindWireTransferType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsByWireTransferTypeId(Guid wireTransferTypeId, AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissionsByWireTransferTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionsByWireTransferTypeId(Guid wireTransferTypeId, List<CommissionDTO> commissions, AsyncCallback callback, Object state);
        bool EndUpdateCommissionsByWireTransferTypeId(IAsyncResult result);
    }
}
