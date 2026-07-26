using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IWithdrawalNotificationService
    {
        #region  Withdrawal Notification

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        WithdrawalNotificationDTO AddWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ApproveWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalApprovalOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalAuditOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool SettleWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalSettlementOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ProcessDeathSettlements(WithdrawalNotificationDTO withdrawalNotificationDTO, List<WithdrawalSettlementDTO> withdrawalSettlementDTOs, InsuranceCompanyDTO insuranceCompanyDTO, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<WithdrawalNotificationDTO> FindWithdrawalNotifications();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotificationsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotificationsByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotificationsByStatusAndFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        WithdrawalNotificationDTO FindWithdrawalNotification(Guid withdrawalNotificationId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<WithdrawalSettlementDTO> FindWithdrawalSettlementsByWithdrawalNotificationId(Guid withdrawalNotificationId, bool includeProductDescription);

        #endregion
    }
}
