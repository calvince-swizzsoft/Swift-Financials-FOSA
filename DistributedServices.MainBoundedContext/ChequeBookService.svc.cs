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
    public class ChequeBookService : IChequeBookService
    {
        private readonly IChequeBookAppService _chequeBookAppService;

        public ChequeBookService(
            IChequeBookAppService chequeBookAppService)
        {
            Guard.ArgumentNotNull(chequeBookAppService, nameof(chequeBookAppService));

            _chequeBookAppService = chequeBookAppService;
        }

        #region Cheque Book

        public ChequeBookDTO AddChequeBook(ChequeBookDTO chequeBookDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeBookAppService.AddNewChequeBook(chequeBookDTO, moduleNavigationItemCode, serviceHeader);
        }

        public bool UpdateChequeBook(ChequeBookDTO chequeBookDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeBookAppService.UpdateChequeBook(chequeBookDTO, serviceHeader);
        }

        public bool FlagPaymentVoucher(PaymentVoucherDTO paymentVoucherDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeBookAppService.FlagVoucher(paymentVoucherDTO, serviceHeader);
        }

        public List<ChequeBookDTO> FindChequeBooks()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeBookAppService.FindChequeBooks(serviceHeader);
        }

        public PageCollectionInfo<ChequeBookDTO> FindChequeBooksByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeBookAppService.FindChequeBooks(text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<ChequeBookDTO> FindChequeBooksByTypeAndFilterInPage(int type, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeBookAppService.FindChequeBooks(type, text, pageIndex, pageSize, serviceHeader);
        }

        public ChequeBookDTO FindChequeBook(Guid chequeBookId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeBookAppService.FindChequeBook(chequeBookId, serviceHeader);
        }

        public List<PaymentVoucherDTO> FindPaymentVouchersByChequeBookId(Guid chequeBookId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeBookAppService.FindPaymentVouchersByChequeBookId(chequeBookId, serviceHeader);
        }

        public PageCollectionInfo<PaymentVoucherDTO> FindPaymentVouchersByChequeBookIdAndFilterInPage(Guid chequeBookId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _chequeBookAppService.FindPaymentVouchersByChequeBookId(chequeBookId, text, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
