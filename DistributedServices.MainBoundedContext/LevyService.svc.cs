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
    public class LevyService : ILevyService
    {
        private readonly ILevyAppService _levyAppService;

        public LevyService(
            ILevyAppService levyAppService)
        {
            Guard.ArgumentNotNull(levyAppService, nameof(levyAppService));

            _levyAppService = levyAppService;
        }

        #region Levy

        public LevyDTO AddLevy(LevyDTO levyDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _levyAppService.AddNewLevy(levyDTO, serviceHeader);
        }

        public bool UpdateLevy(LevyDTO levyDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _levyAppService.UpdateLevy(levyDTO, serviceHeader);
        }

        public List<LevyDTO> FindLevies()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _levyAppService.FindLevies(serviceHeader);
        }

        public LevyDTO FindLevy(Guid levyId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _levyAppService.FindLevy(levyId, serviceHeader);
        }

        public PageCollectionInfo<LevyDTO> FindLeviesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _levyAppService.FindLevies(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<LevyDTO> FindLeviesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _levyAppService.FindLevies(text, pageIndex, pageSize, serviceHeader);
        }

        public List<LevySplitDTO> FindLevySplitsByLevyId(Guid levyId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _levyAppService.FindLevySplits(levyId, serviceHeader);
        }

        public bool UpdateLevySplitsByLevyId(Guid levyId, List<LevySplitDTO> levySplits)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _levyAppService.UpdateLevySplits(levyId, levySplits, serviceHeader);
        }

        #endregion
    }
}
