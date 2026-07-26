using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IExternalChequeService
    {
        #region External Cheque

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ExternalChequeDTO AddExternalCheque(ExternalChequeDTO externalChequeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ExternalChequeDTO> FindUnClearedExternalChequesByCustomerAccountId(Guid customerAccountId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MarkExternalChequeCleared(Guid externalChequeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ExternalChequeDTO> FindUnTransferredExternalChequesByTellerIdAndFilter(Guid tellerId, string text);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExternalChequeDTO> FindUnTransferredExternalChequesByTellerIdAndFilterInPage(Guid tellerId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExternalChequeDTO> FindExternalChequesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExternalChequeDTO> FindExternalChequesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool TransferExternalCheques(List<ExternalChequeDTO> externalChequeDTOs, TellerDTO tellerDTO, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExternalChequeDTO> FindUnClearedExternalChequesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExternalChequeDTO> FindUnClearedExternalChequesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ClearExternalCheque(ExternalChequeDTO externalChequeDTO, int clearingOption, int moduleNavigationItemCode, UnPayReasonDTO unPayReasonDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExternalChequeDTO> FindUnBankedExternalChequesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool BankExternalCheques(List<ExternalChequeDTO> externalChequeDTOs, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ExternalChequePayableDTO> FindExternalChequePayablesByExternalChequeId(Guid externalChequeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateExternalChequePayablesByExternalChequeId(Guid externalChequeId, List<ExternalChequePayableDTO> externalChequePayables);

        #endregion
    }
}
