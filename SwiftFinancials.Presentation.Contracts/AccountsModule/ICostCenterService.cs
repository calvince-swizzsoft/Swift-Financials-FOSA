using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "ICostCenterService")]
    public interface ICostCenterService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCostCenter(CostCenterDTO costCenterDTO, AsyncCallback callback, Object state);
        CostCenterDTO EndAddCostCenter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCostCenter(CostCenterDTO costCenterDTO, AsyncCallback callback, Object state);
        bool EndUpdateCostCenter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCostCenters(AsyncCallback callback, Object state);
        List<CostCenterDTO> EndFindCostCenters(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCostCenter(Guid costCenterId, AsyncCallback callback, Object state);
        CostCenterDTO EndFindCostCenter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCostCentersInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CostCenterDTO> EndFindCostCentersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCostCentersByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CostCenterDTO> EndFindCostCentersByFilterInPage(IAsyncResult result);
    }
}
