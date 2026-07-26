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
    public interface ICreditTypeService
    {
        #region Credit Type

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CreditTypeDTO AddCreditType(CreditTypeDTO creditTypeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCreditType(CreditTypeDTO creditTypeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CreditTypeDTO> FindCreditTypes();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CreditTypeDTO> FindCreditTypesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CreditTypeDTO> FindCreditTypesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CreditTypeDTO FindCreditType(Guid creditTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> FindCommissionsByCreditTypeId(Guid creditTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionsByCreditTypeId(Guid creditTypeId, List<CommissionDTO> commissions);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DirectDebitDTO> FindDirectDebitsByCreditTypeId(Guid creditTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDirectDebitsByCreditTypeId(Guid creditTypeId, List<DirectDebitDTO> directDebits);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ProductCollectionInfo FindAttachedProductsByCreditTypeId(Guid creditTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateAttachedProductsByCreditTypeId(Guid creditTypeId, ProductCollectionInfo attachedProductsTuple);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ProductCollectionInfo FindConcessionExemptProductsByCreditTypeId(Guid creditTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateConcessionExemptProductsByCreditTypeId(Guid creditTypeId, ProductCollectionInfo concessionExemptProductsTuple);

        #endregion
    }
}
