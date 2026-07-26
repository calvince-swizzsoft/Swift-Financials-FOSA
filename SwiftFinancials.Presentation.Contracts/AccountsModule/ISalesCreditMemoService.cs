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
    [ServiceContract(Name = "ISalesCreditMemoService")]
    public interface ISalesCreditMemoService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO, AsyncCallback callback, Object state);
        SalesCreditMemoDTO EndAddSalesCreditMemo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO, AsyncCallback callback, Object state);
        bool EndUpdateSalesCreditMemo(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalesCreditMemos(AsyncCallback callback, Object state);
        List<SalesCreditMemoDTO> EndFindSalesCreditMemos(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        JournalDTO EndPostSalesCreditMemo(IAsyncResult result);
    }
}
