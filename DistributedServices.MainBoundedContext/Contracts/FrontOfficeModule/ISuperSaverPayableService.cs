using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ISuperSaverPayableService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SuperSaverPayableDTO AddNewSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SuperSaverPayableDTO FindSuperSaverPayable(Guid superSaverPayableId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SuperSaverPayableDTO> FindSuperSaverPayablesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuditOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SuperSaverInterestDTO FindCustomerSuperSaverPayable(Guid customerId);
    }
}
