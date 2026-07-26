using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.FrontOfficeModule
{
    [ServiceContract(Name = "ISuperSaverPayableService")]
    public interface ISuperSaverPayableService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNewSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, AsyncCallback callback, Object state);
        SuperSaverPayableDTO EndAddNewSuperSaverPayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSuperSaverPayable(Guid superSaverPayableId, AsyncCallback callback, Object state);
        SuperSaverPayableDTO EndFindSuperSaverPayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSuperSaverPayablesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SuperSaverPayableDTO> EndFindSuperSaverPayablesByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuditOption, AsyncCallback callback, Object state);
        bool EndAuditSuperSaverPayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuthorizeSuperSaverPayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerSuperSaverPayable(Guid customerId, AsyncCallback callback, Object state);
        SuperSaverInterestDTO EndFindCustomerSuperSaverPayable(IAsyncResult result);
    }
}
