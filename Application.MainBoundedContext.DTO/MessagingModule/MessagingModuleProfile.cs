using AutoMapper;
using Domain.MainBoundedContext.MessagingModule.Aggregates.CustomerMessageHistoryAgg;
using Domain.MainBoundedContext.MessagingModule.Aggregates.EmailAlertAgg;
using Domain.MainBoundedContext.MessagingModule.Aggregates.MessageGroupAgg;
using Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertAgg;
using Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertCommissionAgg;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class MessagingModuleProfile : Profile
    {
        public MessagingModuleProfile()
        {
            //TextAlert => TextAlertDTO
            CreateMap<TextAlert, TextAlertDTO>()
                .ForMember(dest => dest.MaskedTextMessageBody, opt => opt.MapFrom(src => src.TextMessage.SecurityCritical ? "Security Critical" : src.TextMessage.Body))
                .ForMember(dest => dest.TextMessageDLRStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.TextMessageOriginDescription, opt => opt.Ignore());

            //EmailAlert => EmailAlertDTO
            CreateMap<EmailAlert, EmailAlertDTO>()
                .ForMember(dest => dest.MaskedMailMessageBody, opt => opt.MapFrom(src => src.MailMessage.SecurityCritical ? "Security Critical" : src.MailMessage.Body))
                .ForMember(dest => dest.MailMessageDLRStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.MailMessageOriginDescription, opt => opt.Ignore());

            //MessageGroup => MessageGroupDTO
            CreateMap<MessageGroup, MessageGroupDTO>()
                .ForMember(dest => dest.TargetDescription, opt => opt.Ignore());

            //CustomerMessageHistory => CustomerMessageHistoryDTO
            CreateMap<CustomerMessageHistory, CustomerMessageHistoryDTO>()
                .ForMember(dest => dest.MessageCategoryDescription, opt => opt.Ignore());

            //TextAlertCommission => TextAlertCommissionDTO
            CreateMap<TextAlertCommission, TextAlertCommissionDTO>();
        }
    }
}
