using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ISalaryHeadService
    {
        #region Salary Head

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalaryHeadDTO AddSalaryHead(SalaryHeadDTO salaryHeadDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateSalaryHead(SalaryHeadDTO salaryHeadDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SalaryHeadDTO> FindSalaryHeads(bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SalaryHeadDTO> FindSalaryHeadsInPage(int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SalaryHeadDTO> FindSalaryHeadsByFilterInPage(string text, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalaryHeadDTO FindSalaryHead(Guid salaryHeadId);

        #endregion
    }
}
