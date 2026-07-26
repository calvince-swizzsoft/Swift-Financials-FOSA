using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IFixedDepositTypeService
    {
        #region Fixed Deposit Type

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        FixedDepositTypeDTO AddFixedDepositType(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateFixedDepositType(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<FixedDepositTypeDTO> FindFixedDepositTypes();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        FixedDepositTypeDTO FindFixedDepositType(Guid fixedDepositTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<FixedDepositTypeDTO> FindFixedDepositTypesByMonths(int months);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FixedDepositTypeDTO> FindFixedDepositTypesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FixedDepositTypeDTO> FindFixedDepositTypesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LevyDTO> FindLeviesByFixedDepositTypeId(Guid fixedDepositTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLeviesByFixedDepositTypeId(Guid fixedDepositTypeId, List<LevyDTO> levies);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ProductCollectionInfo FindAttachedProductsByFixedDepositTypeId(Guid fixedDepositTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateAttachedProductsByFixedDepositTypeId(Guid fixedDepositTypeId, ProductCollectionInfo attachedProductsTuple);

        #endregion

        #region Fixed Deposit Type Graduated Scale

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<FixedDepositTypeGraduatedScaleDTO> FindGraduatedScalesByFixedDepositTypeId(Guid fixedDepositTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateGraduatedScalesByFixedDepositTypeId(Guid fixedDepositTypeId, List<FixedDepositTypeGraduatedScaleDTO> graduatedScales);

        #endregion
    }
}
