using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
static class BudgetActualChecks
{
    public static void Run()
    {
        var a=Guid.NewGuid(); var unbudgeted=Guid.NewGuid(); var income=Guid.NewGuid(); var loan=Guid.NewGuid();
        var entries=new List<BudgetEntryDTO> {
            new BudgetEntryDTO{ Type=0, ChartOfAccountId=a, ChartOfAccountAccountType=5000, Amount=60 },
            new BudgetEntryDTO{ Type=0, ChartOfAccountId=a, ChartOfAccountAccountType=5000, Amount=40 },
            new BudgetEntryDTO{ Type=0, ChartOfAccountId=income, ChartOfAccountAccountType=4000, Amount=200 },
            new BudgetEntryDTO{ Type=1, LoanProductId=loan, Amount=500 }
        };
        var actuals=new List<BudgetActualLineDTO> {
            new BudgetActualLineDTO{TargetId=a,Section="Expenses",Actual=-10},
            new BudgetActualLineDTO{TargetId=income,Section="Income",Actual=250},
            new BudgetActualLineDTO{TargetId=unbudgeted,Section="Expenses",Actual=50},
            new BudgetActualLineDTO{TargetId=loan,Section="Loan disbursements",Actual=300}
        };
        var method=typeof(BudgetAppService).GetMethod("BuildBudgetActuals",BindingFlags.Static|BindingFlags.NonPublic);
        var report=(BudgetActualsDTO)method.Invoke(null,new object[]{new BudgetDTO(),new DateTime(2026,9,12),entries,actuals});
        var expense=report.Lines.Single(x=>x.TargetId==a);
        if(expense.Budget!=100 || expense.Actual!=-10 || expense.Difference!=110 || expense.Percentage!=-0.1m)throw new Exception("Repeated allocations/refund calculation");
        var missing=report.Lines.Single(x=>x.TargetId==unbudgeted);
        if(!missing.Unbudgeted || missing.Percentage!=null || missing.Difference!=-50)throw new Exception("Unbudgeted activity");
        var total=report.Totals.Single(x=>x.Section=="Expenses");
        if(total.Actual!=40 || total.Budget!=100 || total.Difference!=60)throw new Exception("Section totals double counted actuals");
        if(report.Lines.Single(x=>x.TargetId==income).Difference!=-50)throw new Exception("Income overachievement");
        if(report.Lines.Single(x=>x.TargetId==loan).Percentage!=0.6m)throw new Exception("Loan usage");
        var empty=(BudgetActualsDTO)method.Invoke(null,new object[]{new BudgetDTO(),DateTime.Today,new List<BudgetEntryDTO>(),new List<BudgetActualLineDTO>()});
        if(empty.Totals.Count!=3 || empty.Totals.Any(x=>x.Percentage!=null))throw new Exception("Empty section totals");
        Console.WriteLine("PASS: report grouping, refunds, income overachievement, loan usage, unbudgeted activity and section totals.");
    }
}
