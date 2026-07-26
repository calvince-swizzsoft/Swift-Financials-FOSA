using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "ILevyService")]
    public interface ILevyService 
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLevy(LevyDTO levyDTO, AsyncCallback callback, Object state);
        LevyDTO EndAddLevy(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLevy(LevyDTO levyDTO, AsyncCallback callback, Object state);
        bool EndUpdateLevy(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLevies(AsyncCallback callback, Object state);
        List<LevyDTO> EndFindLevies(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLevy(Guid levyId, AsyncCallback callback, Object state);
        LevyDTO EndFindLevy(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeviesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LevyDTO> EndFindLeviesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeviesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LevyDTO> EndFindLeviesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLevySplitsByLevyId(Guid levyId, AsyncCallback callback, Object state);
        List<LevySplitDTO> EndFindLevySplitsByLevyId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLevySplitsByLevyId(Guid levyId, List<LevySplitDTO> levySplits, AsyncCallback callback, Object state);
        bool EndUpdateLevySplitsByLevyId(IAsyncResult result);
    }
}
