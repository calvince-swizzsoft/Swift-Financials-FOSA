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
    public interface IChequeTypeService
    {
        #region Cheque Type

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ChequeTypeDTO AddChequeType(ChequeTypeDTO chequeTypeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateChequeType(ChequeTypeDTO chequeTypeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ChequeTypeDTO> FindChequeTypes();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ChequeTypeDTO FindChequeType(Guid chequeTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ChequeTypeDTO> FindChequeTypesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ChequeTypeDTO> FindChequeTypesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> FindCommissionsByChequeTypeId(Guid chequeTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionsByChequeTypeId(Guid chequeTypeId, List<CommissionDTO> commissions);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ProductCollectionInfo FindAttachedProductsByChequeTypeId(Guid chequeTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateAttachedProductsByChequeTypeId(Guid chequeTypeId, ProductCollectionInfo attachedProductsTuple);

        #endregion
    }
}
