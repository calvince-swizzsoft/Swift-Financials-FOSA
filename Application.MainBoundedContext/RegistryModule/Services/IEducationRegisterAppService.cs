using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface IEducationRegisterAppService
    {
        EducationRegisterDTO AddNewEducationRegister(EducationRegisterDTO educationRegisterDTO, ServiceHeader serviceHeader);

        bool UpdateEducationRegister(EducationRegisterDTO educationRegisterDTO, ServiceHeader serviceHeader);

        EducationAttendeeDTO AddNewEducationAttendee(EducationAttendeeDTO educationAttendeeDTO, ServiceHeader serviceHeader);

        bool RemoveEducationAttendees(List<EducationAttendeeDTO> educationAttendeeDTOs, ServiceHeader serviceHeader);

        bool UpdateEducationAttendeeCollection(Guid educationRegisterId, List<EducationAttendeeDTO> educationAttendeeCollection, ServiceHeader serviceHeader);

        List<EducationRegisterDTO> FindEducationRegisters(ServiceHeader serviceHeader);

        PageCollectionInfo<EducationRegisterDTO> FindEducationRegisters(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        EducationRegisterDTO FindEducationRegister(Guid educationRegisterId, ServiceHeader serviceHeader);

        List<EducationAttendeeDTO> FindEducationAttendees(Guid educationRegisterId, ServiceHeader serviceHeader);

        PageCollectionInfo<EducationAttendeeDTO> FindEducationAttendees(Guid educationRegisterId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);
    }
}
