using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "ITreasuryService")]
    public interface ITreasuryService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddTreasury(TreasuryDTO treasuryDTO, AsyncCallback callback, Object state);
        TreasuryDTO EndAddTreasury(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateTreasury(TreasuryDTO treasuryDTO, AsyncCallback callback, Object state);
        bool EndUpdateTreasury(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTreasuries(bool includeBalances, AsyncCallback callback, Object state);
        List<TreasuryDTO> EndFindTreasuries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTreasury(Guid treasuryId, bool includeBalance, AsyncCallback callback, Object state);
        TreasuryDTO EndFindTreasury(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTreasuriesInPage(int pageIndex, int pageSize, bool includeBalances, AsyncCallback callback, Object state);
        PageCollectionInfo<TreasuryDTO> EndFindTreasuriesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTreasuriesByFilterInPage(string text, int pageIndex, int pageSize, bool includeBalances, AsyncCallback callback, Object state);
        PageCollectionInfo<TreasuryDTO> EndFindTreasuriesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTreasuryByBranchId(Guid branchId, bool includeBalance, AsyncCallback callback, Object state);
        TreasuryDTO EndFindTreasuryByBranchId(IAsyncResult result);
    }
}
