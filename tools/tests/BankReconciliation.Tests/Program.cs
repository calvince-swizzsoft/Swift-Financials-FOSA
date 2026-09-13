using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Data.SqlClient;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;

class Program
{
    static int checks;
    static void Check(bool ok, string message) { if (!ok) throw new Exception(message); checks++; }
    static T Proxy<T>(Func<IMethodCallMessage, object> call) { return (T)new Stub(typeof(T), call).GetTransparentProxy(); }
    sealed class Stub : RealProxy
    {
        readonly Func<IMethodCallMessage, object> handler;
        public Stub(Type type, Func<IMethodCallMessage, object> handler) : base(type) { this.handler = handler; }
        public override IMessage Invoke(IMessage msg)
        {
            var call = (IMethodCallMessage)msg;
            try { return new ReturnMessage(handler(call), null, 0, call.LogicalCallContext, call); }
            catch (Exception e) { return new ReturnMessage(e, call); }
        }
    }
    sealed class Mapping : ITypeAdapter, ITypeAdapterFactory
    {
        public ITypeAdapter Create() { return this; }
        public T Adapt<S,T>(S source) where S : class where T : class,new() { return Adapt<T>(source); }
        public T Adapt<T>(object source) where T : class,new()
        {
            if (source is IEnumerable<BankReconciliationEntry>)
                return (T)(object)((IEnumerable<BankReconciliationEntry>)source).Select(e => new BankReconciliationEntryDTO { Id=e.Id, AdjustmentType=e.AdjustmentType, Value=e.Value, ChartOfAccountId=e.ChartOfAccountId, Remarks=e.Remarks }).ToList();
            throw new Exception("Unexpected mapping " + typeof(T));
        }
    }
    sealed class Fixture
    {
        public readonly Guid Bank = Guid.NewGuid(), Contra = Guid.NewGuid(), Branch = Guid.NewGuid();
        public readonly DateTime End = new DateTime(2026,9,11);
        public readonly ServiceHeader Header = new ServiceHeader { ApplicationUserName="reconciliation-test" };
        public readonly List<BankReconciliationEntry> Entries = new List<BankReconciliationEntry>();
        public BankReconciliationPeriod Period;
        public BankReconciliationPeriodAppService Service;
        public List<Journal> Journals = new List<Journal>();
        public decimal Ledger = 1000;
        public int Commits, Transactions;
        public bool FailPosting;
        public Fixture(decimal statement)
        {
            Period = BankReconciliationPeriodFactory.CreateBankReconciliationPeriod(Branch, Guid.NewGuid(), Guid.NewGuid(), Bank, "test-bank", new Duration(End.AddDays(-10), End), statement, 99999, "test");
            Period.Status=1;
            var scope = Proxy<IDbContextScope>(c => { if (c.MethodName=="SaveChanges") { Commits++; Check(Period.Status!=1,"Status set before outer commit"); return 1; } return null; });
            var read = Proxy<IDbContextReadOnlyScope>(c => null);
            var factory = Proxy<IDbContextScopeFactory>(c => {
                if (c.MethodName=="CreateReadOnly") return read;
                Check(c.MethodName=="CreateWithTransaction", "Mutation uses a transaction");
                Check((System.Data.IsolationLevel)c.Args[0]==System.Data.IsolationLevel.Serializable,"Serializable isolation"); Transactions++; return scope;
            });
            var periods = Proxy<IRepository<BankReconciliationPeriod>>(c => {
                if (c.MethodName=="Get") return Period;
                if (c.MethodName=="DatabaseSqlQuery") {
                    var sql=(string)c.Args[0]; var ps=((object[])c.Args[2]).Cast<SqlParameter>().ToList();
                    Check(sql.Contains("COALESCE(e.ValueDate,e.CreatedDate)<@End"),"Effective dates, not creation dates alone");
                    Check((DateTime)ps.Single(p=>p.ParameterName=="@End").Value==End.AddDays(1),"Includes the full end date, excludes next day");
                    Check((Guid)ps.Single(p=>p.ParameterName=="@Branch").Value==Branch && (Guid)ps.Single(p=>p.ParameterName=="@Account").Value==Bank,"Correct branch and bank ledger");
                    return new[]{Ledger};
                } throw new Exception(c.MethodName);
            });
            var entries = Proxy<IRepository<BankReconciliationEntry>>(c => { Check(c.MethodName=="AllMatching","Totals use all entries"); return Entries; });
            var accounts = Proxy<IRepository<ChartOfAccount>>(c => c.MethodName=="Get" ? (object)new ChartOfAccount() : 0);
            var ctor=typeof(JournalEntryPostingService).GetConstructors().Single();
            var realPosting=(JournalEntryPostingService)ctor.Invoke(ctor.GetParameters().Select(p => new Stub(p.ParameterType,c=>{throw new Exception("Unexpected dependency");}).GetTransparentProxy()).ToArray());
            var posting = Proxy<IJournalEntryPostingService>(c => {
                if(c.MethodName=="PerformDoubleEntry") { realPosting.PerformDoubleEntry((Journal)c.Args[0],(Guid)c.Args[1],(Guid)c.Args[2],(ServiceHeader)c.Args[3]); return null; }
                Check(c.MethodName=="BulkSave","Expected journal save");
                Check(Period.Status==1 && Commits==0,"Journal save runs before closing and commit");
                Journals=(List<Journal>)c.Args[1]; return !FailPosting;
            });
            Service = new BankReconciliationPeriodAppService(factory,periods,entries,posting,Proxy<ISqlCommandAppService>(c=>null),Proxy<IBankLinkageAppService>(c=>new BankLinkageDTO{ BranchId=Branch,ChartOfAccountId=Bank }),accounts);
        }
        public void Add(int type, decimal amount, bool contra=true)
        {
            var e=new BankReconciliationEntry { AdjustmentType=type,Value=amount,ChartOfAccountId=contra?(Guid?)Contra:null,Remarks="test" }; e.GenerateNewIdentity(); Entries.Add(e);
        }
        public bool Close() { return Service.CloseBankReconciliationPeriod(new BankReconciliationPeriodDTO {Id=Period.Id},1,123,Header); }
        public void RejectClose() { try { Close(); throw new Exception("Invalid close accepted"); } catch(BankReconciliationValidationException) { Check(Period.Status==1 && Commits==0,"Failed close stays open without commit"); } }
    }
    static int Main()
    {
        try {
            TypeAdapterFactory.SetCurrent(new Mapping());
            foreach(var type in new[]{0,1,2,3})
            {
                var f=new Fixture(type==2?1050:type==3?950:type==0?950:1050);
                var created=BankReconciliationEntryFactory.CreateBankReconciliationEntry(f.Period.Id,f.Contra,type,50,"CHK-1","Test drawee",f.End,"Factory regression");
                Check(created.ChartOfAccountId==(type>=2?(Guid?)f.Contra:null),"Factory preserves contra only for G/L adjustments");
                Check(created.ChequeNumber=="CHK-1"&&created.ChequeDrawee=="Test drawee"&&created.ChequeDate==f.End,"Factory retains optional cheque details for every type");
                f.Entries.Add(created);Check(f.Close(),"Factory-created adjustment closes successfully");
                if(type>=2)Check(f.Journals.Single().JournalEntries.Any(x=>x.ChartOfAccountId==f.Contra),"Saved contra reaches posted journal");
                else Check(f.Journals.Count==0,"Timing difference creates no journal");
            }
            var charge=new Fixture(950); charge.Add(3,50); Check(charge.Close(),"Bank charge reconciles");
            var journal=charge.Journals.Single(); var lines=journal.JournalEntries.ToList();
            Check(lines.Single(e=>e.ChartOfAccountId==charge.Bank).Amount==-50,"Bank charge credits bank");
            Check(lines.Single(e=>e.ChartOfAccountId==charge.Contra).Amount==50,"Bank charge debits expense");
            Check(lines.Sum(e=>e.Amount)==0,"Journal balances");
            Check(journal.ValueDate==charge.End && lines.All(e=>e.ValueDate==charge.End),"Journal and entries use selected end date, not month end");
            Check(charge.Period.GeneralLedgerAccountBalance==1000,"Refresh stale ledger snapshot before closing");
            Check(!charge.Close() && charge.Commits==1,"Second close cannot post twice");
            var income=new Fixture(1050); income.Add(2,50); income.Close();
            Check(income.Journals.Single().JournalEntries.Single(e=>e.ChartOfAccountId==income.Bank).Amount==50,"GL debit increases bank");
            var timing=new Fixture(900); timing.Add(0,150,false); timing.Add(1,50,false); timing.Close();
            Check(timing.Journals.Count==0 && timing.Period.Status==2,"Bank-side timing differences close without posting");
            var many=new Fixture(1205); for(int i=0;i<205;i++) many.Add(2,1);
            var summary = new BankReconciliationPeriodDTO {Id=many.Period.Id,Status=1,BranchId=many.Branch,ChartOfAccountId=many.Bank,DurationEndDate=many.End,BankAccountBalance=1205};
            typeof(BankReconciliationPeriodAppService).GetMethod("WithSummary",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(many.Service,new object[]{summary,many.Header});
            Check(summary.EntryCount==205 && summary.GeneralLedgerAdjustments==205 && summary.UnreconciledBalance==0,"Summary includes adjustments beyond first 100");
            many.Close();
            Check(many.Journals.Count==205,"All 205 adjustments included and posted");
            var failed=new Fixture(950){FailPosting=true}; failed.Add(3,50); failed.RejectClose();
            var mismatch=new Fixture(950); mismatch.RejectClose();
            var invalid=new Fixture(950); invalid.Add(3,50,false); invalid.RejectClose();
            var negative=new Fixture(1050); negative.Add(3,-50); negative.RejectClose();
            var unknown=new Fixture(1000); unknown.Add(4,50); unknown.RejectClose();
            var same=new Fixture(950); same.Add(3,50); same.Entries[0].ChartOfAccountId=same.Bank; same.RejectClose();
            Console.WriteLine("PASS: " + checks + " bank reconciliation regression assertions (no database writes)."); return 0;
        } catch(Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
