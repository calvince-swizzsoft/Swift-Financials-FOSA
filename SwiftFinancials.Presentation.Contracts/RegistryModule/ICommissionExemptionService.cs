using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "ICommissionExemptionService")]
    public interface ICommissionExemptionService
    {
        #region Commission Exemption

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCommissionExemption(CommissionExemptionDTO commissionExemptionDTO, AsyncCallback callback, Object state);
        CommissionExemptionDTO EndAddCommissionExemption(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionExemption(CommissionExemptionDTO commissionExemptionDTO, AsyncCallback callback, Object state);
        bool EndUpdateCommissionExemption(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCommissionExemptionEntry(CommissionExemptionEntryDTO commissionExemptionEntryDTO, AsyncCallback callback, Object state);
        CommissionExemptionEntryDTO EndAddCommissionExemptionEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveCommissionExemptionEntries(List<CommissionExemptionEntryDTO> commissionExemptionEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveCommissionExemptionEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionExemptionEntryCollection(Guid commissionExemptionId, List<CommissionExemptionEntryDTO> commissionExemptionEntryCollection, AsyncCallback callback, Object state);
        bool EndUpdateCommissionExemptionEntryCollection(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionExemptions(AsyncCallback callback, Object state);
        List<CommissionExemptionDTO> EndFindCommissionExemptions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionExemptionsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CommissionExemptionDTO> EndFindCommissionExemptionsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionExemption(Guid commissionExemptionId, AsyncCallback callback, Object state);
        CommissionExemptionDTO EndFindCommissionExemption(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionExemptionEntriesByCommissionExemptionId(Guid commissionExemptionId, AsyncCallback callback, Object state);
        List<CommissionExemptionEntryDTO> EndFindCommissionExemptionEntriesByCommissionExemptionId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionExemptionEntriesByCommissionExemptionIdInPage(Guid commissionExemptionId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CommissionExemptionEntryDTO> EndFindCommissionExemptionEntriesByCommissionExemptionIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionExemptionEntriesByCustomerId(Guid customerId, AsyncCallback callback, Object state);
        List<CommissionExemptionEntryDTO> EndFindCommissionExemptionEntriesByCustomerId(IAsyncResult result);

        #endregion
    }
}
