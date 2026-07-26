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
    public interface ISalesCreditMemoService
    {

        #region Credit Memo

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalesCreditMemoDTO AddSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SalesCreditMemoDTO> FindSalesCreditMemos();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO PostSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO, int moduleNavigationItemCode);

        #endregion
    }
}
