using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IDelegateService")]
    public interface IDelegateService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDelegate(DelegateDTO delegateDTO, AsyncCallback callback, Object state);
        DelegateDTO EndAddDelegate(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDelegate(DelegateDTO delegateDTO, AsyncCallback callback, Object state);
        bool EndUpdateDelegate(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDelegates(AsyncCallback callback, Object state);
        List<DelegateDTO> EndFindDelegates(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDelegatesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DelegateDTO> EndFindDelegatesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDelegatesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DelegateDTO> EndFindDelegatesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDelegate(Guid delegateId, AsyncCallback callback, Object state);
        DelegateDTO EndFindDelegate(IAsyncResult result);
    }
}
