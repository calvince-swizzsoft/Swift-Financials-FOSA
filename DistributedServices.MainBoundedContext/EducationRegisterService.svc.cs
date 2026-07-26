using Application.MainBoundedContext.DTO;
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
    public class EducationRegisterService : IEducationRegisterService
    {
        private readonly IEducationRegisterAppService _educationRegisterAppService;

        public EducationRegisterService(
            IEducationRegisterAppService educationRegisterAppService)
        {
            Guard.ArgumentNotNull(educationRegisterAppService, nameof(educationRegisterAppService));

            _educationRegisterAppService = educationRegisterAppService;
        }

        public EducationRegisterDTO AddEducationRegister(EducationRegisterDTO educationRegisterDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationRegisterAppService.AddNewEducationRegister(educationRegisterDTO, serviceHeader);
        }

        public bool UpdateEducationRegister(EducationRegisterDTO educationRegisterDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationRegisterAppService.UpdateEducationRegister(educationRegisterDTO, serviceHeader);
        }

        public EducationAttendeeDTO AddEducationAttendee(EducationAttendeeDTO educationAttendeeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationRegisterAppService.AddNewEducationAttendee(educationAttendeeDTO, serviceHeader);
        }

        public bool RemoveEducationAttendees(List<EducationAttendeeDTO> educationAttendeeDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationRegisterAppService.RemoveEducationAttendees(educationAttendeeDTOs, serviceHeader);
        }

        public bool UpdateEducationAttendeeCollectionByEducationRegisterId(Guid educationRegisterId, List<EducationAttendeeDTO> educationAttendeeCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationRegisterAppService.UpdateEducationAttendeeCollection(educationRegisterId, educationAttendeeCollection, serviceHeader);
        }

        public List<EducationRegisterDTO> FindEducationRegisters()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationRegisterAppService.FindEducationRegisters(serviceHeader);
        }

        public PageCollectionInfo<EducationRegisterDTO> FindEducationRegistersByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationRegisterAppService.FindEducationRegisters(text, pageIndex, pageSize, serviceHeader);
        }

        public EducationRegisterDTO FindEducationRegister(Guid educationRegisterId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationRegisterAppService.FindEducationRegister(educationRegisterId, serviceHeader);
        }

        public List<EducationAttendeeDTO> FindEducationAttendeesByEducationRegisterId(Guid educationRegisterId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationRegisterAppService.FindEducationAttendees(educationRegisterId, serviceHeader);
        }

        public PageCollectionInfo<EducationAttendeeDTO> FindEducationAttendeesByEducationRegisterIdInPage(Guid educationRegisterId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationRegisterAppService.FindEducationAttendees(educationRegisterId, text, pageIndex, pageSize, serviceHeader);
        }
    }
}
