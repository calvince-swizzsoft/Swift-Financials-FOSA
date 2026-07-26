using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AdministrationModule
{
    [ServiceContract(Name = "IBankService")]
    public interface IBankService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddBank(BankDTO bankDTO, AsyncCallback callback, Object state);
        BankDTO EndAddBank(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateBank(BankDTO bankDTO, AsyncCallback callback, Object state);
        bool EndUpdateBank(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBanks(AsyncCallback callback, Object state);
        List<BankDTO> EndFindBanks(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBank(Guid bankId, AsyncCallback callback, Object state);
        BankDTO EndFindBank(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBanksInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BankDTO> EndFindBanksInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBanksByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BankDTO> EndFindBanksByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBankBranchesByBankId(Guid bankId, AsyncCallback callback, Object state);
        List<BankBranchDTO> EndFindBankBranchesByBankId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateBankBranchesByBankId(Guid bankId, List<BankBranchDTO> bankBranches, AsyncCallback callback, Object state);
        bool EndUpdateBankBranchesByBankId(IAsyncResult result);
    }
}
