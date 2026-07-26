using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IBankLinkageService")]
    public interface IBankLinkageService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddBankLinkage(BankLinkageDTO bankLinkageDTO, AsyncCallback callback, Object state);
        BankLinkageDTO EndAddBankLinkage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateBankLinkage(BankLinkageDTO bankLinkageDTO, AsyncCallback callback, Object state);
        bool EndUpdateBankLinkage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankLinkages(AsyncCallback callback, Object state);
        List<BankLinkageDTO> EndFindBankLinkages(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankLinkagesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BankLinkageDTO> EndFindBankLinkagesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankLinkagesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BankLinkageDTO> EndFindBankLinkagesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankLinkage(Guid bankLinkageId, AsyncCallback callback, Object state);
        BankLinkageDTO EndFindBankLinkage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankLinkageByBankAccountId(Guid bankAccountId, AsyncCallback callback, Object state);
        BankLinkageDTO EndFindBankLinkageByBankAccountId(IAsyncResult result);
    }
}
