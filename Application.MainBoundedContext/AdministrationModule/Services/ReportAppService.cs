using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.ReportAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public class ReportAppService : IReportAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Report> _reportRepository;

        public ReportAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Report> reportRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (reportRepository == null)
                throw new ArgumentNullException(nameof(reportRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _reportRepository = reportRepository;
        }

        public ReportDTO AddNewReport(ReportDTO reportDTO, ServiceHeader serviceHeader)
        {
            if (reportDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    // Create account type from factory and set persistent id
                    var report = ReportFactory.CreateReport(reportDTO.ParentId, reportDTO.ReportName, reportDTO.ReportPath, reportDTO.ReportQuery, reportDTO.Category);

                    _reportRepository.Add(report, serviceHeader);

                    // Save entity
                    dbContextScope.SaveChanges(serviceHeader);

                    // Return dto
                    return report.ProjectedAs<ReportDTO>();
                }
            }
            else return null;
        }

        public bool UpdateReport(ReportDTO reportDTO, ServiceHeader serviceHeader)
        {
            if (reportDTO == null || reportDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _reportRepository.Get(reportDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = ReportFactory.CreateReport(reportDTO.ParentId, reportDTO.ReportName, reportDTO.ReportPath, reportDTO.ReportQuery, reportDTO.Category);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    _reportRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<ReportDTO> FindReports(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ReportSpecifications.ParentReports();

                ISpecification<Report> spec = filter;

                var reports = _reportRepository.AllMatching(spec, serviceHeader, c => c.Children);

                if (reports != null && reports.Any())
                {
                    return reports.ProjectedAsCollection<ReportDTO>();
                }
                else return null;
            }
        }

        public List<ReportDTO> FindReports(bool updateDepth, bool traverseTree, ServiceHeader serviceHeader)
        {
            var reports = FindReports(serviceHeader);

            if (reports != null && reports.Any())
            {
                var reportDTOsList = new List<ReportDTO>();

                Action<ReportDTO> traverse = null;

                /* recursive lambda */
                traverse = (node) =>
                {
                    int depth = 0;

                    var tempNode = node;
                    while (tempNode.Parent != null)
                    {
                        tempNode = tempNode.Parent;
                        depth++;
                    }

                    var reportDTO = new ReportDTO();
                    reportDTO.Id = node.Id;
                    reportDTO.ParentId = node.ParentId;
                    reportDTO.ReportName = node.ReportName;
                    reportDTO.ReportPath = node.ReportPath;
                    reportDTO.ReportQuery = node.ReportQuery;
                    reportDTO.Category = node.Category;
                    reportDTO.Depth = depth;
                    reportDTO.CreatedDate = node.CreatedDate;

                    reportDTOsList.Add(reportDTO);

                    if (node.Children != null)
                    {
                        foreach (var item in node.Children)
                        {
                            traverse(item);
                        }
                    }
                };

                if (traverseTree)
                {
                    foreach (var c in reports)
                    {
                        traverse(c);
                    }

                    #region update depth
                    // TODO:
                    //if (updateDepth && reportDTOsList.Any())
                    //{
                    //    reportDTOsList.ForEach(item =>
                    //    {
                    //        if (item != null)
                    //        {
                    //            var report = _reportRepository.Get(item.Id);

                    //            if (report != null)
                    //                report.Depth = item.Depth;
                    //        }
                    //    });

                    //    dbContextScope.SaveChanges(serviceHeader);
                    //}

                    #endregion
                }
                else
                {
                    reportDTOsList = reports;
                }

                return reportDTOsList;
            }
            else return null;
        }

        public ReportDTO FindReport(Guid reportId, ServiceHeader serviceHeader)
        {
            if (reportId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var report = _reportRepository.Get(reportId, serviceHeader);

                    if (report != null)
                    {
                        return report.ProjectedAs<ReportDTO>();
                    }
                    else
                        return null;
                }
            }
            else
                return null;
        }
    }
}
