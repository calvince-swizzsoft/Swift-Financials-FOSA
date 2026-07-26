using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IBranchService
    {
        #region Branch

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BranchDTO AddBranch(BranchDTO branchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateBranchAsync(BranchDTO branchDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BranchDTO> FindBranches();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BranchDTO> FindBranchesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BranchDTO> FindBranchesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BranchDTO FindBranch(Guid branchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BranchDTO FindBranchByCode(int branchCode);

        #endregion
    }
}
