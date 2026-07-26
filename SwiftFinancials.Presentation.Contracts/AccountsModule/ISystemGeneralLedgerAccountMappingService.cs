using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{

    [ServiceContract(Name = "ISystemGeneralLedgerAccountMappingService")]
    public interface ISystemGeneralLedgerAccountMappingService
    {
        #region System General Ledger Account Mapping

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSystemGeneralLedgerAccountMapping(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO, AsyncCallback callback, Object state);
        SystemGeneralLedgerAccountMappingDTO EndAddSystemGeneralLedgerAccountMapping(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateSystemGeneralLedgerAccountMapping(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO, AsyncCallback callback, Object state);
        bool EndUpdateSystemGeneralLedgerAccountMapping(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSystemGeneralLedgerAccountMappings(AsyncCallback callback, Object state);
        List<SystemGeneralLedgerAccountMappingDTO> EndFindSystemGeneralLedgerAccountMappings(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSystemGeneralLedgerAccountMapping(Guid systemGeneralLedgerAccountMappingId, AsyncCallback callback, Object state);
        SystemGeneralLedgerAccountMappingDTO EndFindSystemGeneralLedgerAccountMapping(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSystemGeneralLedgerAccountMappingsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> EndFindSystemGeneralLedgerAccountMappingsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSystemGeneralLedgerAccountMappingsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> EndFindSystemGeneralLedgerAccountMappingsByFilterInPage(IAsyncResult result);

        #endregion
    }
}
