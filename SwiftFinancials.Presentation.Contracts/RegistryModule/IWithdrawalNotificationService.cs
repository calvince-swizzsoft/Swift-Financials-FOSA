using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IWithdrawalNotificationService")]
    public interface IWithdrawalNotificationService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, AsyncCallback callback, Object state);
        WithdrawalNotificationDTO EndAddWithdrawalNotification(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginApproveWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalApprovalOption, AsyncCallback callback, Object state);
        bool EndApproveWithdrawalNotification(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, AsyncCallback callback, Object state);
        bool EndUpdateWithdrawalNotification(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalAuditOption, AsyncCallback callback, Object state);
        bool EndAuditWithdrawalNotification(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginSettleWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalSettlementOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndSettleWithdrawalNotification(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginProcessDeathSettlements(WithdrawalNotificationDTO withdrawalNotificationDTO, List<WithdrawalSettlementDTO> withdrawalSettlementDTOs, InsuranceCompanyDTO insuranceCompanyDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndProcessDeathSettlements(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWithdrawalNotifications(AsyncCallback callback, Object state);
        List<WithdrawalNotificationDTO> EndFindWithdrawalNotifications(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWithdrawalNotificationsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<WithdrawalNotificationDTO> EndFindWithdrawalNotificationsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWithdrawalNotificationsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<WithdrawalNotificationDTO> EndFindWithdrawalNotificationsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWithdrawalNotificationsByStatusAndFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<WithdrawalNotificationDTO> EndFindWithdrawalNotificationsByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWithdrawalNotification(Guid withdrawalNotificationId, AsyncCallback callback, Object state);
        WithdrawalNotificationDTO EndFindWithdrawalNotification(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindWithdrawalSettlementsByWithdrawalNotificationId(Guid withdrawalNotificationId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<WithdrawalSettlementDTO> EndFindWithdrawalSettlementsByWithdrawalNotificationId(IAsyncResult result);

        
       
    }
}
