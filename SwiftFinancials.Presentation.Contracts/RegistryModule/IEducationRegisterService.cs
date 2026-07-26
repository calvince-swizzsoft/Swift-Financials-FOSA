using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IEducationRegisterService")]
    public interface IEducationRegisterService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEducationRegister(EducationRegisterDTO educationRegisterDTO, AsyncCallback callback, Object state);
        EducationRegisterDTO EndAddEducationRegister(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEducationRegister(EducationRegisterDTO educationRegisterDTO, AsyncCallback callback, Object state);
        bool EndUpdateEducationRegister(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEducationAttendee(EducationAttendeeDTO educationAttendeeDTO, AsyncCallback callback, Object state);
        EducationAttendeeDTO EndAddEducationAttendee(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveEducationAttendees(List<EducationAttendeeDTO> educationAttendeeDTOs, AsyncCallback callback, Object state);
        bool EndRemoveEducationAttendees(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEducationAttendeeCollectionByEducationRegisterId(Guid educationRegisterId, List<EducationAttendeeDTO> educationAttendeeCollection, AsyncCallback callback, Object state);
        bool EndUpdateEducationAttendeeCollectionByEducationRegisterId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEducationRegisters(AsyncCallback callback, Object state);
        List<EducationRegisterDTO> EndFindEducationRegisters(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEducationRegistersByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EducationRegisterDTO> EndFindEducationRegistersByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEducationRegister(Guid educationRegisterId, AsyncCallback callback, Object state);
        EducationRegisterDTO EndFindEducationRegister(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEducationAttendeesByEducationRegisterId(Guid educationRegisterId, AsyncCallback callback, Object state);
        List<EducationAttendeeDTO> EndFindEducationAttendeesByEducationRegisterId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEducationAttendeesByEducationRegisterIdInPage(Guid educationRegisterId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EducationAttendeeDTO> EndFindEducationAttendeesByEducationRegisterIdInPage(IAsyncResult result);
    }
}
