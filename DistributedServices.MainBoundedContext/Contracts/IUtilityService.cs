using Application.MainBoundedContext.DTO;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IUtilityService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ApplicationDomainWrapper> FindApplicationDomainsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ConfigureApplicationDatabase();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ConfigureAspNetIdentityDatabase();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ConfigureAspNetMembershipDatabase();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool SeedEnumerations();
    }
}
