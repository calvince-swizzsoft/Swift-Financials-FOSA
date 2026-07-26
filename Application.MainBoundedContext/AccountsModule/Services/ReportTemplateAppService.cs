using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class ReportTemplateAppService : IReportTemplateAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ReportTemplate> _reportTemplateRepository;
        private readonly IRepository<ReportTemplateEntry> _reportTemplateEntryRepository;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public ReportTemplateAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<ReportTemplate> reportTemplateRepository,
           IRepository<ReportTemplateEntry> reportTemplateEntryRepository,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (reportTemplateRepository == null)
                throw new ArgumentNullException(nameof(reportTemplateRepository));

            if (reportTemplateEntryRepository == null)
                throw new ArgumentNullException(nameof(reportTemplateEntryRepository));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _reportTemplateRepository = reportTemplateRepository;
            _reportTemplateEntryRepository = reportTemplateEntryRepository;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public ReportTemplateDTO AddNewReportTemplate(ReportTemplateDTO reportTemplateDTO, ServiceHeader serviceHeader)
        {
            if (reportTemplateDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var reportTemplate = ReportTemplateFactory.CreateReportTemplate(reportTemplateDTO.ParentId, reportTemplateDTO.Description, reportTemplateDTO.Category, reportTemplateDTO.SpreadsheetCellReference);

                    if (reportTemplateDTO.IsLocked)
                        reportTemplate.Lock();
                    else reportTemplate.UnLock();

                    _reportTemplateRepository.Add(reportTemplate, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return reportTemplate.ProjectedAs<ReportTemplateDTO>();
                }
            }
            else return null;
        }

        public bool UpdateReportTemplate(ReportTemplateDTO reportTemplateDTO, ServiceHeader serviceHeader)
        {
            if (reportTemplateDTO == null || reportTemplateDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _reportTemplateRepository.Get(reportTemplateDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = ReportTemplateFactory.CreateReportTemplate(reportTemplateDTO.ParentId, reportTemplateDTO.Description, reportTemplateDTO.Category, reportTemplateDTO.SpreadsheetCellReference);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);


                    if (reportTemplateDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _reportTemplateRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<ReportTemplateDTO> FindReportTemplates(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ReportTemplateSpecifications.ParentReportTemplates();

                ISpecification<ReportTemplate> spec = filter;

                var reportTemplates = _reportTemplateRepository.AllMatching(spec, serviceHeader, c => c.Children);

                if (reportTemplates != null && reportTemplates.Any())
                {
                    return reportTemplates.ProjectedAsCollection<ReportTemplateDTO>();
                }
                else return null;
            }
        }

        public List<ReportTemplateDTO> FindReportTemplates(ServiceHeader serviceHeader, bool updateDepth, bool traverseTree)
        {
            var reportTemplates = FindReportTemplates(serviceHeader);

            if (reportTemplates != null && reportTemplates.Any())
            {
                var reportTemplateDTOsList = new List<ReportTemplateDTO>();

                Action<ReportTemplateDTO> traverse = null;

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

                    var reportTemplateDTO = new ReportTemplateDTO();
                    reportTemplateDTO.Id = node.Id;
                    reportTemplateDTO.ParentId = node.ParentId;
                    reportTemplateDTO.Description = node.Description;
                    reportTemplateDTO.Category = node.Category;
                    reportTemplateDTO.SpreadsheetCellReference = node.SpreadsheetCellReference;
                    reportTemplateDTO.Depth = depth;
                    reportTemplateDTO.IsLocked = node.IsLocked;
                    reportTemplateDTO.CreatedDate = node.CreatedDate;

                    reportTemplateDTOsList.Add(reportTemplateDTO);

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
                    foreach (var c in reportTemplates)
                    {
                        traverse(c);
                    }

                    #region update depth
                    // TODO:
                    //if (updateDepth && reportTemplateDTOsList.Any())
                    //{
                    //    reportTemplateDTOsList.ForEach(item =>
                    //    {
                    //        if (item != null)
                    //        {
                    //            var reportTemplate = _reportTemplateRepository.Get(item.Id);

                    //            if (reportTemplate != null)
                    //                reportTemplate.Depth = item.Depth;
                    //        }
                    //    });
                    //}

                    #endregion
                }
                else
                {
                    reportTemplateDTOsList = reportTemplates;
                }

                return reportTemplateDTOsList;
            }
            else return null;
        }

        public List<ReportTemplateDTO> FindReportTemplateBalances(Guid rootTemplateId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader)
        {
            if (rootTemplateId != Guid.Empty)
            {
                var reportTemplate = FindReportTemplate(rootTemplateId, serviceHeader);

                if (reportTemplate != null)
                {
                    var reportTemplateDTOsList = new List<ReportTemplateDTO>();

                    Action<ReportTemplateDTO> traverse = null;

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

                        var reportTemplateDTO = new ReportTemplateDTO();
                        reportTemplateDTO.Id = node.Id;
                        reportTemplateDTO.ParentId = node.ParentId;
                        reportTemplateDTO.Description = node.Description;
                        reportTemplateDTO.Category = node.Category;
                        reportTemplateDTO.SpreadsheetCellReference = node.SpreadsheetCellReference;
                        reportTemplateDTO.Depth = depth;
                        reportTemplateDTO.IsLocked = node.IsLocked;
                        reportTemplateDTO.CreatedDate = node.CreatedDate;

                        reportTemplateDTOsList.Add(reportTemplateDTO);

                        if (node.Children != null)
                        {
                            foreach (var item in node.Children)
                            {
                                traverse(item);
                            }
                        }
                    };

                    traverse(reportTemplate);

                    reportTemplateDTOsList.ForEach(reportTemplateDTO =>
                    {
                        var reportTemplateEntries = FindReportTemplateEntries(reportTemplateDTO.Id, serviceHeader);

                        if (reportTemplateEntries != null && reportTemplateEntries.Any())
                        {
                            foreach (var entry in reportTemplateEntries)
                            {
                                // fetch balances as at cut-off date 
                                var balance = _sqlCommandAppService.FindGlAccountBalance(entry.ChartOfAccountId, cutOffDate, transactionDateFilter, serviceHeader);

                                reportTemplateDTO.BookBalance += balance;
                            }
                        }
                    });

                    return reportTemplateDTOsList;
                }
                else return null;
            }
            else return null;
        }

        public ReportTemplateDTO PopulateReportTemplate(ReportTemplateDTO reportTemplateDTO, List<ReportTemplateDTO> templateBalances, string fileUploadDirectory, ServiceHeader serviceHeader)
        {
            if (reportTemplateDTO != null && !string.IsNullOrWhiteSpace(reportTemplateDTO.TemplateFileName) && templateBalances != null && !string.IsNullOrWhiteSpace(fileUploadDirectory))
            {
                var path = Path.Combine(fileUploadDirectory, reportTemplateDTO.TemplateFileName);

                if (System.IO.File.Exists(path))
                {
                    using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        HSSFWorkbook hssfWorkbook = new HSSFWorkbook(file);

                        if (hssfWorkbook != null)
                        {
                            ISheet sheet = hssfWorkbook.GetSheetAt(0);

                            if (sheet != null)
                            {
                                templateBalances.ForEach(item =>
                                {
                                    if (!string.IsNullOrEmpty(item.SpreadsheetCellReference))
                                    {
                                        var cellReferenceParts = item.SpreadsheetCellReference.Split(new char[] { ':' });

                                        if (cellReferenceParts != null && cellReferenceParts.Length == 2)
                                        {
                                            var rownum = -1;
                                            var cellnum = -1;

                                            int.TryParse(cellReferenceParts[0], out rownum);
                                            int.TryParse(cellReferenceParts[1], out cellnum);

                                            if (rownum != -1 && cellnum != -1)
                                            {
                                                var row = sheet.GetRow(rownum - 1/*because NPOI indexes from 0*/);

                                                if (row != null)
                                                {
                                                    var cell = row.GetCell(cellnum - 1/*because NPOI indexes from 0*/);

                                                    if (cell != null)
                                                    {
                                                        var value = (double)item.BookBalance;

                                                        // Insert data into template
                                                        cell.SetCellValue((value * -1 > 0d) ? value * -1 : value);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                });

                                // Force formulas to update with new data we added
                                sheet.ForceFormulaRecalculation = true;

                                // Save the NPOI workbook into a memory stream to be sent to the browser, could have saved to disk.
                                MemoryStream ms = new MemoryStream();
                                hssfWorkbook.Write(ms);

                                // Send the memory stream to the browser
                                reportTemplateDTO.TemplateFileBuffer = ms.GetBuffer();
                            }
                        }
                    }

                    return reportTemplateDTO;
                }
                else return null;
            }
            else return null;
        }

        public ReportTemplateDTO FindReportTemplate(Guid reportTemplateId, ServiceHeader serviceHeader)
        {
            if (reportTemplateId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var reportTemplate = _reportTemplateRepository.Get(reportTemplateId, serviceHeader);

                    if (reportTemplate != null)
                    {
                        return reportTemplate.ProjectedAs<ReportTemplateDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<ReportTemplateEntryDTO> FindReportTemplateEntries(Guid reportTemplateId, ServiceHeader serviceHeader)
        {
            if (reportTemplateId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ReportTemplateEntrySpecifications.ReportTemplateEntryWithReportTemplateId(reportTemplateId);

                    ISpecification<ReportTemplateEntry> spec = filter;

                    var reportTemplateEntries = _reportTemplateEntryRepository.AllMatching(spec, serviceHeader);

                    if (reportTemplateEntries != null && reportTemplateEntries.Any())
                    {
                        return reportTemplateEntries.ProjectedAsCollection<ReportTemplateEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateReportTemplateEntries(Guid reportTemplateId, List<ReportTemplateEntryDTO> reportTemplateEntries, ServiceHeader serviceHeader)
        {
            if (reportTemplateId != null && reportTemplateEntries != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _reportTemplateRepository.Get(reportTemplateId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = ReportTemplateEntrySpecifications.ReportTemplateEntryWithReportTemplateId(reportTemplateId);

                        ISpecification<ReportTemplateEntry> spec = filter;

                        var currentReportTemplateEntries = _reportTemplateEntryRepository.AllMatching(spec, serviceHeader);

                        if (currentReportTemplateEntries != null)
                        {
                            currentReportTemplateEntries.ToList().ForEach(x => _reportTemplateEntryRepository.Remove(x, serviceHeader));
                        }

                        if (reportTemplateEntries.Any())
                        {
                            foreach (var item in reportTemplateEntries)
                            {
                                var reportTemplateEntry = ReportTemplateEntryFactory.CreateReportTemplateEntry(persisted.Id, item.ChartOfAccountId);

                                _reportTemplateEntryRepository.Add(reportTemplateEntry, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }
    }
}
