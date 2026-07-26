using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract(Name = "IPaymentService")]
    public interface IPaymentService
    {

        #region Payment

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PaymentDTO AddPayment(PaymentDTO paymentDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdatePayment(PaymentDTO paymentDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PaymentDTO> FindPayments();


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PaymentLineDTO> FindPaymentLines();


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO PostPayment(PaymentDTO paymentDTO, int moduleNavigationItemCode);


        //[OperationContract()]
        //[FaultContract(typeof(ApplicationServiceError))]

        //JournalDTO PayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode);




        #endregion
    }
}