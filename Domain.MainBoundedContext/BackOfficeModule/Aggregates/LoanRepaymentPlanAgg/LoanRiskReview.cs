using System;
using Domain.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRepaymentPlanAgg
{
 public class LoanRiskReview:Entity
 {
  public Guid CustomerAccountId{get;set;} public virtual CustomerAccount CustomerAccount{get;set;}
  public DateTime AsAt{get;set;} public int Revision{get;set;} public int RiskCategory{get;set;}
  public decimal ProvisioningAdjustment{get;set;} public string Evidence{get;set;} public string BasisHash{get;set;}
 }
}
