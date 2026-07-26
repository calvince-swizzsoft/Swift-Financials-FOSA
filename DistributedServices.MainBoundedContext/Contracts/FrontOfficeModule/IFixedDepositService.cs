using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IFixedDepositService
    {
        #region Fixed Deposit

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        FixedDepositDTO InvokeFixedDeposit(FixedDepositDTO fixedDepositDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditFixedDeposit(FixedDepositDTO fixedDepositDTO, int fixedDepositAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RevokeFixedDeposits(List<FixedDepositDTO> fixedDepositDTOs, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PayFixedDeposit(FixedDepositDTO fixedDepositDTO, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<FixedDepositDTO> FindFixedDeposits(bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<FixedDepositDTO> FindFixedDepositsByCustomerAccountId(Guid customerAccountId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FixedDepositDTO> FindFixedDepositsByBranchIdInPage(Guid branchId, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FixedDepositDTO> FindPayableFixedDepositsByFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FixedDepositDTO> FindRevocableFixedDepositsByFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FixedDepositDTO> FindFixedDepositsByFilterInPage(string text, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FixedDepositDTO> FindFixedDepositsByStatusAndFilterInPage(int status, string text, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        FixedDepositDTO FindFixedDeposit(Guid fixedDepositId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<FixedDepositPayableDTO> FindFixedDepositPayablesByFixedDepositId(Guid fixedDepositId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateFixedDepositPayablesByFixedDepositId(Guid fixedDepositId, List<FixedDepositPayableDTO> fixedDepositPayables);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ExecutePayableFixedDeposits(DateTime targetDate, int pageSize);

        #endregion
    }
}
