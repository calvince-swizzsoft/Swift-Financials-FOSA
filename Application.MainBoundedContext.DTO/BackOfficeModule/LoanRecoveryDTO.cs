using System;
using System.Collections.Generic;
namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
 public class LoanRecoveryRequest { public List<Guid> SelectedAccountIds{get;set;} public Guid LoanCaseId{get;set;} public Guid RequestId{get;set;} public string BasisHash{get;set;} }
 public class LoanRecoveryAccount
 {
  public Guid Id{get;set;} public Guid ChartOfAccountId{get;set;} public string Product{get;set;}
  public bool Selected{get;set;}
  public decimal Available{get;set;} public decimal Amount{get;set;}
 }
 public class LoanRecoveryMember
 {
  public Guid CustomerId{get;set;} public Guid? GuarantorId{get;set;} public string Name{get;set;}
  public decimal Guaranteed{get;set;} public decimal RemainingGuarantee{get;set;} public decimal OtherCommitments{get;set;}
  public decimal Available{get;set;} public decimal Amount{get;set;}
  public List<LoanRecoveryAccount> Accounts{get;set;}=new List<LoanRecoveryAccount>();
 }
 public class LoanRecoveryPreview
 {
  public Guid LoanCaseId{get;set;} public int CaseNumber{get;set;} public DateTime AsAt{get;set;}
  public Guid LoanAccountId{get;set;} public Guid BranchId{get;set;} public Guid PrincipalGlId{get;set;} public Guid InterestGlId{get;set;} public Guid PostingPeriodId{get;set;}
  public decimal PrincipalDue{get;set;} public decimal InterestDue{get;set;} public decimal UnpostedInterest{get;set;}
  public decimal TotalDue{get{return PrincipalDue+InterestDue;}}
  public decimal TotalRecovery{get;set;} public decimal PrincipalRecovery{get;set;} public decimal InterestRecovery{get;set;} public decimal Shortfall{get;set;}
  public LoanRecoveryMember Borrower{get;set;} public List<LoanRecoveryMember> Guarantors{get;set;}=new List<LoanRecoveryMember>();
  public string BasisHash{get;set;} public string EligibilityHash{get;set;}
 }
 public class LoanRecoveryReceipt {public Guid Id{get;set;} public Guid LoanCaseId{get;set;} public decimal Total{get;set;} public DateTime PostedAt{get;set;} }
}
