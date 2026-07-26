using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WithdrawalNotificationService : IWithdrawalNotificationService
    {
        private readonly IWithdrawalNotificationAppService _withdrawalNotificationAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public WithdrawalNotificationService(
            IWithdrawalNotificationAppService withdrawalNotificationAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(withdrawalNotificationAppService, nameof(withdrawalNotificationAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _withdrawalNotificationAppService = withdrawalNotificationAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Withdrawal Notification

        public WithdrawalNotificationDTO AddWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _withdrawalNotificationAppService.AddNewWithdrawalNotification(withdrawalNotificationDTO, serviceHeader);
        }

        public bool ApproveWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalApprovalOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _withdrawalNotificationAppService.ApproveWithdrawalNotification(withdrawalNotificationDTO, membershipWithdrawalApprovalOption, serviceHeader);
        }

        public bool AuditWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalAuditOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _withdrawalNotificationAppService.AuditWithdrawalNotification(withdrawalNotificationDTO, membershipWithdrawalAuditOption, serviceHeader);
        }

        public bool SettleWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalSettlementOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _withdrawalNotificationAppService.SettleWithdrawalNotification(withdrawalNotificationDTO, membershipWithdrawalSettlementOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool ProcessDeathSettlements(WithdrawalNotificationDTO withdrawalNotificationDTO, List<WithdrawalSettlementDTO> withdrawalSettlementDTOs, InsuranceCompanyDTO insuranceCompanyDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _withdrawalNotificationAppService.ProcessDeathSettlements(withdrawalNotificationDTO, withdrawalSettlementDTOs, insuranceCompanyDTO, moduleNavigationItemCode, serviceHeader);
        }

        public List<WithdrawalNotificationDTO> FindWithdrawalNotifications()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _withdrawalNotificationAppService.FindWithdrawalNotifications(serviceHeader);
        }

        public PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotificationsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _withdrawalNotificationAppService.FindWithdrawalNotifications(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotificationsByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _withdrawalNotificationAppService.FindWithdrawalNotifications(text, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotificationsByStatusAndFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _withdrawalNotificationAppService.FindWithdrawalNotifications(startDate, endDate, status, text, customerFilter, pageIndex, pageSize, serviceHeader);
        }
        
        public WithdrawalNotificationDTO FindWithdrawalNotification(Guid withdrawalNotificationId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _withdrawalNotificationAppService.FindWithdrawalNotification(withdrawalNotificationId, serviceHeader);
        }

        public List<WithdrawalSettlementDTO> FindWithdrawalSettlementsByWithdrawalNotificationId(Guid withdrawalNotificationId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var withdrawalSettlements = _withdrawalNotificationAppService.FindWithdrawalSettlementsByWithdrawalNotificationId(withdrawalNotificationId, serviceHeader);

            if (includeProductDescription && withdrawalSettlements != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(withdrawalSettlements, serviceHeader);

            return withdrawalSettlements;
        }

        #endregion
    }
}
