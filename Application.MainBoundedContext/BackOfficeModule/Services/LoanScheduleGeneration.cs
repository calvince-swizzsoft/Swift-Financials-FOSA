using System;
using System.Linq;
using System.Globalization;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public static class LoanScheduleGeneration
 {
  public static DateTime DueDate(DateTime disbursed,int grace,int frequency,int paymentDue,int index)
  {
   var anchor=disbursed.Date.AddDays(grace);int n=index+(paymentDue==0?1:0);
   if(frequency<=12)return anchor.AddMonths(n*(12/frequency));
   return anchor.AddDays(n*(frequency==24?15:frequency==26?14:frequency==52?7:1));
  }
  public static LoanScheduleProposalDTO Generate(LoanPlanDTO p,int term,int frequency,int grace,int paymentDue,int mode,double apr)
  {
   if(term<1||term>1200||!Enum.IsDefined(typeof(PaymentFrequencyPerYear),frequency)||grace<0||paymentDue<0||paymentDue>1||!Enum.IsDefined(typeof(InterestCalculationMode),mode)||double.IsNaN(apr)||double.IsInfinity(apr)||apr<0||apr>1000)
    throw new LoanAgeingException("LoanTerms","The saved loan has invalid or unsupported term, frequency, grace, payment timing or interest settings.");
   decimal count=term*(decimal)frequency/12;
   if(count<1||count>1200||decimal.Truncate(count)!=count)throw new LoanAgeingException("Instalments","The saved term and frequency do not define a whole number of instalments between 1 and 1,200. Review the contractual schedule.");
   if(p.DisbursementDate.Year<1753||p.DisbursementDate.Year>=9999||p.Principal<=0||p.Principal>1000000000000000m)throw new LoanAgeingException("Principal","A valid original disbursement date and positive posted principal are required.");
   try
   {
    // Reuse the established financial calculation. Its legacy Today-based dates are
    // deliberately discarded; all dates here come from this loan's saved settings.
    var rows=new FinancialsService().RepaymentSchedule(term,frequency,grace,mode,apr,-(double)p.Principal,0,paymentDue);
    if(rows.Count!=(int)count)throw new LoanAgeingException("Instalments","The saved terms produced an unexpected number of instalments.");
    p.Instalments=rows.Select((x,i)=>new LoanPlanInstalmentDTO{Number=i+1,DueDate=DueDate(p.DisbursementDate,grace,frequency,paymentDue,i),Principal=decimal.Round(x.PrincipalPayment,2,MidpointRounding.AwayFromZero),Interest=decimal.Round(x.InterestPayment,2,MidpointRounding.AwayFromZero),InterestDueDate=DueDate(p.DisbursementDate,grace,frequency,paymentDue,i)}).ToList();
    decimal residual=p.Principal-p.Instalments.Sum(x=>x.Principal);
    if(Math.Abs(residual)>.01m*(count+1))throw new LoanAgeingException("Principal","The saved terms do not amortize the posted principal. Review the payment timing and interest method.");
    p.Instalments.Last().Principal+=residual;
    if(p.Instalments.Any(x=>x.DueDate.Year>=9999||x.Principal<=0||x.Interest<0))throw new LoanAgeingException("Instalments","The saved terms produce invalid amounts or dates. Review the loan terms.");
    return new LoanScheduleProposalDTO{Plan=p,Terms=string.Format(CultureInfo.InvariantCulture,"Original principal KSh {0:0.00}; disbursed {1:yyyy-MM-dd}; term {2} months; {3} payments/year; grace {4} days; {5}; APR {6:0.####}%; {7}",p.Principal,p.DisbursementDate,term,frequency,grace,paymentDue==0?"end of period":"beginning of period",apr,(InterestCalculationMode)mode)};
   }
   catch(ArgumentException){throw new LoanAgeingException("LoanTerms","The saved terms produce unsupported dates or financial calculations. Review the term, rate and original disbursement date.");}
   catch(OverflowException){throw new LoanAgeingException("LoanTerms","The saved terms exceed the supported amount or date range.");}
  }
  public static void Upfront(LoanPlanDTO p,decimal total)
  {
   foreach(var row in p.Instalments){row.Interest=0;row.InterestDueDate=null;}
   p.Instalments[0].Interest=total;p.Instalments[0].InterestDueDate=p.DisbursementDate.Date;
  }
 }
}
