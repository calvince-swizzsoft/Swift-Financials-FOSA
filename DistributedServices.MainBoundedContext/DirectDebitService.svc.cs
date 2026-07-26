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
    public class DirectDebitService : IDirectDebitService
    {
        private readonly IDirectDebitAppService _directDebitAppService;

        public DirectDebitService(
          IDirectDebitAppService directDebitAppService)
        {
            Guard.ArgumentNotNull(directDebitAppService, nameof(directDebitAppService));

            _directDebitAppService = directDebitAppService;
        }

        #region Direct Debit

        public DirectDebitDTO AddDirectDebit(DirectDebitDTO directDebitDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _directDebitAppService.AddNewDirectDebit(directDebitDTO, serviceHeader);
        }

        public bool UpdateDirectDebit(DirectDebitDTO directDebitDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _directDebitAppService.UpdateDirectDebit(directDebitDTO, serviceHeader);
        }

        public List<DirectDebitDTO> FindDirectDebits()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var directDebits = _directDebitAppService.FindDirectDebits(serviceHeader);

            _directDebitAppService.FetchDirectDebitsProductDescription(directDebits, serviceHeader);

            return directDebits;
        }

        public PageCollectionInfo<DirectDebitDTO> FindDirectDebitsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var directDebits = _directDebitAppService.FindDirectDebits(pageIndex, pageSize, serviceHeader);

            if (directDebits != null && directDebits.PageCollection != null)
                _directDebitAppService.FetchDirectDebitsProductDescription(directDebits.PageCollection, serviceHeader);

            return directDebits;
        }

        public PageCollectionInfo<DirectDebitDTO> FindDirectDebitsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var directDebits = _directDebitAppService.FindDirectDebits(text, pageIndex, pageSize, serviceHeader);

            if (directDebits != null && directDebits.PageCollection != null)
                _directDebitAppService.FetchDirectDebitsProductDescription(directDebits.PageCollection, serviceHeader);

            return directDebits;
        }

        public DirectDebitDTO FindDirectDebit(Guid directDebitId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var directDebit = _directDebitAppService.FindDirectDebit(directDebitId, serviceHeader);

            if (directDebit != null)
                _directDebitAppService.FetchDirectDebitsProductDescription(new List<DirectDebitDTO> { directDebit }, serviceHeader);

            return directDebit;
        }

        #endregion
    }
}
