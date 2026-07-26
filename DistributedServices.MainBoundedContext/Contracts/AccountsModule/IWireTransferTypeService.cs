using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IWireTransferTypeService
    {
        #region Wire Transfer Type

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        WireTransferTypeDTO AddWireTransferType(WireTransferTypeDTO wireTransferTypeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateWireTransferType(WireTransferTypeDTO wireTransferTypeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<WireTransferTypeDTO> FindWireTransferTypes();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<WireTransferTypeDTO> FindWireTransferTypesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<WireTransferTypeDTO> FindWireTransferTypesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        WireTransferTypeDTO FindWireTransferType(Guid wireTransferTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> FindCommissionsByWireTransferTypeId(Guid wireTransferTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionsByWireTransferTypeId(Guid wireTransferTypeId, List<CommissionDTO> commissions);

        #endregion
    }
}
