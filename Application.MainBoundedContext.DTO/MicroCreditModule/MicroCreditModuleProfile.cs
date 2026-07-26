using AutoMapper;
using Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupAgg;
using Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupMemberAgg;
using Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditOfficerAgg;

namespace Application.MainBoundedContext.DTO.MicroCreditModule
{
    public class MicroCreditModuleProfile : Profile
    {
        public MicroCreditModuleProfile()
        {
            //MicroCreditOfficer => MicroCreditOfficerDTO
            CreateMap<MicroCreditOfficer, MicroCreditOfficerDTO>()
                .ForMember(dest => dest.EmployeeBloodGroupDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeTypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedEmployeeCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerIndividualGenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerIndividualMaritalStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerIndividualNationalityDescription, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerFullName, opt => opt.Ignore());

            //MicroCreditGroup => MicroCreditGroupDTO
            CreateMap<MicroCreditGroup, MicroCreditGroupDTO>()
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.MeetingFrequencyDescription, opt => opt.Ignore());

            //MicroCreditGroupMember => MicroCreditGroupMemberDTO
            CreateMap<MicroCreditGroupMember, MicroCreditGroupMemberDTO>()
                .ForMember(dest => dest.DesignationDescription, opt => opt.Ignore());
        }
    }
}
