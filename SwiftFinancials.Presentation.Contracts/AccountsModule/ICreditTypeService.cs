using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "ICreditTypeService")]
    public interface ICreditTypeService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCreditType(CreditTypeDTO creditTypeDTO, AsyncCallback callback, Object state);
        CreditTypeDTO EndAddCreditType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCreditType(CreditTypeDTO creditTypeDTO, AsyncCallback callback, Object state);
        bool EndUpdateCreditType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditTypes(AsyncCallback callback, Object state);
        List<CreditTypeDTO> EndFindCreditTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditTypesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CreditTypeDTO> EndFindCreditTypesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditTypesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CreditTypeDTO> EndFindCreditTypesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditType(Guid creditTypeId, AsyncCallback callback, Object state);
        CreditTypeDTO EndFindCreditType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsByCreditTypeId(Guid creditTypeId, AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissionsByCreditTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionsByCreditTypeId(Guid creditTypeId, List<CommissionDTO> commissions, AsyncCallback callback, Object state);
        bool EndUpdateCommissionsByCreditTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDirectDebitsByCreditTypeId(Guid creditTypeId, AsyncCallback callback, Object state);
        List<DirectDebitDTO> EndFindDirectDebitsByCreditTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDirectDebitsByCreditTypeId(Guid creditTypeId, List<DirectDebitDTO> directDebits, AsyncCallback callback, Object state);
        bool EndUpdateDirectDebitsByCreditTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAttachedProductsByCreditTypeId(Guid creditTypeId, AsyncCallback callback, Object state);
        ProductCollectionInfo EndFindAttachedProductsByCreditTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAttachedProductsByCreditTypeId(Guid creditTypeId, ProductCollectionInfo attachedProductsTuple, AsyncCallback callback, Object state);
        bool EndUpdateAttachedProductsByCreditTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindConcessionExemptProductsByCreditTypeId(Guid creditTypeId, AsyncCallback callback, Object state);
        ProductCollectionInfo EndFindConcessionExemptProductsByCreditTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateConcessionExemptProductsByCreditTypeId(Guid creditTypeId, ProductCollectionInfo concessionExemptProductsTuple, AsyncCallback callback, Object state);
        bool EndUpdateConcessionExemptProductsByCreditTypeId(IAsyncResult result);
    }
}
