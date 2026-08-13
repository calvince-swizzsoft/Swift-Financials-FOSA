using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IAlternateChannelAppService
    {
        AlternateChannelDTO AddNewAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader);

        bool UpdateAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader);

        bool ReplaceAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader);

        bool RenewAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader);

        bool DelinkAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader);

        bool StopAlternateChannel(AlternateChannelDTO alternateChannelDTO, ServiceHeader serviceHeader);

        // Sets (or resets) the transacting-session PIN for an existing link, hashed at rest
        // via the same PasswordHash (PBKDF2) utility EmployeePasswordHistoryAppService already
        // uses for staff credentials — never stored or returned in plain text. AddNewAlternateChannel
        // also hashes+stores AlternateChannelDTO.MobilePIN when supplied, so linking can set the
        // PIN in the same call; this method exists for setting/resetting it afterward without
        // touching every other field the way UpdateAlternateChannel would require.
        bool SetMobilePIN(Guid alternateChannelId, string pin, ServiceHeader serviceHeader);

        // Validates a supplied PIN against the stored hash. False (not an exception) for a
        // missing channel, an unset PIN, or a wrong PIN alike — deliberately indistinguishable,
        // same reasoning as every other credential-check in this codebase.
        bool VerifyMobilePIN(Guid alternateChannelId, string pin, ServiceHeader serviceHeader);

        AlternateChannelDTO FindAlternateChannel(Guid alternateChannelId, ServiceHeader serviceHeader);

        List<AlternateChannelDTO> FindAlternateChannels(ServiceHeader serviceHeader);

        PageCollectionInfo<AlternateChannelDTO> FindAlternateChannels(int pageIndex, int pageCount, ServiceHeader serviceHeader);

        PageCollectionInfo<AlternateChannelDTO> FindAlternateChannels(string text, int alternateChannelFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<AlternateChannelDTO> FindAlternateChannels(int type, int recordStatus, string text, int alternateChannelFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<AlternateChannelDTO> FindThirdPartyNotifiableAlternateChannels(int type, string text, int alternateChannelFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<AlternateChannelDTO> FindAlternateChannelsByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader);

        List<AlternateChannelDTO> FindAlternateChannelsByCustomerId(Guid customerId, ServiceHeader serviceHeader);

        List<AlternateChannelDTO> FindAlternateChannelsByCardNumberAndCardType(string cardNumber, int cardType, ServiceHeader serviceHeader);

        List<AlternateChannelDTO> FindAlternateChannelsByCardNumber(string cardNumber, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCommissions(int alternateChannelType, int alternateChannelTypeKnownChargeType, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCachedCommissions(int alternateChannelType, int alternateChannelTypeKnownChargeType, ServiceHeader serviceHeader);

        bool UpdateCommissions(int alternateChannelType, List<CommissionDTO> commissionDTOs, int alternateChannelTypeKnownChargeType, int alternateChannelTypeChargeBenefactor, ServiceHeader serviceHeader);
    }
}
