using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IAlternateChannelService")]
    public interface IAlternateChannelService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddAlternateChannel(AlternateChannelDTO alternateChannelDTO, AsyncCallback callback, Object state);
        AlternateChannelDTO EndAddAlternateChannel(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAlternateChannel(AlternateChannelDTO alternateChannelDTO, AsyncCallback callback, Object state);
        bool EndUpdateAlternateChannel(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginReplaceAlternateChannel(AlternateChannelDTO alternateChannelDTO, AsyncCallback callback, Object state);
        bool EndReplaceAlternateChannel(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRenewAlternateChannel(AlternateChannelDTO alternateChannelDTO, AsyncCallback callback, Object state);
        bool EndRenewAlternateChannel(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginDelinkAlternateChannel(AlternateChannelDTO alternateChannelDTO, AsyncCallback callback, Object state);
        bool EndDelinkAlternateChannel(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginStopAlternateChannel(AlternateChannelDTO alternateChannelDTO, AsyncCallback callback, Object state);
        bool EndStopAlternateChannel(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannel(Guid alternateChannelId, bool includeProductDescription, AsyncCallback callback, Object state);
        AlternateChannelDTO EndFindAlternateChannel(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelsInPage(int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<AlternateChannelDTO> EndFindAlternateChannelsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelsByFilterInPage(string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<AlternateChannelDTO> EndFindAlternateChannelsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelsByTypeAndFilterInPage(int type, int recordStatus, string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<AlternateChannelDTO> EndFindAlternateChannelsByTypeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindThirdPartyNotifiableAlternateChannelsByTypeAndFilterInPage(int type, string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<AlternateChannelDTO> EndFindThirdPartyNotifiableAlternateChannelsByTypeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelsByCustomerAccountId(Guid customerAccountId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<AlternateChannelDTO> EndFindAlternateChannelsByCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelsByCustomerId(Guid customerId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<AlternateChannelDTO> EndFindAlternateChannelsByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelsByCardNumberAndCardType(string cardNumber, int cardType, bool includeProductDescription, AsyncCallback callback, Object state);
        List<AlternateChannelDTO> EndFindAlternateChannelsByCardNumberAndCardType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsByAlternateChannelType(int alternateChannelType, int alternateChannelKnownChargeType, AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissionsByAlternateChannelType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionsByAlternateChannelType(int alternateChannelType, List<CommissionDTO> commissions, int alternateChannelKnownChargeType, int alternateChannelChargeBenefactor, AsyncCallback callback, Object state);
        bool EndUpdateCommissionsByAlternateChannelType(IAsyncResult result);
    }
}
