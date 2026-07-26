using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.FrontOfficeModule
{
    [ServiceContract(Name = "IFiscalCountService")]
    public interface IFiscalCountService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddFiscalCount(FiscalCountDTO fiscalCountDTO, AsyncCallback callback, Object state);
        FiscalCountDTO EndAddFiscalCount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFiscalCountsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FiscalCountDTO> EndFindFiscalCountsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFiscalCountsByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FiscalCountDTO> EndFindFiscalCountsByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginEndOfDayExecuted(EmployeeDTO employeeDTO, AsyncCallback callback, Object state);
        bool EndEndOfDayExecuted(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFiscalCount(Guid fiscalCountId, AsyncCallback callback, Object state);
        FiscalCountDTO EndFindFiscalCount(IAsyncResult result);        
    }
}
