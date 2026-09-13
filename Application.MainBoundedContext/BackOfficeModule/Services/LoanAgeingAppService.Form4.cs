using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.AccountsModule.Services;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRepaymentPlanAgg;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public partial class LoanAgeingAppService
 {
  static LoanRiskReviewDTO ReviewDto(LoanRiskReview r){return new LoanRiskReviewDTO{CustomerAccountId=r.CustomerAccountId,AsAt=r.AsAt,Revision=r.Revision,RiskCategory=r.RiskCategory,ProvisioningAdjustment=r.ProvisioningAdjustment,Evidence=r.Evidence,BasisHash=r.BasisHash};}
  public SasraForm4Result PreviewForm4(SasraForm4Request input,ServiceHeader h)
  {
   SasraForm4.Validate(input);var report=ReportCore(input.AsAt,null,0,100,null,h,true);
   using(scopes.CreateReadOnly()){var saved=riskReviews.AllMatching(new DirectSpecification<LoanRiskReview>(x=>x.AsAt==input.AsAt.Date),h).GroupBy(x=>x.CustomerAccountId).Select(g=>g.OrderByDescending(x=>x.Revision).First()).Select(ReviewDto).ToList();return SasraForm4.Build(input,report,saved);}
  }
  public LoanRiskReviewDTO SaveRiskReview(LoanRiskReviewDTO input,ServiceHeader h)
  {
   Check(input!=null,"Review","Provide the credit-quality review.");Check(input.AsAt.Year>=1753&&input.AsAt.Year<9999&&input.CustomerAccountId!=Guid.Empty,"AsAt","Select a valid date and loan account.");
   Check(input.RiskCategory>=0&&input.RiskCategory<=4,"RiskCategory","Select one of the five supported risk categories.");
   Check(input.ProvisioningAdjustment.HasValue&&Math.Abs(input.ProvisioningAdjustment.Value)<=1000000000000000m&&decimal.Round(input.ProvisioningAdjustment.Value,2)==input.ProvisioningAdjustment,"ProvisioningAdjustment","Enter a supported provisioning-basis adjustment with at most two decimals, or zero explicitly.");
   Check(!string.IsNullOrWhiteSpace(input.Evidence)&&input.Evidence.Length<=2000,"Evidence","Document the credit-quality review and the interest/suspense treatment supporting the adjustment, including zero (up to 2,000 characters).");
   var report=ReportCore(input.AsAt,null,0,100,input.CustomerAccountId,h,true);var account=report.Accounts.Single();var minimum=SasraForm4.Minimum(account,input.AsAt);
   Check(minimum.HasValue,"RiskCategory","Resolve this account's principal, interest and restructuring issues before saving its risk review.");
   Check(input.RiskCategory>=minimum.Value,"RiskCategory","The selected category cannot improve the ageing-based or retained pre-restructure classification.");
   Check(account.OutstandingPrincipal+input.ProvisioningAdjustment>=0,"ProvisioningAdjustment","The provisioning basis cannot be negative.");
   Check(input.BasisHash==SasraForm4.Basis(account),"BasisHash","The ageing basis changed. Refresh Form 4 and review the account again.",409);
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable))
   {
    var previous=riskReviews.AllMatching(new DirectSpecification<LoanRiskReview>(x=>x.CustomerAccountId==input.CustomerAccountId&&x.AsAt==input.AsAt.Date),h).OrderByDescending(x=>x.Revision).FirstOrDefault();
    Check(input.Revision==(previous?.Revision??0),"Revision","A newer risk-review revision exists. Reload before saving.",409);
    var row=new LoanRiskReview{CustomerAccountId=input.CustomerAccountId,AsAt=input.AsAt.Date,Revision=input.Revision+1,RiskCategory=input.RiskCategory,ProvisioningAdjustment=input.ProvisioningAdjustment.Value,Evidence=input.Evidence.Trim(),BasisHash=input.BasisHash,CreatedDate=DateTime.Now,CreatedBy=h.ApplicationUserName};row.GenerateNewIdentity();riskReviews.Add(row,h);scope.SaveChanges(h);return ReviewDto(row);
   }
  }
 }
}
