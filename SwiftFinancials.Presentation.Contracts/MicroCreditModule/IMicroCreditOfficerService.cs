using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.MicroCreditModule
{
    [ServiceContract(Name = "IMicroCreditOfficerService")]
    public interface IMicroCreditOfficerService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddMicroCreditOfficer(MicroCreditOfficerDTO microCreditOfficerDTO, AsyncCallback callback, Object state);
        MicroCreditOfficerDTO EndAddMicroCreditOfficer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateMicroCreditOfficer(MicroCreditOfficerDTO microCreditOfficerDTO, AsyncCallback callback, Object state);
        bool EndUpdateMicroCreditOfficer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMicroCreditOfficers(AsyncCallback callback, Object state);
        List<MicroCreditOfficerDTO> EndFindMicroCreditOfficers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMicroCreditOfficersInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<MicroCreditOfficerDTO> EndFindMicroCreditOfficersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMicroCreditOfficersByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<MicroCreditOfficerDTO> EndFindMicroCreditOfficersByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMicroCreditOfficer(Guid microCreditOfficerId, AsyncCallback callback, Object state);
        MicroCreditOfficerDTO EndFindMicroCreditOfficer(IAsyncResult result);
    }
}
