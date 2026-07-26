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
    public interface IDynamicChargeService
    {
        #region Dynamic Charge

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DynamicChargeDTO AddDynamicCharge(DynamicChargeDTO dynamicChargeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDynamicCharge(DynamicChargeDTO dynamicChargeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DynamicChargeDTO> FindDynamicCharges();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DynamicChargeDTO FindDynamicCharge(Guid dynamicChargeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DynamicChargeDTO> FindDynamicChargesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> FindCommissionsByDynamicChargeId(Guid dynamicChargeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionsByDynamicChargeId(Guid dynamicChargeId, List<CommissionDTO> commissions);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DynamicChargeDTO> FindDynamicChargesByFilterInPage(string text, int pageIndex, int pageSize);

        #endregion
    }
}
