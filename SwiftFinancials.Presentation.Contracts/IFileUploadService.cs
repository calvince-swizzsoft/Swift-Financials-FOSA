using Application.MainBoundedContext.DTO;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts
{
    [ServiceContract(Name = "IFileUploadService")]
    public interface IFileUploadService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFileUpload(FileData fileData, AsyncCallback callback, Object state);
        string EndFileUpload(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFileUploadDone(string filename, AsyncCallback callback, Object state);
        bool EndFileUploadDone(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPingNetwork(string hostNameOrAddress, AsyncCallback callback, Object state);
        bool EndPingNetwork(IAsyncResult result);
    }
}
