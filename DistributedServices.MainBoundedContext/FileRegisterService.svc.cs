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
    public class FileRegisterService : IFileRegisterService
    {
        private readonly IFileRegisterAppService _fileRegisterAppService;

        public FileRegisterService(
            IFileRegisterAppService fileRegisterAppService)
        {
            Guard.ArgumentNotNull(fileRegisterAppService, nameof(fileRegisterAppService));

            _fileRegisterAppService = fileRegisterAppService;
        }

        #region File Register

        public PageCollectionInfo<FileRegisterDTO> FindFileRegistersInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fileRegisterAppService.FindFileRegisters(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<FileRegisterDTO> FindFileRegistersByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fileRegisterAppService.FindFileRegisters(text, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<FileMovementHistoryDTO> FindFileMovementHistoryByFileRegisterIdInPage(Guid fileRegisterId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fileRegisterAppService.FindFileMovementHistoryByFileRegisterId(fileRegisterId, pageIndex, pageSize, serviceHeader);
        }

        public List<FileMovementHistoryDTO> FindFileMovementHistoryByFileRegisterId(Guid fileRegisterId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fileRegisterAppService.FindFileMovementHistoryByFileRegisterId(fileRegisterId, serviceHeader);
        }

        public CustomerFileRegisterLastDepartmentInfo FindFileRegisterAndLastDepartmentByCustomerId(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fileRegisterAppService.FindFileRegisterAndLastDepartmentByCustomerId(customerId, serviceHeader);
        }

        public bool MultiDestinationDispatch(List<FileMovementHistoryDTO> fileMovementHistoryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fileRegisterAppService.MultiDestinationDispatch(fileMovementHistoryDTOs, serviceHeader);
        }

        public bool SingleDestinationDispatch(Guid sourceDepartmentId, Guid destinationDepartmentId, string remarks, string carrier, List<FileRegisterDTO> fileRegisterDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fileRegisterAppService.SingleDestinationDispatch(sourceDepartmentId, destinationDepartmentId, remarks, carrier, fileRegisterDTOs, serviceHeader);
        }

        public PageCollectionInfo<FileRegisterDTO> FindFileRegistersByFilterStatusAndLastFileMovementDestinationDepartmentIdInPage(string text, int customerFilter, int fileMovementStatus, Guid lastDestinationDepartmentId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fileRegisterAppService.FindFileRegisters(fileMovementStatus, lastDestinationDepartmentId, text, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<FileRegisterDTO> FindFileRegistersByFilterExcludingLastDestinationDepartmentIdInPage(string text, int customerFilter, Guid lastDestinationDepartmentId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fileRegisterAppService.FindFileRegistersExcludingLastDestinationDepartmentId(lastDestinationDepartmentId, text, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public bool ReceiveFiles(List<FileRegisterDTO> fileRegisterDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fileRegisterAppService.ReceiveFiles(fileRegisterDTOs, serviceHeader);
        }

        public bool RecallFiles(List<FileRegisterDTO> fileRegisterDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fileRegisterAppService.RecallFiles(fileRegisterDTOs, serviceHeader);
        }

        #endregion
    }
}
