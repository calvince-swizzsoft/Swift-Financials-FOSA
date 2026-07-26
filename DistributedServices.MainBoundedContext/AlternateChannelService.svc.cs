using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
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
    public class AlternateChannelService : IAlternateChannelService
    {
        private readonly IAlternateChannelAppService _alternateChannelAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public AlternateChannelService(
            IAlternateChannelAppService alternateChannelAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(alternateChannelAppService, nameof(alternateChannelAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _alternateChannelAppService = alternateChannelAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Alternate Channel

        public AlternateChannelDTO AddAlternateChannel(AlternateChannelDTO alternateChannelDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelAppService.AddNewAlternateChannel(alternateChannelDTO, serviceHeader);
        }

        public bool UpdateAlternateChannel(AlternateChannelDTO alternateChannelDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelAppService.UpdateAlternateChannel(alternateChannelDTO, serviceHeader);
        }

        public bool ReplaceAlternateChannel(AlternateChannelDTO alternateChannelDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelAppService.ReplaceAlternateChannel(alternateChannelDTO, serviceHeader);
        }

        public bool RenewAlternateChannel(AlternateChannelDTO alternateChannelDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelAppService.RenewAlternateChannel(alternateChannelDTO, serviceHeader);
        }

        public bool DelinkAlternateChannel(AlternateChannelDTO alternateChannelDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelAppService.DelinkAlternateChannel(alternateChannelDTO, serviceHeader);
        }

        public bool StopAlternateChannel(AlternateChannelDTO alternateChannelDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelAppService.StopAlternateChannel(alternateChannelDTO, serviceHeader);
        }

        public AlternateChannelDTO FindAlternateChannel(Guid alternateChannelType, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var alternateChannel = _alternateChannelAppService.FindAlternateChannel(alternateChannelType, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<AlternateChannelDTO> { alternateChannel }, serviceHeader);

            return alternateChannel;
        }

        public PageCollectionInfo<AlternateChannelDTO> FindAlternateChannelsInPage(int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var alternateChannels = _alternateChannelAppService.FindAlternateChannels(pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(alternateChannels.PageCollection, serviceHeader);

            return alternateChannels;
        }

        public PageCollectionInfo<AlternateChannelDTO> FindAlternateChannelsByFilterInPage(string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var alternateChannels = _alternateChannelAppService.FindAlternateChannels(text, alternateChannelFilter, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(alternateChannels.PageCollection, serviceHeader);

            return alternateChannels;
        }

        public PageCollectionInfo<AlternateChannelDTO> FindAlternateChannelsByTypeAndFilterInPage(int type, int recordStatus, string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var alternateChannels = _alternateChannelAppService.FindAlternateChannels(type, recordStatus, text, alternateChannelFilter, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(alternateChannels.PageCollection, serviceHeader);

            return alternateChannels;
        }

        public PageCollectionInfo<AlternateChannelDTO> FindThirdPartyNotifiableAlternateChannelsByTypeAndFilterInPage(int type, string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var alternateChannels = _alternateChannelAppService.FindThirdPartyNotifiableAlternateChannels(type, text, alternateChannelFilter, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(alternateChannels.PageCollection, serviceHeader);

            return alternateChannels;
        }

        public List<AlternateChannelDTO> FindAlternateChannelsByCustomerAccountId(Guid customerAccountId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var alternateChannels = _alternateChannelAppService.FindAlternateChannelsByCustomerAccountId(customerAccountId, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(alternateChannels, serviceHeader);

            return alternateChannels;
        }

        public List<AlternateChannelDTO> FindAlternateChannelsByCustomerId(Guid customerId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var alternateChannels = _alternateChannelAppService.FindAlternateChannelsByCustomerId(customerId, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(alternateChannels, serviceHeader);

            return alternateChannels;
        }

        public List<AlternateChannelDTO> FindAlternateChannelsByCardNumberAndCardType(string cardNumber, int cardType, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var alternateChannels = _alternateChannelAppService.FindAlternateChannelsByCardNumberAndCardType(cardNumber, cardType, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(alternateChannels, serviceHeader);

            return alternateChannels;
        }

        public List<CommissionDTO> FindCommissionsByAlternateChannelType(int alternateChannelType, int alternateChannelKnownChargeType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelAppService.FindCommissions(alternateChannelType, alternateChannelKnownChargeType, serviceHeader);
        }

        public bool UpdateCommissionsByAlternateChannelType(int alternateChannelType, List<CommissionDTO> commissions, int alternateChannelKnownChargeType, int alternateChannelChargeBenefactor)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelAppService.UpdateCommissions(alternateChannelType, commissions, alternateChannelKnownChargeType, alternateChannelChargeBenefactor, serviceHeader);
        }

        #endregion
    }
}
