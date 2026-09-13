using System;
using System.Collections.Generic;
namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
 public class LoanAgeingException : Exception { public string Field {get;private set;} public int Status {get;private set;} public LoanAgeingException(string field,string message,int status=400):base(message){Field=field;Status=status;} }
 public class LoanScheduleProposalDTO
 {
  public LoanPlanDTO Plan{get;set;} public string Terms{get;set;} public string ProposalHash{get;set;} public bool CanConfirm{get;set;}
  public List<string> Warnings{get;set;}=new List<string>();
 }
 public class LoanScheduleConfirmationDTO {public Guid LoanCaseId{get;set;} public int Revision{get;set;} public string ProposalHash{get;set;} }
 public class LoanPlanInstalmentDTO { public decimal? Interest{get;set;} public DateTime? InterestDueDate{get;set;} public int Number{get;set;} public DateTime DueDate{get;set;} public decimal Principal{get;set;} }
 public class LoanPlanDTO
 {
  public bool IsRestructuring { get; set; }
        public DateTime? EffectiveAt { get; set; }
        public decimal? OpeningInterest { get; set; }
        public int? PriorRiskCategory { get; set; }
        public string PriorPlanIds { get; set; }
        public string OpeningLedgerHash { get; set; }
        public bool InterestTermsConfirmed{get;set;} public Guid? InterestReceivableChartOfAccountId{get;set;} public Guid? InterestChargedChartOfAccountId{get;set;}
  public Guid Id{get;set;} public Guid LoanCaseId{get;set;} public Guid CustomerAccountId{get;set;} public Guid PrincipalChartOfAccountId{get;set;} public Guid SourceJournalId{get;set;}
  public int CaseNumber{get;set;} public int Revision{get;set;} public bool IsConfirmed{get;set;} public DateTime DisbursementDate{get;set;} public decimal Principal{get;set;}
  public string Evidence{get;set;} public string AllocationPolicy{get;set;} public DateTime CreatedDate{get;set;} public string CreatedBy{get;set;}
  public List<LoanPlanInstalmentDTO> Instalments{get;set;}=new List<LoanPlanInstalmentDTO>();
 }
 public class LoanAgeingCaseDTO
 {
  public DateTime CreatedDate{get;set;} public Guid Id{get;set;} public int CaseNumber{get;set;} public string Product{get;set;} public Guid CustomerId{get;set;} public Guid LoanProductId{get;set;} public Guid BranchId{get;set;}
  public DateTime? DisbursedDate{get;set;} public decimal DisbursedAmount{get;set;} public int Status{get;set;} public int TermMonths{get;set;} public int Frequency{get;set;} public int CalculationMode{get;set;}
  public Guid? InterestReceivableChartOfAccountId{get;set;} public Guid? InterestChargedChartOfAccountId{get;set;}
  public Guid PrincipalChartOfAccountId{get;set;} public int PlanRevision{get;set;} public bool IsConfirmed{get;set;}
 }
 public class LoanAgeingCasePage {public int Total{get;set;} public List<LoanAgeingCaseDTO> Items{get;set;}=new List<LoanAgeingCaseDTO>();}
 public class LoanAgeingAccountLink {public Guid Id{get;set;} public Guid CustomerId{get;set;} public Guid ProductId{get;set;} public Guid BranchId{get;set;} }
 public class LoanAgeingPosting
 {
  public Guid Id{get;set;} public Guid JournalId{get;set;} public Guid? ParentJournalId{get;set;} public Guid? CustomerAccountId{get;set;} public Guid ChartOfAccountId{get;set;} public decimal Amount{get;set;}
  public Guid? ContraChartOfAccountId{get;set;} public DateTime EffectiveDate{get;set;} public int TransactionCode{get;set;} public string Reference{get;set;}
 }
 public class LoanAgeingInstalmentResult : LoanPlanInstalmentDTO {public int CaseNumber{get;set;} public Guid PlanId{get;set;} public decimal AllocatedPrincipal{get;set;} public decimal RemainingPrincipal{get;set;} public int DaysOverdue{get;set;} }
 public class LoanInterestInstalmentResult {public Guid PlanId{get;set;} public int CaseNumber{get;set;} public int Number{get;set;} public DateTime DueDate{get;set;} public decimal Interest{get;set;} public decimal AllocatedInterest{get;set;} public decimal RemainingInterest{get;set;} }
 public class LoanInterestAgeingResult
 {
  public decimal Receivable{get;set;} public decimal? ScheduledInterest{get;set;} public decimal? NetSettlements{get;set;} public decimal? OverdueInterest{get;set;} public decimal? UnaccruedDueInterest{get;set;} public int? DaysPastDue{get;set;} public string Bucket{get;set;}="Needs review";
  public List<string> Issues{get;set;}=new List<string>(); public List<LoanInterestInstalmentResult> Instalments{get;set;}=new List<LoanInterestInstalmentResult>();
 }
 public class LoanAgeingAccountResult
 {
  public bool IsRestructured{get;set;} public int? PriorRiskCategory{get;set;} public Guid? RestructuringPlanId{get;set;} public DateTime? RestructuredAt{get;set;} public LoanInterestAgeingResult Interest{get;set;} public int? CombinedDaysPastDue{get;set;} public string CombinedBucket{get;set;}="Needs review";
  public Guid CustomerAccountId{get;set;} public List<int> CaseNumbers{get;set;}=new List<int>(); public decimal OutstandingPrincipal{get;set;} public decimal? OverduePrincipal{get;set;} public int? DaysPastDue{get;set;} public DateTime? OldestUnpaidDueDate{get;set;} public string Bucket{get;set;}
  public List<string> Issues{get;set;}=new List<string>(); public List<LoanAgeingInstalmentResult> Instalments{get;set;}=new List<LoanAgeingInstalmentResult>();
 }
 public class LoanAgeingResult
 {
  public decimal AccountInterestReceivable{get;set;} public decimal LedgerInterestReceivable{get;set;} public decimal InterestDifference{get;set;} public decimal KnownOverdueInterest{get;set;} public int InterestAccountsRequiringReview{get;set;}
  public DateTime AsAt{get;set;} public DateTime GeneratedAtUtc{get;set;} public string Policy{get;set;} public int TotalAccounts{get;set;} public int AccountsRequiringReview{get;set;} public decimal LedgerPrincipal{get;set;} public decimal AccountPrincipal{get;set;} public decimal Difference{get;set;} public decimal KnownOverduePrincipal{get;set;}
  public List<string> Issues{get;set;}=new List<string>(); public List<string> Warnings{get;set;}=new List<string>(); public List<LoanAgeingAccountResult> Accounts{get;set;}=new List<LoanAgeingAccountResult>();
 }
}
