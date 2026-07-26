using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MicroCreditModule;
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
    public interface IMicroCreditOfficerService
    {
        #region Microcredit Officer

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MicroCreditOfficerDTO AddMicroCreditOfficer(MicroCreditOfficerDTO microCreditOfficerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateMicroCreditOfficer(MicroCreditOfficerDTO microCreditOfficerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<MicroCreditOfficerDTO> FindMicroCreditOfficers();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<MicroCreditOfficerDTO> FindMicroCreditOfficersInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<MicroCreditOfficerDTO> FindMicroCreditOfficersByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MicroCreditOfficerDTO FindMicroCreditOfficer(Guid microCreditOfficerId);

        #endregion
    }
}
