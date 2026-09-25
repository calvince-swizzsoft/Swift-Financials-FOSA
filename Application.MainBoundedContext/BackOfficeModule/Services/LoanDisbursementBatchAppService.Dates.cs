using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchAgg;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public sealed class LoanDisbursementDateException : InvalidOperationException
    {
        public LoanDisbursementDateException(string message) : base(message) { }
    }
    public static class LoanDisbursementDates
    {
        public static PostingPeriodDTO Period(DateTime date, DateTime today, IEnumerable<PostingPeriodDTO> periods)
        {
            if (date.Year < 1900 || date.Date > today.Date)
                throw new LoanDisbursementDateException("Select an effective disbursement date between 1900 and today.");
            var matches = periods.Where(p => p.DurationStartDate.Date <= date.Date && p.DurationEndDate.Date >= date.Date).ToList();
            if (matches.Count != 1 || matches[0].IsClosed || matches[0].IsLocked)
                throw new LoanDisbursementDateException("The effective disbursement date must belong to exactly one open, unlocked posting period.");
            return matches[0];
        }
        public static void Loan(DateTime date, DateTime received, DateTime? approved)
        {
            if (date.Date < received.Date)
                throw new LoanDisbursementDateException("The effective disbursement date cannot precede the loan application date.");
            if (!approved.HasValue || date.Date < approved.Value.Date)
                throw new LoanDisbursementDateException("The effective disbursement date cannot precede approval. For a historical insider loan, record its actual board approval date first.");
        }
    }
    public partial class LoanDisbursementBatchAppService
    {
        PostingPeriodDTO ResolveDisbursementPeriod(DateTime date, ServiceHeader h)
        {
            return LoanDisbursementDates.Period(date, DateTime.Today, _postingPeriodAppService.FindPostingPeriods(h));
        }
        void ValidateLoanDate(LoanCase loan, DateTime date, ServiceHeader h)
        {
            var payload = _loanCaseRepository.DatabaseSqlQuery<string>(
                "SELECT TOP 1 Payload FROM dbo.swiftFin_SasraInsiderRecords WHERE Kind='board' AND SubjectId=@Id ORDER BY Revision DESC",
                h, new SqlParameter("@Id", loan.Id)).FirstOrDefault();
            var board = payload == null ? null : JsonConvert.DeserializeObject<InsiderDecisionDTO>(payload);
            // A recorded approval is business evidence; processing timestamps remain unchanged.
            var approved = board != null && board.Decision == "Approved" ? (DateTime?)board.DecisionDate : loan.ApprovedDate;
            LoanDisbursementDates.Loan(date, loan.ReceivedDate, approved);
        }
        void ValidateBatchDates(LoanDisbursementBatch batch, ServiceHeader h, DateTime? proposedDate = null)
        {
            var date = (proposedDate ?? batch.EffectiveDisbursementDate ?? DateTime.Today).Date;
            ResolveDisbursementPeriod(date, h);
            var entries = _loanDisbursementBatchEntryRepository.AllMatching(
                new DirectSpecification<Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchEntryAgg.LoanDisbursementBatchEntry>(e => e.LoanDisbursementBatchId == batch.Id), h).ToList();
            foreach (var entry in entries)
            {
                var loan = _loanCaseRepository.Get(entry.LoanCaseId, h);
                if (loan == null) throw new LoanDisbursementDateException("A batch loan could not be found. Reload the batch.");
                ValidateLoanDate(loan, date, h);
            }
            batch.EffectiveDisbursementDate = date;
        }
    }
}
