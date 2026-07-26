using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IChequeTypeService")]
    public interface IChequeTypeService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddChequeType(ChequeTypeDTO chequeTypeDTO, AsyncCallback callback, Object state);
        ChequeTypeDTO EndAddChequeType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateChequeType(ChequeTypeDTO chequeTypeDTO, AsyncCallback callback, Object state);
        bool EndUpdateChequeType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChequeTypes(AsyncCallback callback, Object state);
        List<ChequeTypeDTO> EndFindChequeTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChequeType(Guid chequeTypeId, AsyncCallback callback, Object state);
        ChequeTypeDTO EndFindChequeType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChequeTypesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ChequeTypeDTO> EndFindChequeTypesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChequeTypesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ChequeTypeDTO> EndFindChequeTypesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsByChequeTypeId(Guid chequeTypeId, AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissionsByChequeTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionsByChequeTypeId(Guid chequeTypeId, List<CommissionDTO> commissions, AsyncCallback callback, Object state);
        bool EndUpdateCommissionsByChequeTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAttachedProductsByChequeTypeId(Guid chequeTypeId, AsyncCallback callback, Object state);
        ProductCollectionInfo EndFindAttachedProductsByChequeTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAttachedProductsByChequeTypeId(Guid chequeTypeId, ProductCollectionInfo attachedProductsTuple, AsyncCallback callback, Object state);
        bool EndUpdateAttachedProductsByChequeTypeId(IAsyncResult result);
    }
}
