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
    public class LoanRequestService : ILoanRequestService
    {
        private readonly ILoanRequestAppService _loanRequestAppService;

        public LoanRequestService(
            ILoanRequestAppService loanRequestAppService)
        {
            Guard.ArgumentNotNull(loanRequestAppService, nameof(loanRequestAppService));

            _loanRequestAppService = loanRequestAppService;
        }

        #region Loan Case

        public LoanRequestDTO AddLoanRequest(LoanRequestDTO loanRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanRequestAppService.AddNewLoanRequest(loanRequestDTO, serviceHeader);
        }

        public bool RemoveLoanRequest(Guid loanRequestId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanRequestAppService.RemoveLoanRequest(loanRequestId, serviceHeader);
        }

        public bool CancelLoanRequest(LoanRequestDTO loanRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanRequestAppService.CancelLoanRequest(loanRequestDTO, serviceHeader);
        }

        public bool RegisterLoanRequest(LoanRequestDTO loanRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanRequestAppService.RegisterLoanRequest(loanRequestDTO, serviceHeader);
        }

        public LoanRequestDTO FindLoanRequest(Guid loanRequestId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanRequestAppService.FindLoanRequest(loanRequestId, serviceHeader);
        }

        public PageCollectionInfo<LoanRequestDTO> FindLoanRequestsByFilterInPage(DateTime startDate, DateTime endDate, string text, int loanRequestFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanRequestAppService.FindLoanRequests(startDate, endDate, text, loanRequestFilter, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<LoanRequestDTO> FindLoanRequestsByStatusAndFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int loanRequestFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanRequestAppService.FindLoanRequests(startDate, endDate, status, text, loanRequestFilter, pageIndex, pageSize, serviceHeader);
        }

        public List<LoanRequestDTO> FindLoanRequestsByCustomerIdInProcess(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanRequestAppService.FindLoanRequestsByCustomerIdInProcess(customerId, serviceHeader);
        }

        #endregion
    }
}
