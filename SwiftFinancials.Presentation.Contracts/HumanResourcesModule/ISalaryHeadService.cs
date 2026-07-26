using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "ISalaryHeadService")]
    public interface ISalaryHeadService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSalaryHead(SalaryHeadDTO salaryHeadDTO, AsyncCallback callback, Object state);
        SalaryHeadDTO EndAddSalaryHead(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateSalaryHead(SalaryHeadDTO salaryHeadDTO, AsyncCallback callback, Object state);
        bool EndUpdateSalaryHead(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryHeads(bool includeProductDescription, AsyncCallback callback, Object state);
        List<SalaryHeadDTO> EndFindSalaryHeads(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryHeadsInPage(int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<SalaryHeadDTO> EndFindSalaryHeadsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryHeadsByFilterInPage(string text, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<SalaryHeadDTO> EndFindSalaryHeadsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryHead(Guid salaryHeadId, AsyncCallback callback, Object state);
        SalaryHeadDTO EndFindSalaryHead(IAsyncResult result);
    }
}
