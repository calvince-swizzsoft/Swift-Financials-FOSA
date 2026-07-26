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
    public interface IPurchaseCreditMemoService
    {

        #region Credit Memo

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PurchaseCreditMemoDTO AddPurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdatePurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PurchaseCreditMemoDTO> FindPurchaseCreditMemos();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO PostPurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO, int moduleNavigationItemCode);

        #endregion
    }
}
