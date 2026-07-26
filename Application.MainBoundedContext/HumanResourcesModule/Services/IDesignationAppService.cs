using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IDesignationAppService
    {
        DesignationDTO AddNewDesignation(DesignationDTO designationDTO, ServiceHeader serviceHeader);

        bool UpdateDesignation(DesignationDTO designationDTO, ServiceHeader serviceHeader);

        List<DesignationDTO> FindDesignations(ServiceHeader serviceHeader);

        PageCollectionInfo<DesignationDTO> FindDesignations(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<DesignationDTO> FindDesignations(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<DesignationDTO> FindDesignations(ServiceHeader serviceHeader, bool updateDepth = false, bool traverseTree = true);

        DesignationDTO FindDesignation(Guid designationId, ServiceHeader serviceHeader);

        bool UpdateTransactionThresholdCollection(Guid designationId, List<TransactionThresholdDTO> transactionThresholdCollection, ServiceHeader serviceHeader);

        List<TransactionThresholdDTO> FindTransactionThresholdCollection(Guid designationId, ServiceHeader serviceHeader);

        List<TransactionThresholdDTO> FindTransactionThresholdCollection(Guid designationId, int transactionThresholdType, ServiceHeader serviceHeader);

        bool ValidateTransactionThreshold(Guid designationId, int transactionThresholdType, decimal transactionAmount, ServiceHeader serviceHeader);
    }
}
