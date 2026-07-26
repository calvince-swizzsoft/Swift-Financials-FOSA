using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IEducationRegisterService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EducationRegisterDTO AddEducationRegister(EducationRegisterDTO educationRegisterDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEducationRegister(EducationRegisterDTO educationRegisterDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EducationAttendeeDTO AddEducationAttendee(EducationAttendeeDTO educationAttendeeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveEducationAttendees(List<EducationAttendeeDTO> educationAttendeeDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEducationAttendeeCollectionByEducationRegisterId(Guid educationRegisterId, List<EducationAttendeeDTO> educationAttendeeCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EducationRegisterDTO> FindEducationRegisters();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EducationRegisterDTO> FindEducationRegistersByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EducationRegisterDTO FindEducationRegister(Guid educationRegisterId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EducationAttendeeDTO> FindEducationAttendeesByEducationRegisterId(Guid educationRegisterId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EducationAttendeeDTO> FindEducationAttendeesByEducationRegisterIdInPage(Guid educationRegisterId, string text, int pageIndex, int pageSize);
    }
}
