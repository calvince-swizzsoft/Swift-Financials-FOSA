using System;
using System.Collections.Generic;
namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
 public class LoanNoticeRequest {public Guid LoanCaseId{get;set;} public Guid RecipientCustomerId{get;set;} public DateTime AsAt{get;set;} public string NoticeType{get;set;} public string BasisHash{get;set;}}
 public class LoanNoticeCandidate : LoanNoticeRequest
 {
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
  public string CreatedBy{get;set;} public string ApprovedBy{get;set;} public DateTime? ApprovedAtUtc{get;set;} public string CancelledBy{get;set;} public DateTime? CancelledAtUtc{get;set;}
 }
 public class LoanNoticePage {public int Total{get;set;} public List<LoanNoticeDTO> Items{get;set;}=new List<LoanNoticeDTO>();}
 public class LoanNoticeContact
 {
  public Guid LoanCaseId{get;set;} public Guid CustomerId{get;set;} public string Name{get;set;}
  public Guid? CompanyId{get;set;} public string CompanyName{get;set;} public string PolicyJson{get;set;} public int PolicyRevision{get;set;}
 }
}
