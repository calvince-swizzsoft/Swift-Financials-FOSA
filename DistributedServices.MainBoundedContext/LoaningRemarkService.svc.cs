using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
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
    public class LoaningRemarkService : ILoaningRemarkService
    {
        private readonly ILoaningRemarkAppService _loaningRemarkAppService;

        public LoaningRemarkService(
            ILoaningRemarkAppService loaningRemarkAppService)
        {
            Guard.ArgumentNotNull(loaningRemarkAppService, nameof(loaningRemarkAppService));

            _loaningRemarkAppService = loaningRemarkAppService;
        }

        #region LoaningRemark

        public LoaningRemarkDTO AddLoaningRemark(LoaningRemarkDTO loaningRemarkDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loaningRemarkAppService.AddNewLoaningRemark(loaningRemarkDTO, serviceHeader);
        }

        public bool UpdateLoaningRemark(LoaningRemarkDTO loaningRemarkDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loaningRemarkAppService.UpdateLoaningRemark(loaningRemarkDTO, serviceHeader);
        }

        public List<LoaningRemarkDTO> FindLoaningRemarks()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loaningRemarkAppService.FindLoaningRemarks(serviceHeader);
        }

        public PageCollectionInfo<LoaningRemarkDTO> FindLoaningRemarksInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loaningRemarkAppService.FindLoaningRemarks(pageIndex, pageSize, serviceHeader);
        }

        public LoaningRemarkDTO FindLoaningRemark(Guid loaningRemarkId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loaningRemarkAppService.FindLoaningRemark(loaningRemarkId, serviceHeader);
        }

        public PageCollectionInfo<LoaningRemarkDTO> FindLoaningRemarksByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loaningRemarkAppService.FindLoaningRemarks(text, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
