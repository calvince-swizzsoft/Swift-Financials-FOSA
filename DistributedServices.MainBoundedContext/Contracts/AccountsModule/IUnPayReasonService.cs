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
    public interface IUnPayReasonService
    {
        #region UnPay Reason

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        UnPayReasonDTO AddUnPayReason(UnPayReasonDTO unPayReasonDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateUnPayReason(UnPayReasonDTO unPayReasonDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<UnPayReasonDTO> FindUnPayReasons();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        UnPayReasonDTO FindUnPayReason(Guid unPayReasonId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<UnPayReasonDTO> FindUnPayReasonsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<UnPayReasonDTO> FindUnPayReasonsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> FindCommissionsByUnPayReasonId(Guid unPayReasonId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionsByUnPayReasonId(Guid unPayReasonId, List<CommissionDTO> commissions);

        #endregion
    }
}
