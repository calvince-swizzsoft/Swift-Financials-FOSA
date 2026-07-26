using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IPostingPeriodService
    {
        #region Posting Period

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PostingPeriodDTO AddPostingPeriod(PostingPeriodDTO postingPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdatePostingPeriod(PostingPeriodDTO postingPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PostingPeriodDTO> FindPostingPeriods();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<PostingPeriodDTO> FindPostingPeriodsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<PostingPeriodDTO> FindPostingPeriodsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PostingPeriodDTO FindPostingPeriod(Guid postingPeriodId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PostingPeriodDTO FindCurrentPostingPeriod();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ClosePostingPeriod(PostingPeriodDTO postingPeriodDTO, int moduleNavigationItemCode);

        #endregion
    }
}
