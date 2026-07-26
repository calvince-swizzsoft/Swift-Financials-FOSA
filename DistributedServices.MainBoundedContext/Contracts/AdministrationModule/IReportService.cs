using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IReportService
    {
        #region Report

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ReportDTO AddReport(ReportDTO reportDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateReport(ReportDTO reportDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ReportDTO> FindReports(bool updateDepth, bool traverseTree);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ReportDTO FindReport(Guid reportId);

        #endregion
    }
}
