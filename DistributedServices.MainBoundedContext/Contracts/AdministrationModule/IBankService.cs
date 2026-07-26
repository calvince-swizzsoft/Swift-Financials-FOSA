using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IBankService
    {
        #region Bank

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BankDTO AddBank(BankDTO bankDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateBank(BankDTO bankDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BankDTO> FindBanks();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BankDTO FindBank(Guid bankId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BankDTO> FindBanksInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BankDTO> FindBanksByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BankBranchDTO> FindBankBranchesByBankId(Guid bankId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateBankBranchesByBankId(Guid bankId, List<BankBranchDTO> bankBranches);

        #endregion
    }
}
