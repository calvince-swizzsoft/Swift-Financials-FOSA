using System;
using Domain.Seedwork;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRecoveryAgg
{
 public class LoanRecovery : Entity
 {
  public Guid LoanCaseId{get;set;} public virtual LoanCase LoanCase{get;set;}
  public Guid RequestId{get;set;} public string BasisHash{get;set;}
  public decimal Total{get;set;} public string SnapshotJson{get;set;} public string JournalIdsJson{get;set;}
 }
}
