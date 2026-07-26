using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ICostCenterService
    {
        #region Cost Center

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CostCenterDTO AddCostCenter(CostCenterDTO costCenterDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCostCenter(CostCenterDTO costCenterDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CostCenterDTO> FindCostCenters();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CostCenterDTO FindCostCenter(Guid costCenterId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CostCenterDTO> FindCostCentersInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CostCenterDTO> FindCostCentersByFilterInPage(string text, int pageIndex, int pageSize);

        #endregion
    }
}
