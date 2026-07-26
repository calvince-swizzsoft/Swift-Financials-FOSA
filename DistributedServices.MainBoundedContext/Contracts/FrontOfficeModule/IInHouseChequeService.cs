using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IInHouseChequeService
    {
        #region InHouse Cheque

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        InHouseChequeDTO AddInHouseCheque(InHouseChequeDTO inHouseChequeDTO, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddInHouseCheques(List<InHouseChequeDTO> inHouseChequeDTOs, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InHouseChequeDTO> FindInHouseChequesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InHouseChequeDTO> FindInHouseChequesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InHouseChequeDTO> FindInHouseChequesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<InHouseChequeDTO> FindUnPrintedInHouseChequesByBranchIdAndFilterInPage(Guid branchId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PrintInHouseCheque(InHouseChequeDTO inHouseChequeDTO, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode);

        #endregion
    }
}
