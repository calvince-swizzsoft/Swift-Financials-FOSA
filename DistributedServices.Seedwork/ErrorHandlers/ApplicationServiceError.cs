using System.Runtime.Serialization;

namespace DistributedServices.Seedwork.ErrorHandlers
{
    [DataContract(Name = "ServiceError", Namespace = "http://www.stamlinetechnologies.com")]
    public class ApplicationServiceError
    {
        [DataMember(Name = "ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}
