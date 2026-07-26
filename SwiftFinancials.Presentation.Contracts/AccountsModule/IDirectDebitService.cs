using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IDirectDebitService")]
    public interface IDirectDebitService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDirectDebit(DirectDebitDTO directDebitDTO, AsyncCallback callback, Object state);
        DirectDebitDTO EndAddDirectDebit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDirectDebit(DirectDebitDTO directDebitDTO, AsyncCallback callback, Object state);
        bool EndUpdateDirectDebit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDirectDebits(AsyncCallback callback, Object state);
        List<DirectDebitDTO> EndFindDirectDebits(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDirectDebitsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DirectDebitDTO> EndFindDirectDebitsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDirectDebitsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DirectDebitDTO> EndFindDirectDebitsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDirectDebit(Guid directDebitId, AsyncCallback callback, Object state);
        DirectDebitDTO EndFindDirectDebit(IAsyncResult result);
    }
}
