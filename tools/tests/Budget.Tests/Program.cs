using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
class VerifyBudget {
 class Stub:RealProxy {
  readonly decimal balance;
  public Stub(Type t,decimal b):base(t){balance=b;}
  public override IMessage Invoke(IMessage m){var c=(IMethodCallMessage)m; if(c.MethodName!="FindGlAccountBalance")return new ReturnMessage(new Exception("Unexpected dependency: "+c.MethodName),c); return new ReturnMessage(balance,null,0,c.LogicalCallContext,c);}
 }
 static void Check(ChartOfAccountType type,decimal amount,decimal movement,decimal expected){
  var ctor=typeof(BudgetAppService).GetConstructors().Single();
  var service=(BudgetAppService)ctor.Invoke(ctor.GetParameters().Select(p=>new Stub(p.ParameterType,movement).GetTransparentProxy()).ToArray());
  var entry=new BudgetEntryDTO{Type=(int)BudgetEntryType.IncomeOrExpense,ChartOfAccountId=Guid.NewGuid(),BudgetBranchId=Guid.NewGuid(),BudgetPostingPeriodId=Guid.NewGuid(),ChartOfAccountAccountType=(int)type,Amount=amount};
  service.FetchBudgetEntryBalances(new List<BudgetEntryDTO>{entry},new ServiceHeader());
  Console.WriteLine("{0}: budget={1}, signed ledger={2}, expected remaining={3}, actual remaining={4}",type,amount,movement,expected,entry.BudgetBalance);
  if(entry.BudgetBalance!=expected)throw new Exception("Incorrect budget remaining");
  if(entry.ActualToDate!=movement)throw new Exception("Raw signed actual was changed");
 }
 static int Main(){
  BudgetSaveChecks.Run();
  BudgetActualChecks.Run();
  var journal=new Journal{TotalValue=4000}; journal.GenerateNewIdentity();
  var debit=Guid.NewGuid(); var credit=Guid.NewGuid(); journal.PostDoubleEntries(debit,credit,new ServiceHeader());
  if(journal.JournalEntries.Single(e=>e.ChartOfAccountId==debit).Amount!=4000 || journal.JournalEntries.Single(e=>e.ChartOfAccountId==credit).Amount!=-4000)throw new Exception("Unexpected ledger signs");
  Check(ChartOfAccountType.Expense,15000,4000,11000);
  Check(ChartOfAccountType.Income,15000,-4000,11000);
  Check(ChartOfAccountType.Expense,15000,-1000,16000);
  Check(ChartOfAccountType.Income,15000,1000,16000);
  Check(ChartOfAccountType.Expense,15000,0,15000);
  Check(ChartOfAccountType.Income,15000,0,15000);
  Check(ChartOfAccountType.Expense,15000,15000,0);
  Check(ChartOfAccountType.Income,15000,-15000,0);
  Check(ChartOfAccountType.Expense,15000,18000,-3000);
  Check(ChartOfAccountType.Income,15000,-18000,-3000);
  Console.WriteLine("PASS: domain debit/credit convention and 10 budget cases, including refunds, zero actuals, exact allocation and exceeded budgets. No database writes.");return 0;
 }
}
