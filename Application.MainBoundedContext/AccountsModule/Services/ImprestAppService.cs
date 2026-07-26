using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ImprestAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevySplitAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;

using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using static iTextSharp.text.pdf.AcroFields;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class ImprestAppService : IImprestAppService
    {

        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Imprest> _imprestRepository;
        private readonly IRepository<ImprestLine> _imprestLineRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly INumberSeriesGenerator _numberSeriesGenerator;
        private readonly IJournalAppService _journalAppService;

        public ImprestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Imprest> imprestRepository,
           IRepository<ImprestLine> imprestLineRepository,
           IChartOfAccountAppService chartOfAccountAppService,
           INumberSeriesGenerator numberSeriesGenerator,
           IJournalAppService journalAppService
           )
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (imprestRepository == null)
                throw new ArgumentNullException(nameof(imprestRepository));

            if (imprestLineRepository == null)
                throw new ArgumentNullException(nameof(imprestLineRepository));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (numberSeriesGenerator == null)
                throw new ArgumentNullException(nameof(numberSeriesGenerator));

            _dbContextScopeFactory = dbContextScopeFactory;
            _imprestRepository = imprestRepository;
            _imprestLineRepository = imprestLineRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _numberSeriesGenerator = numberSeriesGenerator;
            _journalAppService = journalAppService;
        }



        public ImprestDTO AddNewImprest(ImprestDTO imprestDTO, ServiceHeader serviceHeader)
        {
            if (imprestDTO != null)
            {

                var imprestNo = _numberSeriesGenerator.GetNextNumber("IM", serviceHeader);

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {

                    var imprest = ImprestFactory.CreateImprest(imprestNo, imprestDTO.EmployeeNo, imprestDTO.EmployeeName, imprestDTO.Purpose, imprestDTO.AmountRequested, imprestDTO.RequestDate, imprestDTO.Posted, imprestDTO.BankChartOfAccountId, serviceHeader);

                    AddLines(imprestDTO, imprest, serviceHeader);

                    imprest.CreatedBy = serviceHeader.ApplicationUserName;

                    _imprestRepository.Add(imprest, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return imprest.ProjectedAs<ImprestDTO>();
                }
            }
            else return null;
        }

        public bool UpdateImprest(ImprestDTO imprestDTO, ServiceHeader serviceHeader)
        {
           

            if (imprestDTO == null || imprestDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _imprestRepository.Get(imprestDTO.Id, serviceHeader);

                if (persisted != null)
                {

                    var current = ImprestFactory.CreateImprest(persisted.No, imprestDTO.EmployeeNo, imprestDTO.EmployeeName, imprestDTO.Purpose,imprestDTO.AmountRequested, imprestDTO.RequestDate, imprestDTO.Posted, imprestDTO.BankChartOfAccountId, serviceHeader);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    _imprestRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }


        public void UpdateImprestLines(HashSet<ImprestLineDTO> imprestLines, ServiceHeader serviceHeader)
        {

            if (imprestLines != null)
            {
               
                    foreach (var item in imprestLines)
                    {
                        var persistedLine = _imprestLineRepository.Get(item.Id, serviceHeader);

                        if (persistedLine != null)
                        {

                      
                            //persistedLine.UpdateLine(item.LineNo, item.ExpenseCategory, item.Description, item.Amount, item.ImprestDebtorsChartOfAccountId, item.ExpenseChartOfAccountId);
                            //_imprestLineRepository.Update(persistedLine);
                            persistedLine.AmountSpent = item.AmountSpent;
                            persistedLine.AmountReturned = item.AmountReturned;
                            persistedLine.BankChartOfAccountId = item.BankChartOfAccountId;
                            persistedLine.ImprestDebtorsChartOfAccountId = item.ImprestDebtorsChartOfAccountId;
                            persistedLine.ExpensChartOfAccountId = item.ExpenseChartOfAccountId;

                      
                    }
                        else
                        {
                            throw new InvalidOperationException($"Imprest line with Id {item.Id} not found.");

                        }


                  
           

                }

            }
               
              
        }


        public void AddLines(ImprestDTO imprestDTO, Imprest imprest, ServiceHeader serviceHeader)
        {
            StringBuilder sbErrors = new StringBuilder();

            if (imprest == null || imprest.IsTransient())
                sbErrors.Append("Imprest is either null or in transient state! ");

            if (imprest.Id == null || imprest.Id == Guid.Empty)
                sbErrors.Append("Imprest Id is null or empty!");


            if (sbErrors.Length != 0)
                throw new InvalidOperationException(sbErrors.ToString());
            else
            {

                if (imprestDTO.ImprestLines != null && imprestDTO.ImprestLines.Any())
                {
                    foreach (var item in imprestDTO.ImprestLines)
                    {
                        imprest.AddLine(item.LineNo, item.ExpenseCategory, item.Description, item.Amount, item.ImprestDebtorsChartOfAccountId, imprestDTO.BankChartOfAccountId, item.ExpenseChartOfAccountId);

                    }

                }
            }
        }


        public List<ImprestDTO> FindImprests(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var imprests = _imprestRepository.GetAll(serviceHeader);
               
                if (imprests != null && imprests.Any())
                {
                    return imprests.ProjectedAsCollection<ImprestDTO>();
                }
                else return null;
            }
        }


        public ImprestDTO FindImprest (Guid imprestId, ServiceHeader serviceHeader)
        {

            using (_dbContextScopeFactory.CreateReadOnly())
            {

                var imprest = _imprestRepository.Get(imprestId, serviceHeader);

                if (imprest != null)
                {
                    return imprest.ProjectedAs<ImprestDTO>();
                }

                else
                {
                    return null;
                }
            }
        }


        public List<ImprestLineDTO> FindImprestLines(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var imprestLines = _imprestLineRepository.GetAll(serviceHeader);

                if (imprestLines != null && imprestLines.Any())
                {
                    return imprestLines.ProjectedAsCollection<ImprestLineDTO>();
                }
                else return null;
            }
        }



        public JournalDTO PostImprest(ImprestDTO imprestDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (imprestDTO == null || !imprestDTO.ImprestLines.Any())
            {
                throw new InvalidOperationException("Sorry, but the provided data is incorrect!");
            }

      
            var imprestLineDTOs = imprestDTO.ImprestLines;
            JournalDTO lastCreatedJournal = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // Fetch the persisted invoice once, outside the loop
                var persisted = _imprestRepository.Get(imprestDTO.Id, serviceHeader);
                if (persisted == null)
                {
                    throw new InvalidOperationException("imprest not found!");
                }

                // Process each purchase ice line
                foreach (var item in imprestLineDTOs)
                {
                    var journal = _journalAppService.AddNewJournal(
                        imprestDTO.BranchId,
                        null,
                        item.Amount,
                        string.Format("Imprest~{0}", item.LineNo),
                        imprestDTO.BankBranchName,
                        item.LineNo.ToString(),
                        moduleNavigationItemCode,
                        (int)SystemTransactionCode.GeneralLedger,
                        null,
                        imprestDTO.BankChartOfAccountId,
                        item.ImprestDebtorsChartOfAccountId,
                        serviceHeader);

                    if (journal == null)
                    {
                        throw new InvalidOperationException($"Failed to create journal for imprest line {item.LineNo}");
                    }

                    lastCreatedJournal = journal;
                }

                // Mark the imprest as posted in both DTO and persisted entity
                imprestDTO.Posted = true;
                persisted.Posted = true;

                // Save all changes at once
                if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                {
                    return lastCreatedJournal;
                }
                else
                {
                    throw new InvalidOperationException("Failed to save journal entries to database!");
                }
            }
        }


      
        public JournalDTO PostSurrender(ImprestDTO imprestDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (imprestDTO == null || !imprestDTO.ImprestLines.Any())
            {
                throw new InvalidOperationException("Sorry, but the provided data is incorrect!");
            }

            var imprestLineDTOs = imprestDTO.ImprestLines;
            JournalDTO lastCreatedJournal = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // Fetch the persisted invoice once, outside the loop
                var persisted = _imprestRepository.Get(imprestDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    foreach (var item in imprestLineDTOs)
                    {
                        var journal = _journalAppService.AddNewJournal(
                            imprestDTO.BranchId,
                            null,
                            item.AmountSpent,
                            string.Format("Surrender~{0}", item.LineNo),
                            imprestDTO.BankBranchName,
                            item.LineNo.ToString(),
                            moduleNavigationItemCode,
                            (int)SystemTransactionCode.GeneralLedger,
                            null,
                            item.ImprestDebtorsChartOfAccountId,
                            item.ExpenseChartOfAccountId,
                            serviceHeader);

                        if (journal == null)
                        {
                            throw new InvalidOperationException($"Failed to create journal for surrender of imprest line {item.LineNo}");
                        }

                        lastCreatedJournal = journal;
                    }

                    imprestDTO.Surrendered = true;
                    persisted.Surrendered = true;


                    //--> update imprest lines
                    UpdateImprestLines(imprestLineDTOs, serviceHeader);




                    // Save all changes at once
                    if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                    {
                        return lastCreatedJournal;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to save journal entries to database!");
                    }
                }

                else
                {
                    throw new InvalidOperationException("imprest not found!");
                }
            }
        }


    }
}
    