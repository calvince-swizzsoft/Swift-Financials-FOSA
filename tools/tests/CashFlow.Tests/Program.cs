using System;
using System.Collections.Generic;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;

class Program
{
    static int checks;
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); checks++; }
    static CashFlowRowDTO Row(string section, string line, decimal receipts, decimal payments=0)
    { return new CashFlowRowDTO { Section=section,Line=line,Receipts=receipts,Payments=payments,DetailCount=1 }; }
    static CashFlowReportDTO Report(params CashFlowRowDTO[] flows)
    {
        var rows=new List<CashFlowRowDTO> { Row("Balance","Opening",100),Row("Balance","Closing",150) };
        rows.AddRange(flows);
        return CashFlowAppService.BuildReport(new DateTime(2026,9,1),new DateTime(2026,9,11),null,rows);
    }
    static void Main()
    {
        DomainChecks.Run(Check);
        RepositoryChecks.Run(Check);
        var report=Report(Row("Operating","Income",80),Row("Investing","Assets",0,40),Row("Financing","Borrowing",10));
        Check(report.IsComplete,"Fully classified reconciled report");
        Check(report.NetCashFlow==50 && report.ReconciliationDifference==0,"Three activity sections reconcile");
        report=Report(Row("Operating","Income",45),Row("Exchange","FX",5),Row("Internal","Transfer",200,200));
        Check(report.NetCashFlow==45 && report.ExchangeEffects==5,"Exchange effects separate from flows");
        Check(report.IsComplete,"Internal transfers do not change cash");
        report=Report(Row("Operating","Income",50),Row("Review","Mixed",100,100));
        Check(!report.IsComplete && report.UnclassifiedNet==0,"Zero-net review must block completeness");
        report=Report(Row("Operating","Income",30),Row("Review","Unknown",20));
        Check(!report.IsComplete && report.ReconciliationDifference==0 && report.UnclassifiedNet==20,"Review reconciles without claiming completion");
        report=Report(Row("Operating","Income",49.99m));
        Check(!report.IsComplete && report.ReconciliationDifference==0.01m,"A cent difference stays visible");
        Check(report.Rows.TrueForAll(r=>r.Section!="Balance"),"Balance records are not flow lines");
        Console.WriteLine(checks+" cash-flow domain, EF model and reconciliation assertions passed.");
    }
}
