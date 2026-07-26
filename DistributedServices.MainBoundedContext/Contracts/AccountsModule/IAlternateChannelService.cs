using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IAlternateChannelService
    {
        #region Alternate Channel

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        AlternateChannelDTO AddAlternateChannel(AlternateChannelDTO alternateChannelDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateAlternateChannel(AlternateChannelDTO alternateChannelDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ReplaceAlternateChannel(AlternateChannelDTO alternateChannelDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RenewAlternateChannel(AlternateChannelDTO alternateChannelDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool DelinkAlternateChannel(AlternateChannelDTO alternateChannelDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool StopAlternateChannel(AlternateChannelDTO alternateChannelDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        AlternateChannelDTO FindAlternateChannel(Guid alternateChannelId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AlternateChannelDTO> FindAlternateChannelsInPage(int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AlternateChannelDTO> FindAlternateChannelsByFilterInPage(string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AlternateChannelDTO> FindAlternateChannelsByTypeAndFilterInPage(int type, int recordStatus, string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AlternateChannelDTO> FindThirdPartyNotifiableAlternateChannelsByTypeAndFilterInPage(int type, string text, int alternateChannelFilter, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<AlternateChannelDTO> FindAlternateChannelsByCustomerAccountId(Guid customerAccountId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<AlternateChannelDTO> FindAlternateChannelsByCustomerId(Guid customerId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<AlternateChannelDTO> FindAlternateChannelsByCardNumberAndCardType(string cardNumber, int cardType, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> FindCommissionsByAlternateChannelType(int alternateChannelType, int alternateChannelKnownChargeType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionsByAlternateChannelType(int alternateChannelType, List<CommissionDTO> commissions, int alternateChannelKnownChargeType, int alternateChannelChargeBenefactor);

        #endregion
    }
}
