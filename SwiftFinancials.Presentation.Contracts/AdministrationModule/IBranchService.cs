using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AdministrationModule
{
    [ServiceContract(Name = "IBranchService")]
    public interface IBranchService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddBranch(BranchDTO branchDTO, AsyncCallback callback, Object state);
        BranchDTO EndAddBranch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateBranch(BranchDTO branchDTO, AsyncCallback callback, Object state);
        bool EndUpdateBranch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBranches(AsyncCallback callback, Object state);
        List<BranchDTO> EndFindBranches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBranchesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BranchDTO> EndFindBranchesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBranchesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BranchDTO> EndFindBranchesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBranch(Guid branchId, AsyncCallback callback, Object state);
        BranchDTO EndFindBranch(IAsyncResult result);
        
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBranchByCode(int branchCode, AsyncCallback callback, Object state);
        BranchDTO EndFindBranchByCode(IAsyncResult result);
    }
}
