using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IDebitTypeService")]
    public interface IDebitTypeService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDebitType(DebitTypeDTO debitTypeDTO, AsyncCallback callback, Object state);
        DebitTypeDTO EndAddDebitType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDebitType(DebitTypeDTO debitTypeDTO, AsyncCallback callback, Object state);
        bool EndUpdateDebitType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitTypes(AsyncCallback callback, Object state);
        List<DebitTypeDTO> EndFindDebitTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMandatoryDebitTypes(bool isMandatory,  AsyncCallback callback, Object state);
        List<DebitTypeDTO> EndFindMandatoryDebitTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitTypesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DebitTypeDTO> EndFindDebitTypesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitTypesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DebitTypeDTO> EndFindDebitTypesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDebitType(Guid debitTypeId, AsyncCallback callback, Object state);
        DebitTypeDTO EndFindDebitType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsByDebitTypeId(Guid debitTypeId, AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissionsByDebitTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionsByDebitTypeId(Guid debitTypeId, List<CommissionDTO> commissions, AsyncCallback callback, Object state);
        bool EndUpdateCommissionsByDebitTypeId(IAsyncResult result);
    }
}
