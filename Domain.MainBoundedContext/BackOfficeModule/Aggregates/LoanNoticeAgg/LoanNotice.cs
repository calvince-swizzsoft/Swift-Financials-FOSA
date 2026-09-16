using System;
using Domain.Seedwork;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanNoticeAgg
{
 public class LoanNotice : Entity
 {
  public Guid LoanCaseId {get;set;} public virtual LoanCase LoanCase {get;set;}
  public Guid CompanyId {get;set;} public virtual Company Company {get;set;}
  public Guid RecipientCustomerId {get;set;} public virtual Customer RecipientCustomer {get;set;}
  public string DuplicateKey {get;set;}
  public int CaseNumber {get;set;} public int PolicyRevision {get;set;}
  public DateTime AsAt {get;set;} public DateTime ResponseDeadline {get;set;}
  public string NoticeType {get;set;} public string Channel {get;set;}
  public string RecipientName {get;set;} public string BorrowerName {get;set;} public string CompanyName {get;set;}
  public decimal PrincipalOverdue {get;set;} public decimal InterestOverdue {get;set;} public int DaysOverdue {get;set;}
  public string Body {get;set;} public string PolicyJson {get;set;} public string SnapshotJson {get;set;}
  public bool RequireApproval {get;set;} public string Status {get;set;}
  public string ApprovedBy {get;set;} public DateTime? ApprovedAtUtc {get;set;}
  public string CancelledBy {get;set;} public DateTime? CancelledAtUtc {get;set;}
 }
}
