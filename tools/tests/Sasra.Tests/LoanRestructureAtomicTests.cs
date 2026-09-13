using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
static class LoanRestructureAtomicTests
{
 class Proxy:RealProxy{readonly Func<IMethodCallMessage,object> fn;public Proxy(Type t,Func<IMethodCallMessage,object> f):base(t){fn=f;}public override IMessage Invoke(IMessage msg){var c=(IMethodCallMessage)msg;try{return new ReturnMessage(fn(c),null,0,c.LogicalCallContext,c);}catch(Exception e){return new ReturnMessage(e,c);}}}
 static object Stub(Type t,Func<IMethodCallMessage,object> f){return new Proxy(t,f).GetTransparentProxy();}
 public static void Run()
 {
  var writes=new List<string>();bool fail=false;int commits=0;
  var account=new CustomerAccountDTO{Id=Guid.NewGuid(),CustomerId=Guid.NewGuid(),CustomerAccountTypeTargetProductId=Guid.NewGuid(),PrincipalBalance=-100,InterestBalance=0};
  var product=new LoanProductDTO{Id=account.CustomerAccountTypeTargetProductId,ChartOfAccountId=Guid.NewGuid(),InterestReceivableChartOfAccountId=Guid.NewGuid(),InterestChargedChartOfAccountId=Guid.NewGuid(),LoanRegistrationTermInMonths=12,LoanRegistrationPaymentFrequencyPerYear=12};
  var existing=new LoanCase();existing.GenerateNewIdentity();
  var adapter=Stub(typeof(ITypeAdapter),c=>new List<LoanCaseDTO>{new LoanCaseDTO{Id=existing.Id,Status=48829}});
  TypeAdapterFactory.SetCurrent((ITypeAdapterFactory)Stub(typeof(ITypeAdapterFactory),c=>adapter));
  var scope=Stub(typeof(IDbContextScope),c=>{if(c.MethodName=="SaveChanges"){commits++;writes.Add("commit");return 1;}return null;});
  var read=Stub(typeof(IDbContextReadOnlyScope),c=>null);
  var ctor=typeof(LoanCaseAppService).GetConstructors().Single();
  var args=ctor.GetParameters().Select(p=>Stub(p.ParameterType,c=>{
   switch(p.Name)
   {
    case "dbContextScopeFactory":return c.MethodName=="CreateReadOnly"?read:scope;
    case "loanCaseRepository":if(c.MethodName=="AllMatching")return new List<LoanCase>{existing};if(c.MethodName=="DatabaseSqlQuery")return new[]{2};if(c.MethodName=="Add"){writes.Add("case");return null;}break;
    case "customerAccountAppService":if(c.MethodName=="FindCustomerAccountDTO")return account;if(c.MethodName=="FetchCustomerAccountBalances")return null;if(c.MethodName=="FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId")return new List<CustomerAccountDTO>{new CustomerAccountDTO{Id=Guid.NewGuid()}};break;
    case "chartOfAccountAppService":return Guid.NewGuid();
    case "savingsProductAppService":return new SavingsProductDTO{Id=Guid.NewGuid()};
    case "loanProductAppService":return product;
    case "journalAppService":writes.Add("journal");return new JournalDTO{Id=Guid.NewGuid(),CreatedDate=DateTime.Today.AddHours(12)};
    case "financialsService":return new List<AmortizationTableEntry>{new AmortizationTableEntry{DueDate=DateTime.Today.AddMonths(1),PrincipalPayment=100,InterestPayment=0}};
    case "standingOrderAppService":if(c.MethodName=="FindStandingOrders")return new List<StandingOrderDTO>{new StandingOrderDTO{Id=Guid.NewGuid()}};if(c.MethodName=="UpdateStandingOrder"){writes.Add("standing order");return true;}break;
    case "loanAgeingAppService":if(c.MethodName=="CaptureRestructureSchedule"){writes.Add("schedule");var plan=(LoanPlanDTO)c.Args[0];if(plan.Principal!=100||plan.SourceJournalId==Guid.Empty)throw new Exception("Invalid captured opening");if(fail)throw new LoanAgeingException("Schedule","Simulated capture failure");return null;}break;
   }
   throw new Exception("Unexpected restructure dependency "+p.Name+"."+c.MethodName);
  })).ToArray();
  var service=(LoanCaseAppService)ctor.Invoke(args);var h=new ServiceHeader{ApplicationUserName="test"};
  if(!service.RestructureLoan(Guid.NewGuid(),account.Id,1,100,"TEST",1,h)||commits!=1||string.Join(",",writes)!="case,journal,journal,standing order,schedule,commit")throw new Exception("Restructuring must stage all writes before one commit");
  writes.Clear();fail=true;try{service.RestructureLoan(Guid.NewGuid(),account.Id,1,100,"TEST",1,h);throw new Exception("Capture failure ignored");}catch(LoanAgeingException){if(commits!=1||writes.Contains("commit"))throw new Exception("Failed restructure capture committed");}
  Console.WriteLine("PASS: real restructuring AppService stages case, two journals, standing order and schedule before one commit; capture failure prevents commit.");
 }
}
