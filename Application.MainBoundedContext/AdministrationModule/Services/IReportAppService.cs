using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public interface IReportAppService
    {
        ReportDTO AddNewReport(ReportDTO reportDTO, ServiceHeader serviceHeader);

        bool UpdateReport(ReportDTO reportDTO, ServiceHeader serviceHeader);

        List<ReportDTO> FindReports(ServiceHeader serviceHeader);

        List<ReportDTO> FindReports(bool updateDepth, bool traverseTree, ServiceHeader serviceHeader);

        ReportDTO FindReport(Guid reportId, ServiceHeader serviceHeader);
    }
}
