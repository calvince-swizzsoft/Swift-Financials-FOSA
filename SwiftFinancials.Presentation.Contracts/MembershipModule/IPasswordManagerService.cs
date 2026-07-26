using Application.MainBoundedContext.DTO;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.MembershipModule
{
    [ServiceContract(Name = "IPasswordManagerService")]
    public interface IPasswordManagerService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetSettings(AsyncCallback callback, Object state);
        PasswordSettings EndGetSettings(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginChangePasswordWithAnswer(string userName, string newPassword, AsyncCallback callback, Object state);
        bool EndChangePasswordWithAnswer(IAsyncResult result);
    }
}
