using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class BranchService : IBranchService
    {
        private readonly IBranchAppService _branchAppService;

        public BranchService(
            IBranchAppService branchAppService)
        {
            Guard.ArgumentNotNull(branchAppService, nameof(branchAppService));

            _branchAppService = branchAppService;
        }

        #region Branch

        public BranchDTO AddBranch(BranchDTO branchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _branchAppService.AddNewBranch(branchDTO, serviceHeader);
        }

        public async Task<bool> UpdateBranchAsync(BranchDTO branchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _branchAppService.UpdateBranchAsync(branchDTO, serviceHeader);
        }

        public List<BranchDTO> FindBranches()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _branchAppService.FindBranches(serviceHeader);
        }

        public PageCollectionInfo<BranchDTO> FindBranchesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _branchAppService.FindBranches(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<BranchDTO> FindBranchesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _branchAppService.FindBranches(text, pageIndex, pageSize, serviceHeader);
        }

        public BranchDTO FindBranch(Guid branchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _branchAppService.FindBranch(branchId, serviceHeader);
        }

        public BranchDTO FindBranchByCode(int branchCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _branchAppService.FindBranch(branchCode, serviceHeader);
        }

        #endregion
    }
}
