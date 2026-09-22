using System;
using System.Collections.Generic;
namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
 public class LoanNoticeLoanAction {public Guid LoanCaseId{get;set;} public string Stage{get;set;} public DateTime AsAt{get;set;} public string Action{get;set;} public string BasisHash{get;set;} public bool Retry{get;set;} public string DispatchReference{get;set;}}
 public class LoanNoticeLoanRow {
  public Guid LoanCaseId{get;set;} public int CaseNumber{get;set;} public string BorrowerName{get;set;} public string CompanyName{get;set;} public string Stage{get;set;} public DateTime AsAt{get;set;} public string Channel{get;set;} public decimal PrincipalOverdue{get;set;} public decimal InterestOverdue{get;set;} public int DaysPastDue{get;set;}
  public int RecipientCount{get;set;} public int SentCount{get;set;} public int QueuedCount{get;set;} public int FailedCount{get;set;} public bool RequireApproval{get;set;} public bool CanGenerate{get;set;} public bool HasPrepared{get;set;} public bool NeedsPreparation{get;set;} public bool CanApprove{get;set;} public bool CanSend{get;set;} public bool CanReset{get;set;} public string Status{get;set;} public string BlockingReason{get;set;} public string BasisHash{get;set;}
 }
 public class LoanNoticeLoanPage {public int Total{get;set;} public List<LoanNoticeLoanRow> Items{get;set;}=new List<LoanNoticeLoanRow>();public List<string> Issues{get;set;}=new List<string>();}
 public class LoanNoticeSendRequest {public string Destination{get;set;} public bool Retry{get;set;}}
 public class LoanNoticeDeliveryContact {public Guid? BranchId{get;set;} public string Email{get;set;} public string Mobile{get;set;}}
 public class LoanNoticeDispatchRequest {public string DispatchReference{get;set;}}
 public class LoanNoticeRequest {public Guid LoanCaseId{get;set;} public Guid RecipientCustomerId{get;set;} public DateTime AsAt{get;set;} public string NoticeType{get;set;} public string BasisHash{get;set;}}
 public class LoanNoticeCandidate : LoanNoticeRequest
 {
  public string WorkflowStage{get;set;} public bool CanGenerate{get;set;} public string BlockingReason{get;set;} public DateTime? EligibleAfter{get;set;}
  public List<Guid> ExpectedRecipientIds{get;set;}=new List<Guid>();
  public int CaseNumber{get;set;} public string BorrowerName{get;set;} public string RecipientName{get;set;}
  public Guid CompanyId{get;set;} public string CompanyName{get;set;} public int PolicyRevision{get;set;}
  public decimal PrincipalOverdue{get;set;} public decimal InterestOverdue{get;set;} public int DaysOverdue{get;set;}
  public string Channel{get;set;} public bool RequireApproval{get;set;} public string Template{get;set;}
  public string PolicyJson{get;set;} public int ResponseDays{get;set;} public Guid? ExistingNoticeId{get;set;}
 }
 public class LoanNoticeEligibility
 {
  public int Total{get;set;} public int ReviewCount{get;set;} public int DisabledCount{get;set;}
  public List<LoanNoticeCandidate> Items{get;set;}=new List<LoanNoticeCandidate>();
  public List<string> Issues{get;set;}=new List<string>();
 }
 public class LoanNoticeDTO
 {
  public Guid Id{get;set;} public Guid LoanCaseId{get;set;} public int CaseNumber{get;set;}
  public string BorrowerName{get;set;} public string RecipientName{get;set;} public string CompanyName{get;set;}
  public DateTime AsAt{get;set;} public DateTime ResponseDeadline{get;set;} public DateTime CreatedDate{get;set;}
  public string NoticeType{get;set;} public string Channel{get;set;} public string Status{get;set;} public string Body{get;set;}
  public int PolicyRevision{get;set;} public bool RequireApproval{get;set;} public decimal PrincipalOverdue{get;set;} public decimal InterestOverdue{get;set;} public int DaysOverdue{get;set;}
  public string SendDestination{get;set;} public string DeliveryAttemptsJson{get;set;}
  public Guid? MessageAlertId{get;set;} public string DeliveryDestination{get;set;} public string DeliveryStatus{get;set;}
  public string QueuedBy{get;set;} public DateTime? QueuedAtUtc{get;set;}
  public string SentBy{get;set;} public DateTime? SentAtUtc{get;set;} public string DispatchReference{get;set;}
  public string CreatedBy{get;set;} public string ApprovedBy{get;set;} public DateTime? ApprovedAtUtc{get;set;} public string CancelledBy{get;set;} public DateTime? CancelledAtUtc{get;set;}
 }
 public class LoanNoticePage {public int Total{get;set;} public List<LoanNoticeDTO> Items{get;set;}=new List<LoanNoticeDTO>();}
 public class LoanNoticeContact
 {
  public Guid LoanCaseId{get;set;} public Guid CustomerId{get;set;} public string Name{get;set;}
  public Guid? CompanyId{get;set;} public string CompanyName{get;set;} public string PolicyJson{get;set;} public int PolicyRevision{get;set;}
 }
}
