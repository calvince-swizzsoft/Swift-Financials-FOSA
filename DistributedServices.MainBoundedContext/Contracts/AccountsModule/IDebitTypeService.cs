using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
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
    public interface IDebitTypeService
    {
        #region Debit Type

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DebitTypeDTO AddDebitType(DebitTypeDTO debitTypeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDebitType(DebitTypeDTO debitTypeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DebitTypeDTO> FindDebitTypes();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DebitTypeDTO> FindMandatoryDebitTypes(bool isMandatory);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DebitTypeDTO> FindDebitTypesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DebitTypeDTO> FindDebitTypesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DebitTypeDTO FindDebitType(Guid debitTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> FindCommissionsByDebitTypeId(Guid debitTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionsByDebitTypeId(Guid debitTypeId, List<CommissionDTO> commissions);

        #endregion
    }
}
