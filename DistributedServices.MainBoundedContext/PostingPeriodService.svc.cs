using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
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
    public class PostingPeriodService : IPostingPeriodService
    {
        private readonly IPostingPeriodAppService _postingPeriodAppService;

        public PostingPeriodService(
            IPostingPeriodAppService postingPeriodAppService)
        {
            Guard.ArgumentNotNull(postingPeriodAppService, nameof(postingPeriodAppService));

            _postingPeriodAppService = postingPeriodAppService;
        }

        #region Posting Period

        public PostingPeriodDTO AddPostingPeriod(PostingPeriodDTO postingPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _postingPeriodAppService.AddNewPostingPeriod(postingPeriodDTO, serviceHeader);
        }

        public bool UpdatePostingPeriod(PostingPeriodDTO postingPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _postingPeriodAppService.UpdatePostingPeriod(postingPeriodDTO, serviceHeader);
        }

        public List<PostingPeriodDTO> FindPostingPeriods()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _postingPeriodAppService.FindPostingPeriods(serviceHeader);
        }

        public PageCollectionInfo<PostingPeriodDTO> FindPostingPeriodsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _postingPeriodAppService.FindPostingPeriods(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<PostingPeriodDTO> FindPostingPeriodsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _postingPeriodAppService.FindPostingPeriods(text, pageIndex, pageSize, serviceHeader);
        }

        public PostingPeriodDTO FindPostingPeriod(Guid postingPeriodId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _postingPeriodAppService.FindPostingPeriod(postingPeriodId, serviceHeader);
        }

        public PostingPeriodDTO FindCurrentPostingPeriod()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
        }

        public bool ClosePostingPeriod(PostingPeriodDTO postingPeriodDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _postingPeriodAppService.ClosePostingPeriod(postingPeriodDTO, moduleNavigationItemCode, serviceHeader);
        }

        #endregion
    }
}
