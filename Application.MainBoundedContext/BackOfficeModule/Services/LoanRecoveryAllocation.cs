using System;
using System.Linq;
using Application.MainBoundedContext.DTO.BackOfficeModule;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public static class LoanRecoveryAllocation
 {
  public static decimal Floor(decimal value){return Math.Floor(Math.Max(0,value)*100m)/100m;}
  public static void Allocate(LoanRecoveryPreview plan)
  {
   if(plan.PrincipalDue<0||plan.InterestDue<0||plan.Borrower==null)throw new LoanAgeingException("Recovery","Invalid recovery balance.",409);
   foreach(var member in new[]{plan.Borrower}.Concat(plan.Guarantors))
   {
    member.Amount=0;
    member.Available=Floor(member.Accounts.Where(a=>a.Selected).Sum(a=>Floor(a.Available))-Math.Max(0,member.OtherCommitments));
    foreach(var account in member.Accounts)account.Amount=0;
   }
   plan.Borrower.Amount=Math.Min(Floor(plan.TotalDue),plan.Borrower.Available);
   var left=Floor(plan.TotalDue-plan.Borrower.Amount);
   // Capped proportional allocation, redistributing only within remaining liability.
   while(left>0)
   {
    var active=plan.Guarantors.Where(g=>Math.Min(Floor(g.RemainingGuarantee),g.Available)>g.Amount).OrderBy(g=>g.CustomerId).ToList();
    if(active.Count==0)break;
    var weight=active.Sum(g=>g.RemainingGuarantee);var before=left;
    foreach(var g in active)
    {
     var room=Math.Min(Floor(g.RemainingGuarantee),g.Available)-g.Amount;
     var portion=Math.Min(room,Floor(before*g.RemainingGuarantee/weight));g.Amount+=portion;left-=portion;
    }
    if(left==before)
    {
     foreach(var g in active){if(left<=0)break;g.Amount+=0.01m;left-=0.01m;}
    }
   }
   foreach(var member in new[]{plan.Borrower}.Concat(plan.Guarantors))
   {
    var amount=member.Amount;
    foreach(var a in member.Accounts.Where(a=>a.Selected).OrderBy(a=>a.Id)){a.Amount=Math.Min(amount,Floor(a.Available));amount-=a.Amount;}
   }
   plan.TotalRecovery=plan.Borrower.Amount+plan.Guarantors.Sum(g=>g.Amount);
   plan.InterestRecovery=Math.Min(plan.InterestDue,plan.TotalRecovery);
   plan.PrincipalRecovery=plan.TotalRecovery-plan.InterestRecovery;
   plan.Shortfall=plan.TotalDue-plan.TotalRecovery;
  }
 }
}
