using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IChequeBookService
    {
        #region Cheque Book

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ChequeBookDTO AddChequeBook(ChequeBookDTO chequeBookDTO, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateChequeBook(ChequeBookDTO chequeBookDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool FlagPaymentVoucher(PaymentVoucherDTO paymentVoucherDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ChequeBookDTO> FindChequeBooks();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ChequeBookDTO> FindChequeBooksByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ChequeBookDTO> FindChequeBooksByTypeAndFilterInPage(int type, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ChequeBookDTO FindChequeBook(Guid chequeBookId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PaymentVoucherDTO> FindPaymentVouchersByChequeBookId(Guid chequeBookId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<PaymentVoucherDTO> FindPaymentVouchersByChequeBookIdAndFilterInPage(Guid chequeBookId, string text, int pageIndex, int pageSize);

        #endregion
    }
}
