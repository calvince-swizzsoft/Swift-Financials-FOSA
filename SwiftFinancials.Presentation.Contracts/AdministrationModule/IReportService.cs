using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AdministrationModule
{
    [ServiceContract(Name = "IReportService")]
    public interface IReportService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddReport(ReportDTO reportDTO, AsyncCallback callback, Object state);
        ReportDTO EndAddReport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateReport(ReportDTO reportDTO, AsyncCallback callback, Object state);
        bool EndUpdateReport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindReports(bool updateDepth, bool traverseTree, AsyncCallback callback, Object state);
        List<ReportDTO> EndFindReports(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindReport(Guid reportId, AsyncCallback callback, Object state);
        ReportDTO EndFindReport(IAsyncResult result);

        #region ReportDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNewReport(ReportDTO reportDTO, AsyncCallback callback, object state);
        ReportDTO EndAddNewReport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindReportFilterInPage(int startIndex, int pageSize, IList<string> sortedColumns, string searchString, bool sortAscending, AsyncCallback callback, object state);
        PageCollectionInfo<ReportDTO> EndFindReportFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDetailedReportFilterInPage(int startIndex, int pageSize, IList<string> sortedColumns, string searchString, bool sortAscending, AsyncCallback callback, object state);
        PageCollectionInfo<ReportDTO> EndFindDetailedReportFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindReportHeadersFilterInPage(int startIndex, int pageSize, IList<string> sortedColumns, string searchString, bool sortAscending, AsyncCallback callback, object state);
        PageCollectionInfo<ReportDTO> EndFindReportHeadersFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindReportById(Guid id, AsyncCallback callback, object state);
        ReportDTO EndFindReportById(IAsyncResult result);

        #endregion
    }
}
