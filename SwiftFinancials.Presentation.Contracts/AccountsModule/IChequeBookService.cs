using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IChequeBookService")]
    public interface IChequeBookService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddChequeBook(ChequeBookDTO chequeBookDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        ChequeBookDTO EndAddChequeBook(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateChequeBook(ChequeBookDTO chequeBookDTO, AsyncCallback callback, Object state);
        bool EndUpdateChequeBook(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFlagPaymentVoucher(PaymentVoucherDTO paymentVoucherDTO, AsyncCallback callback, Object state);
        bool EndFlagPaymentVoucher(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChequeBooks(AsyncCallback callback, Object state);
        List<ChequeBookDTO> EndFindChequeBooks(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChequeBooksByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ChequeBookDTO> EndFindChequeBooksByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChequeBooksByTypeAndFilterInPage(int type, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ChequeBookDTO> EndFindChequeBooksByTypeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChequeBook(Guid chequeBookId, AsyncCallback callback, Object state);
        ChequeBookDTO EndFindChequeBook(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPaymentVouchersByChequeBookId(Guid chequeBookId, AsyncCallback callback, Object state);
        List<PaymentVoucherDTO> EndFindPaymentVouchersByChequeBookId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPaymentVouchersByChequeBookIdAndFilterInPage(Guid chequeBookId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<PaymentVoucherDTO> EndFindPaymentVouchersByChequeBookIdAndFilterInPage(IAsyncResult result);
    }
}
