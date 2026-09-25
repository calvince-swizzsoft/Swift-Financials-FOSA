using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using Application.MainBoundedContext.Services;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO;
using Domain.Seedwork;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRepaymentPlanAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderHistoryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
static class LoanScheduleAtomicTests
{
 class Proxy:RealProxy{readonly Func<IMethodCallMessage,object> call;public Proxy(Type t,Func<IMethodCallMessage,object> f):base(t){call=f;}public override IMessage Invoke(IMessage msg){var c=(IMethodCallMessage)msg;try{return new ReturnMessage(call(c),null,0,c.LogicalCallContext,c);}catch(Exception e){return new ReturnMessage(e,c);}}}
 static T Stub<T>(Func<IMethodCallMessage,object> f){return (T)new Proxy(typeof(T),f).GetTransparentProxy();}
 static IRepository<T> Repo<T>(Action<T> add)where T:Entity{return Stub<IRepository<T>>(c=>{if(c.MethodName!="Add")throw new Exception("Unexpected repository call");add((T)c.Args[0]);return null;});}
 public static void Run()
 {
  int commits=0;bool fail=false;var writes=new List<string>();
  var scope=Stub<IDbContextScope>(c=>{if(c.MethodName=="SaveChanges"){commits++;writes.Add("commit");return 1;}return null;});
  var scopes=Stub<IDbContextScopeFactory>(c=>{if(c.MethodName!="CreateWithTransaction" || (System.Data.IsolationLevel)c.Args[0]!=System.Data.IsolationLevel.Serializable)throw new Exception("Expected serializable shared unit of work");return scope;});
  var service=new JournalEntryPostingService(scopes,Repo<Journal>(x=>writes.Add("journal")),Repo<CustomerAccountCarryForward>(x=>{}),Repo<StandingOrderHistory>(x=>{}),Repo<CustomerAccountArrearage>(x=>{}),Repo<LoanRepaymentPlan>(x=>{writes.Add("plan");if(fail)throw new InvalidOperationException("Simulated schedule write failure");}));
  var journal=new Journal();journal.GenerateNewIdentity();var h=new ServiceHeader{ApplicationUserName="test"};
  var plan=LoanAgeingEngine.CaptureDraft(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),journal.Id,new DateTime(2026,1,1),100,new[]{new AmortizationTableEntry{DueDate=new DateTime(2026,2,1),PrincipalPayment=100}},h);
  if(!service.BulkSaveLoanDisbursement(h,new List<Journal>{journal},plan)||commits!=1||string.Join(",",writes)!="journal,plan,commit")throw new Exception("Schedule and funding must share one save");
  fail=true;writes.Clear();try{service.BulkSaveLoanDisbursement(h,new List<Journal>{journal},plan);throw new Exception("Failure ignored");}catch(InvalidOperationException){if(commits!=1||writes.Contains("commit"))throw new Exception("Schedule failure must prevent commit");}
  writes.Clear();try{service.BulkSaveLoanDisbursement(h,new List<Journal>(),plan);throw new Exception("Missing journal accepted");}catch(InvalidOperationException){if(writes.Count!=0)throw new Exception("Invalid funding wrote data");}
  Console.WriteLine("PASS: schedule and disbursement share one unit of work; schedule failure prevents commit; missing funding rejected before mutation.");
 }
}
