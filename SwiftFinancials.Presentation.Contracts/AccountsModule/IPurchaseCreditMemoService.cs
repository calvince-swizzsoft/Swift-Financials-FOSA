using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IPurchaseCreditMemoService")]
    public interface IPurchaseCreditMemoService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddPurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO, AsyncCallback callback, Object state);
        PurchaseCreditMemoDTO EndAddPurchaseCreditMemo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdatePurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO, AsyncCallback callback, Object state);
        bool EndUpdatePurchaseCreditMemo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPurchaseCreditMemos(AsyncCallback callback, Object state);
        List<PurchaseCreditMemoDTO> EndFindPurchaseCreditMemos(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostPurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        JournalDTO EndPostPurchaseCreditMemo(IAsyncResult result);
    }
}
