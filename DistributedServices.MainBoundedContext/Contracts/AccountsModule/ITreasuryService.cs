using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ITreasuryService
    {
        #region Treasury

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        TreasuryDTO AddTreasury(TreasuryDTO treasuryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateTreasury(TreasuryDTO treasuryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TreasuryDTO> FindTreasuries(bool includeBalances);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        TreasuryDTO FindTreasury(Guid treasuryId, bool includeBalance);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TreasuryDTO> FindTreasuriesInPage(int pageIndex, int pageSize, bool includeBalances);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TreasuryDTO> FindTreasuriesByFilterInPage(string text, int pageIndex, int pageSize, bool includeBalances);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        TreasuryDTO FindTreasuryByBranchId(Guid branchId, bool includeBalance);

        #endregion
    }
}
