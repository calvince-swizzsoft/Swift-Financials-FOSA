using Application.MainBoundedContext.DTO;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IPasswordManagerService
    {
        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        PasswordSettings GetSettings();

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ChangePasswordWithAnswer(string userName, string newPassword);
    }
}
