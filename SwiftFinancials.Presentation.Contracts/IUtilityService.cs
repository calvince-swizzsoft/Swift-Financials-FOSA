using Application.MainBoundedContext.DTO;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts
{
    [ServiceContract(Name = "IUtilityService")]
    public interface IUtilityService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindApplicationDomainsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ApplicationDomainWrapper> EndFindApplicationDomainsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginConfigureApplicationDatabase(AsyncCallback callback, Object state);
        bool EndConfigureApplicationDatabase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginConfigureAspNetIdentityDatabase(AsyncCallback callback, Object state);
        bool EndConfigureAspNetIdentityDatabase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginConfigureAspNetMembershipDatabase(AsyncCallback callback, Object state);
        bool EndConfigureAspNetMembershipDatabase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginSeedEnumerations(AsyncCallback callback, Object state);
        bool EndSeedEnumerations(IAsyncResult result);
    }
}
