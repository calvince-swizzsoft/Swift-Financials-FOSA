using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface IFileRegisterAppService
    {
        bool MultiDestinationDispatch(List<FileMovementHistoryDTO> fileMovementHistoryDTOs, ServiceHeader serviceHeader);

        bool SingleDestinationDispatch(Guid sourceDepartmentId, Guid destinationDepartmentId, string remarks, string carrier, List<FileRegisterDTO> fileRegisterDTOs, ServiceHeader serviceHeader);

        bool ReceiveFiles(List<FileRegisterDTO> fileRegisterDTOs, ServiceHeader serviceHeader);

        bool RecallFiles(List<FileRegisterDTO> fileRegisterDTOs, ServiceHeader serviceHeader);

        FileRegisterDTO FindFileRegister(Guid fileRegisterId, ServiceHeader serviceHeader);

        FileRegisterDTO FindFileRegisterByCustomerId(Guid customerId, ServiceHeader serviceHeader);

        List<FileRegisterDTO> FindFileRegisters(ServiceHeader serviceHeader);

        PageCollectionInfo<FileRegisterDTO> FindFileRegisters(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FileRegisterDTO> FindFileRegisters(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FileRegisterDTO> FindFileRegisters(int fileMovementStatus, Guid lastDestinationDepartmentId, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FileRegisterDTO> FindFileRegistersExcludingLastDestinationDepartmentId(Guid lastDestinationDepartmentId, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        CustomerFileRegisterLastDepartmentInfo FindFileRegisterAndLastDepartmentByCustomerId(Guid customerId, ServiceHeader serviceHeader);

        PageCollectionInfo<FileMovementHistoryDTO> FindFileMovementHistoryByFileRegisterId(Guid fileRegisterId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<FileMovementHistoryDTO> FindFileMovementHistoryByFileRegisterId(Guid fileRegisterId, ServiceHeader serviceHeader);
    }
}
