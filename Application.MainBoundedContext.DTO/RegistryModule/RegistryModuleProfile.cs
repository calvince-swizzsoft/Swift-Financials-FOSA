using AutoMapper;
using Domain.MainBoundedContext.RegistryModule.Aggregates.AccountAlertAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.AdministrativeDivisionAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CommissionExemptionAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CommissionExemptionEntryAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingEntryAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CorporationMemberAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerCreditTypeAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerDocumentAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.DirectorAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.DivisionAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EducationAttendeeAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EducationRegisterAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EducationVenueAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EmployerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.FileMovementHistoryAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.FileRegisterAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.FuneralRiderClaimAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.NextOfKinAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.PartnershipMemberAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.PopulationRegisterQueryAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.RefereeAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.StationAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalNotificationAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalSettlementAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ZoneAgg;
using System;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class RegistryModuleProfile : Profile
    {
        public RegistryModuleProfile()
        {
            //Customer => CustomerDTO
            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.BiometricEnrollment, opt => opt.Ignore())
                .ForMember(dest => dest.BiometricFingerprintBuffer, opt => opt.Ignore())
                .ForMember(dest => dest.BiometricFingerprintTemplateBuffer, opt => opt.Ignore())
                .ForMember(dest => dest.BiometricFingerVeinTemplateBuffer, opt => opt.Ignore())
                .ForMember(dest => dest.PassportBuffer, opt => opt.Ignore())
                .ForMember(dest => dest.SignatureBuffer, opt => opt.Ignore())
                .ForMember(dest => dest.IdentityCardBackSideBuffer, opt => opt.Ignore())
                .ForMember(dest => dest.IdentityCardFrontSideBuffer, opt => opt.Ignore())
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.IndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.IndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.IndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.IndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.IndividualEmploymentTermsOfServiceDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullName, opt => opt.Ignore())
                .ForMember(dest => dest.Age, opt => opt.Ignore())
                .ForMember(dest => dest.MembershipPeriod, opt => opt.Ignore());

            //CustomerDTO => CustomerBindingModel
            CreateMap<CustomerDTO, CustomerBindingModel>();

            //Employer => EmployerDTO
            CreateMap<Employer, EmployerDTO>();

            //EmployerDTO => EmployerBindingModel
            CreateMap<EmployerDTO, EmployerBindingModel>();

            //NextOfKin => NextOfKinDTO
            CreateMap<NextOfKin, NextOfKinDTO>()
                .ForMember(dest => dest.SalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.GenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.RelationshipDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //NextOfKinDTO => NextOfKinBindingModel
            CreateMap<NextOfKinDTO, NextOfKinBindingModel>();

            //FileRegister => FileRegisterDTO
            CreateMap<FileRegister, FileRegisterDTO>()
                .ForMember(dest => dest.History, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //FileMovementHistory => FileMovementHistoryDTO
            CreateMap<FileMovementHistory, FileMovementHistoryDTO>()
                .ForMember(dest => dest.FileRegisterCustomerFullName, opt => opt.Ignore());

            //Delegate => DelegateDTO
            CreateMap<Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg.Delegate, DelegateDTO>()
                .ForMember(dest => dest.PaddedCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //Director => DirectorDTO
            CreateMap<Director, DirectorDTO>()
                .ForMember(dest => dest.PaddedCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //Division => DivisionDTO
            CreateMap<Division, DivisionDTO>();

            //DivisionDTO => DivisionBindingModel
            CreateMap<DivisionDTO, DivisionBindingModel>();

            //Station => StationDTO
            CreateMap<Station, StationDTO>();

            //StationDTO => StationBindingModel
            CreateMap<StationDTO, StationBindingModel>();

            //Zone => ZoneDTO
            CreateMap<Zone, ZoneDTO>();

            //ZoneDTO => ZoneBindingModel
            CreateMap<ZoneDTO, ZoneBindingModel>();

            //CustomerDocument => CustomerDocumentDTO
            CreateMap<CustomerDocument, CustomerDocumentDTO>()
                .ForMember(dest => dest.PaddedCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.FileBuffer, opt => opt.Ignore());

            //CustomerDocumentDTO => CustomerDocumentBindingModel
            CreateMap<CustomerDocumentDTO, CustomerDocumentBindingModel>();

            //WithdrawalNotification => WithdrawalNotificationDTO
            CreateMap<WithdrawalNotification, WithdrawalNotificationDTO>()
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryDescription, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //WithdrawalSettlement => WithdrawalSettlementDTO
            CreateMap<WithdrawalSettlement, WithdrawalSettlementDTO>()
                .ForMember(dest => dest.FullAccountNumber, opt => opt.Ignore());

            //CorporationMember => CorporationMemberDTO
            CreateMap<CorporationMember, CorporationMemberDTO>();

            //PartnershipMember => PartnershipMemberDTO
            CreateMap<PartnershipMember, PartnershipMemberDTO>()
                .ForMember(dest => dest.IdentityCardType, opt => opt.MapFrom(src => (int)(src.IdentityCardType)))
                .ForMember(dest => dest.Relationship, opt => opt.MapFrom(src => (int)(src.Relationship)));

            //Referee => RefereeDTO
            CreateMap<Referee, RefereeDTO>();

            //AccountAlert => AccountAlertDTO
            CreateMap<AccountAlert, AccountAlertDTO>()
                .ForMember(dest => dest.PaddedCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //AccountAlertDTO => AccountAlertBindingModel
            CreateMap<AccountAlertDTO, AccountAlertBindingModel>();

            //CommissionExemption => CommissionExemptionDTO
            CreateMap<CommissionExemption, CommissionExemptionDTO>();

            //CommissionExemptionDTO => CommissionExemptionBindingModel
            CreateMap<CommissionExemptionDTO, CommissionExemptionBindingModel>();

            //CommissionExemptionEntry => CommissionExemptionEntryDTO
            CreateMap<CommissionExemptionEntry, CommissionExemptionEntryDTO>();

            //CommissionExemptionEntryDTO => CommissionExemptionEntryBindingModel
            CreateMap<CommissionExemptionEntryDTO, CommissionExemptionEntryBindingModel>();

            //CustomerCreditType => CustomerCreditTypeDTO
            CreateMap<CustomerCreditType, CustomerCreditTypeDTO>();

            //EducationVenue => EducationVenueDTO
            CreateMap<EducationVenue, EducationVenueDTO>();

            //EducationRegister => EducationRegisterDTO
            CreateMap<EducationRegister, EducationRegisterDTO>();

            //EducationAttendee => EducationAttendeeDTO
            CreateMap<EducationAttendee, EducationAttendeeDTO>();

            //ConditionalLending => ConditionalLendingDTO
            CreateMap<ConditionalLending, ConditionalLendingDTO>();

            //ConditionalLendingDTO => ConditionalLendingBindingModel
            CreateMap<ConditionalLendingDTO, ConditionalLendingBindingModel>();

            //ConditionalLendingEntry => ConditionalLendingEntryDTO
            CreateMap<ConditionalLendingEntry, ConditionalLendingEntryDTO>();

            //ConditionalLendingEntryDTO => ConditionalLendingEntryBindingModel
            CreateMap<ConditionalLendingEntryDTO, ConditionalLendingEntryBindingModel>();

            //FuneralRiderClaim => FuneralRiderClaimDTO
            CreateMap<FuneralRiderClaim, FuneralRiderClaimDTO>();

            //AdministrativeDivision => AdministrativeDivisionDTO
            CreateMap<AdministrativeDivision, AdministrativeDivisionDTO>();

            //AdministrativeDivisionDTO => AdministrativeDivisionBindingModel
            CreateMap<AdministrativeDivisionDTO, AdministrativeDivisionBindingModel>();

            //PopulationRegisterQuery => PopulationRegisterQueryDTO
            CreateMap<PopulationRegisterQuery, PopulationRegisterQueryDTO>()
                .ForMember(dest => dest.PhotoBuffer, opt => opt.Ignore())
                .ForMember(dest => dest.PhotoFromPassportBuffer, opt => opt.Ignore())
                .ForMember(dest => dest.SignatureBuffer, opt => opt.Ignore())
                .ForMember(dest => dest.FingerprintBuffer, opt => opt.Ignore());

            //PopulationRegisterQueryDTO => PopulationRegisterQueryBindingModel
            CreateMap<PopulationRegisterQueryDTO, PopulationRegisterQueryBindingModel>();
        }
    }
}
