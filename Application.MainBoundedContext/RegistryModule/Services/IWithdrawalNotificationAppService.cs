using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface IWithdrawalNotificationAppService
    {
        WithdrawalNotificationDTO AddNewWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, ServiceHeader serviceHeader);

        bool ApproveWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalApprovalOption, ServiceHeader serviceHeader);

        bool AuditWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalAuditOption, ServiceHeader serviceHeader);

        bool SettleWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalSettlementOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool ProcessDeathSettlements(WithdrawalNotificationDTO withdrawalNotificationDTO, List<WithdrawalSettlementDTO> withdrawalSettlementDTOs, InsuranceCompanyDTO insuranceCompanyDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        List<WithdrawalNotificationDTO> FindWithdrawalNotifications(ServiceHeader serviceHeader);

        PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotifications(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotifications(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotifications(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        WithdrawalNotificationDTO FindWithdrawalNotification(Guid withdrawalNotificationId, ServiceHeader serviceHeader);

        List<WithdrawalNotificationDTO> FindWithdrawalNotificationsByCustomerId(Guid customerId, ServiceHeader serviceHeader);

        List<WithdrawalSettlementDTO> FindWithdrawalSettlementsByWithdrawalNotificationId(Guid withdrawalNotificationId, ServiceHeader serviceHeader);
    }
}
