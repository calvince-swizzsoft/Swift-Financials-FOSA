using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IFiscalCountService
    {
        #region Fiscal Count

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        FiscalCountDTO AddFiscalCount(FiscalCountDTO fiscalCountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FiscalCountDTO> FindFiscalCountsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<FiscalCountDTO> FindFiscalCountsByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> EndOfDayExecutedAsync(EmployeeDTO employeeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        FiscalCountDTO FindFiscalCount(Guid fiscalCountId);

        #endregion
    }
}
