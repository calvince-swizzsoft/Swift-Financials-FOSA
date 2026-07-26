using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IImprestService
    {

        #region Imprest

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ImprestDTO AddImprest(ImprestDTO imprestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateImprest(ImprestDTO imprestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ImprestDTO> FindImprests();


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ImprestLineDTO> FindImprestLines();


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO PostImprest(ImprestDTO imprestDTO, int moduleNavigationItemCode);


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO PostSurrender(ImprestDTO imprestDTO, int moduleNavigationItemCode);


        //[OperationContract()]
        //[FaultContract(typeof(ApplicationServiceError))]

        //JournalDTO PayImprest(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode);


        #endregion
    }
}