using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IPostingPeriodService")]
    public interface IPostingPeriodService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddPostingPeriod(PostingPeriodDTO postingPeriodDTO, AsyncCallback callback, Object state);
        PostingPeriodDTO EndAddPostingPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdatePostingPeriod(PostingPeriodDTO postingPeriodDTO, AsyncCallback callback, Object state);
        bool EndUpdatePostingPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPostingPeriods(AsyncCallback callback, Object state);
        List<PostingPeriodDTO> EndFindPostingPeriods(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPostingPeriodsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<PostingPeriodDTO> EndFindPostingPeriodsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPostingPeriodsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<PostingPeriodDTO> EndFindPostingPeriodsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPostingPeriod(Guid postingPeriodId, AsyncCallback callback, Object state);
        PostingPeriodDTO EndFindPostingPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCurrentPostingPeriod(AsyncCallback callback, Object state);
        PostingPeriodDTO EndFindCurrentPostingPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginClosePostingPeriod(PostingPeriodDTO postingPeriodDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndClosePostingPeriod(IAsyncResult result);
    }
}
