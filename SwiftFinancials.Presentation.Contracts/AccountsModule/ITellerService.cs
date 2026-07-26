using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "ITellerService")]
    public interface ITellerService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddTeller(TellerDTO tellerDTO, AsyncCallback callback, Object state);
        TellerDTO EndAddTeller(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateTeller(TellerDTO tellerDTO, AsyncCallback callback, Object state);
        bool EndUpdateTeller(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTellers(AsyncCallback callback, Object state);
        List<TellerDTO> EndFindTellers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTeller(Guid tellerId, bool includeBalance, AsyncCallback callback, Object state);
        TellerDTO EndFindTeller(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTellersInPage(int pageIndex, int pageSize, bool includeBalances, AsyncCallback callback, Object state);
        PageCollectionInfo<TellerDTO> EndFindTellersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTellersByFilterInPage(int tellerType, string text, int pageIndex, int pageSize, bool includeBalances, AsyncCallback callback, Object state);
        PageCollectionInfo<TellerDTO> EndFindTellersByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTellersByType(int tellerType, string reference, bool includeBalances, AsyncCallback callback, Object state);
        List<TellerDTO> EndFindTellersByType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTellersByReference(string reference, bool includeBalances, AsyncCallback callback, Object state);
        List<TellerDTO> EndFindTellersByReference(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTellerByEmployeeId(Guid employeeId, bool includeBalance, AsyncCallback callback, Object state);
        TellerDTO EndFindTellerByEmployeeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginComputeTellerCashTariffs(CustomerAccountDTO customerAccountDTO, decimal totalValue, int frontOfficeTransactionType, AsyncCallback callback, Object state);
        List<TariffWrapper> EndComputeTellerCashTariffs(IAsyncResult result);
    }
}
