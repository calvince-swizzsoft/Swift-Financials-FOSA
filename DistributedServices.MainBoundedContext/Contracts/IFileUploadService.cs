using Application.MainBoundedContext.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IFileUploadService
    {
        [OperationContract]
        string FileUpload(FileData fileData);

        [OperationContract]
        bool FileUploadDone(string filename);

        [OperationContract]
        bool PingNetwork(string hostNameOrAddress);
    }
}
