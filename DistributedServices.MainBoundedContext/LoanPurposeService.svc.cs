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
    public class LoanPurposeService : ILoanPurposeService
    {
        private readonly ILoanPurposeAppService _loanPurposeAppService;

        public LoanPurposeService(
            ILoanPurposeAppService loanPurposeAppService)
        {
            Guard.ArgumentNotNull(loanPurposeAppService, nameof(loanPurposeAppService));

            _loanPurposeAppService = loanPurposeAppService;
        }

        #region Loan Purpose

        public LoanPurposeDTO AddLoanPurpose(LoanPurposeDTO loanPurposeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanPurposeAppService.AddNewLoanPurpose(loanPurposeDTO, serviceHeader);
        }

        public bool UpdateLoanPurpose(LoanPurposeDTO loanPurposeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanPurposeAppService.UpdateLoanPurpose(loanPurposeDTO, serviceHeader);
        }

        public List<LoanPurposeDTO> FindLoanPurposes()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanPurposeAppService.FindLoanPurposes(serviceHeader);
        }

        public LoanPurposeDTO FindLoanPurpose(Guid loanPurposeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanPurposeAppService.FindLoanPurpose(loanPurposeId, serviceHeader);
        }

        public PageCollectionInfo<LoanPurposeDTO> FindLoanPurposesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanPurposeAppService.FindLoanPurposes(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<LoanPurposeDTO> FindLoanPurposesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanPurposeAppService.FindLoanPurposes(text, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
