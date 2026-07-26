using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IIncomeAdjustmentService
    {
        #region Income Adjustment

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        IncomeAdjustmentDTO AddIncomeAdjustment(IncomeAdjustmentDTO incomeAdjustmentDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateIncomeAdjustment(IncomeAdjustmentDTO incomeAdjustmentDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<IncomeAdjustmentDTO> FindIncomeAdjustments();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        IncomeAdjustmentDTO FindIncomeAdjustment(Guid incomeAdjustmentId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<IncomeAdjustmentDTO> FindIncomeAdjustmentsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<IncomeAdjustmentDTO> FindIncomeAdjustmentsByFilterInPage(string text, int pageIndex, int pageSize);

        #endregion
    }
}
