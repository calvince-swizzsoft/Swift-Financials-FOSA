using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IBankLinkageService
    {
        #region Bank Linkage

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BankLinkageDTO AddBankLinkage(BankLinkageDTO bankLinkageDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateBankLinkage(BankLinkageDTO bankLinkageDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BankLinkageDTO> FindBankLinkages();
        
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BankLinkageDTO> FindBankLinkagesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BankLinkageDTO> FindBankLinkagesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BankLinkageDTO FindBankLinkage(Guid bankLinkageId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BankLinkageDTO FindBankLinkageByBankAccountId(Guid bankAccountId);

        #endregion
    }
}
