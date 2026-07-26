using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.AttachedLoanAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanPurposeAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg
{
    public class LoanCase : Entity
    {
        public Guid? ParentId { get; set; }

        public virtual LoanCase Parent { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public Guid LoanProductId { get; set; }

        public virtual LoanProduct LoanProduct { get; private set; }

        public Guid? LoanPurposeId { get; set; }

        public virtual LoanPurpose LoanPurpose { get; private set; }

        public Guid? SavingsProductId { get; set; }

        public virtual SavingsProduct SavingsProduct { get; private set; }

        public int CaseNumber { get; set; }

        public string Remarks { get; set; }

        public decimal AmountApplied { get; set; }

        public DateTime ReceivedDate { get; set; }

        public string AppraisedBy { get; set; }

        public DateTime? AppraisedDate { get; set; }

        public string SystemAppraisalRemarks { get; set; }

        public decimal SystemAppraisedAmount { get; set; }

        public string AppraisalRemarks { get; set; }

        public decimal AppraisedAmount { get; set; }

        public string AppraisedAmountRemarks { get; set; }

        public decimal AppraisedNetIncome { get; set; }

        public decimal AppraisedAbility { get; set; }

        public decimal ApprovedAmount { get; set; }

        public string ApprovedAmountRemarks { get; set; }

        public decimal ApprovedPrincipalPayment { get; set; }

        public decimal ApprovedInterestPayment { get; set; }

        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string ApprovalRemarks { get; set; }

        public string AuditedBy { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuditRemarks { get; set; }

        public decimal AuditTopUpAmount { get; set; }

        public string CancelledBy { get; set; }

        public DateTime? CancelledDate { get; set; }

        public bool IsBatched { get; set; }

        public int BatchNumber { get; set; }

        public string BatchedBy { get; set; }

        public string DisbursementRemarks { get; set; }

        public string DisbursedBy { get; set; }

        public DateTime? DisbursedDate { get; set; }

        public decimal DisbursedAmount { get; set; }

        public decimal MonthlyPaybackAmount { get; set; }

        public decimal TotalPaybackAmount { get; set; }

        public int Status { get; set; }

        public decimal LoanProductInvestmentsBalance { get; set; }

        public decimal LoanProductLoanBalance { get; set; }

        public decimal TotalLoansBalance { get; set; }

        public decimal LoanProductLatestIncome { get; set; }

        public virtual LoanInterest LoanInterest { get; set; }

        public virtual LoanRegistration LoanRegistration { get; set; }

        public double MaximumAmountPercentage { get; set; }

        public virtual Charge TakeHome { get; set; }

        public string Reference { get; set; }

        HashSet<AttachedLoan> _attachedLoans;
        public virtual ICollection<AttachedLoan> AttachedLoans
        {
            get
            {
                if (_attachedLoans == null)
                {
                    _attachedLoans = new HashSet<AttachedLoan>();
                }
                return _attachedLoans;
            }
            private set
            {
                _attachedLoans = new HashSet<AttachedLoan>(value);
            }
        }
    }
}
