using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ISystemGeneralLedgerAccountMappingService
    {
        #region System General Ledger Account Mapping

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SystemGeneralLedgerAccountMappingDTO AddSystemGeneralLedgerAccountMapping(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateSystemGeneralLedgerAccountMapping(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SystemGeneralLedgerAccountMappingDTO FindSystemGeneralLedgerAccountMapping(Guid systemGeneralLedgerAccountMappingId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappingsInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappingsByFilterInPage(string text, int pageIndex, int pageSize);

        #endregion
    }
}

