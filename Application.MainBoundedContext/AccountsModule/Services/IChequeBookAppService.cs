using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IChequeBookAppService
    {
        ChequeBookDTO AddNewChequeBook(ChequeBookDTO chequeBookDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool UpdateChequeBook(ChequeBookDTO chequeBookDTO, ServiceHeader serviceHeader);

        bool PayVoucher(PaymentVoucherDTO paymentVoucherDTO, ServiceHeader serviceHeader);

        bool FlagVoucher(PaymentVoucherDTO paymentVoucherDTO, ServiceHeader serviceHeader);

        List<ChequeBookDTO> FindChequeBooks(ServiceHeader serviceHeader);

        PageCollectionInfo<ChequeBookDTO> FindChequeBooks(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<ChequeBookDTO> FindChequeBooks(int type, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        ChequeBookDTO FindChequeBook(Guid chequeBookId, ServiceHeader serviceHeader);

        List<PaymentVoucherDTO> FindPaymentVouchersByChequeBookId(Guid chequeBookId, ServiceHeader serviceHeader);

        List<PaymentVoucherDTO> FindPaymentVouchersByVoucherNumberAndChequeBookReference(int chequeBookType, int voucherNumber, string chequeBookReference, ServiceHeader serviceHeader);

        PageCollectionInfo<PaymentVoucherDTO> FindPaymentVouchersByChequeBookId(Guid chequeBookId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);
    }
}
