using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchEntryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
class Program
{
    static int checks;
    static void Check(bool value, string message) { if (!value) throw new Exception(message); checks++; }
    static int Main() { try { JournalChecks(); BatchChecks(); Console.WriteLine(checks + " reversal failure, retry and lifecycle checks passed."); return 0; } catch(Exception e) { Console.Error.WriteLine(e); return 1; } }
    static T Build<T>(Func<ParameterInfo, IMethodCallMessage, object> invoke) {
        var constructor = typeof(T).GetConstructors().Single();
        return (T)constructor.Invoke(constructor.GetParameters().Select(p => new Stub(p.ParameterType, c => invoke(p,c)).GetTransparentProxy()).ToArray());
    }
    static object Scope(IMethodCallMessage call, Action save, Action dispose) {
        return new Stub(((MethodInfo)call.MethodBase).ReturnType, c => {
            if (c.MethodName == "SaveChanges") { save(); return 1; }
            if (c.MethodName == "Dispose") { dispose(); return null; }
            throw new Exception("Unexpected scope call " + c.MethodName);
        }).GetTransparentProxy();
    }
    static void JournalChecks() {
        var header = new ServiceHeader { ApplicationUserName = "test" };
        var original = new Journal { TotalValue = 500, PostingPeriodId = Guid.NewGuid(), BranchId = Guid.NewGuid(), PrimaryDescription = "Fee", Reference = "TEST" };
        original.GenerateNewIdentity();
        original.JournalEntries.Add(new JournalEntry { Amount = 500, ChartOfAccountId = Guid.NewGuid(), CustomerAccountId = Guid.NewGuid() });
        original.JournalEntries.Add(new JournalEntry { Amount = -500, ChartOfAccountId = Guid.NewGuid() });
        bool bulkFailure = false, commitFailure = false, committed = false;
        int commits = 0;
        List<Journal> staged = null;
        var service = Build<JournalAppService>((p,c) => {
            if(c.MethodName == "Create") { committed = false; bool locked = original.IsLocked; return Scope(c, () => { if(commitFailure) throw new InvalidOperationException("simulated commit failure"); commits++; committed = true; }, () => { if(!committed && !locked) original.UnLock(); }); }
            if(c.MethodName == "Get" && p.Name == "journalRepository") return (Guid)c.Args[0] == original.Id ? original : null;
            if(c.MethodName == "BulkSave") { Check(!original.IsLocked, "No original lock is staged before posting accepts the reversal"); staged = (List<Journal>)c.Args[1]; return !bulkFailure; }
            throw new Exception("Unexpected journal dependency " + p.Name + "." + c.MethodName);
        });
        var request = new List<JournalDTO> { new JournalDTO { Id = original.Id, TotalValue = 999999 } };
        Check(!service.ReverseJournals(new List<JournalDTO>(), "test", 0, header), "Empty request fails");
        Check(!service.ReverseJournals(new List<JournalDTO>{request[0],request[0]}, "test", 0, header), "Duplicate journal IDs fail");
        Check(!service.ReverseJournals(new List<JournalDTO>{new JournalDTO {Id=Guid.NewGuid()}}, "test", 0, header), "Missing journal fails");
        bulkFailure = true;
        try { service.ReverseJournals(request, "test", 0, header); throw new Exception("Expected posting failure"); } catch(InvalidOperationException) { Check(!original.IsLocked && commits == 0, "Failed posting does not lock or commit original"); }
        bulkFailure = false; commitFailure = true;
        try { service.ReverseJournals(request, "test", 0, header); throw new Exception("Expected commit failure"); } catch(InvalidOperationException) { Check(!original.IsLocked && commits == 0, "Commit failure rolls back through scope"); }
        commitFailure = false;
        Check(service.ReverseJournals(request, "test", 0, header), "Retry succeeds");
        Check(original.IsLocked && commits == 1 && staged.Count == 1 && staged[0].IsLocked, "One commit contains reversal and lock");
        Check(staged[0].TotalValue == 500 && staged[0].JournalEntries.Sum(e=>e.Amount) == 0, "Persisted value used and reversal balanced");
        Check(original.JournalEntries.All(e=>staged[0].JournalEntries.Any(r=>r.Amount == -e.Amount && r.ChartOfAccountId == e.ChartOfAccountId && r.CustomerAccountId == e.CustomerAccountId)), "Every original line is reversed on the same accounts");
        Check(!service.ReverseJournals(request, "test", 0, header) && commits == 1, "Stale unlocked DTO cannot reverse a locked original again");
        original.UnLock(); original.JournalEntries.First().Amount += 1;
        Check(!service.ReverseJournals(request, "test", 0, header), "Unbalanced journal rejected");
        original.JournalEntries.Clear();
        Check(!service.ReverseJournals(request, "test", 0, header), "Journal without entries rejected");
    }
    static void BatchChecks() {
        var header = new ServiceHeader { ApplicationUserName = "worker" };
        var batch = new JournalReversalBatch { Status = (byte)BatchStatus.Posted, AuthorizedBy = "authorizer" }; batch.GenerateNewIdentity();
        var entry = new JournalReversalBatchEntry { JournalReversalBatchId = batch.Id, JournalId = Guid.NewGuid(), Status = (byte)BatchEntryStatus.Pending }; entry.GenerateNewIdentity();
        var journal = new JournalDTO { Id = entry.JournalId.Value };
        bool reverseResult = false, throwReverse = false, commitFailure = false, committed = false;
        int calls = 0, commits = 0;
        var service = Build<JournalReversalBatchAppService>((p,c)=> {
            if(c.MethodName == "CreateWithTransaction") { Check((System.Data.IsolationLevel)c.Args[0] == System.Data.IsolationLevel.Serializable, "Batch uses serializable transaction"); var status=entry.Status; committed=false; return Scope(c, ()=> { if(commitFailure) throw new InvalidOperationException("commit failure"); commits++; committed=true; }, ()=> {if(!committed) entry.Status=status;}); }
            if(c.MethodName == "Get") { if(p.Name == "journalReversalBatchEntryRepository") return (Guid)c.Args[0] == entry.Id ? entry : null; return batch; }
            if(c.MethodName == "FindJournal") return journal;
            if(c.MethodName == "ReverseJournals") { calls++; Check(entry.Status == (int)BatchEntryStatus.Pending, "Entry remains Pending until reversal succeeds"); if(throwReverse) throw new InvalidOperationException("posting failure"); return reverseResult; }
            throw new Exception("Unexpected batch dependency " + p.Name + "." + c.MethodName);
        });
        Check(!service.PostJournalReversalBatchEntry(Guid.NewGuid(),0,header), "Missing batch entry fails");
        foreach(var status in new[]{BatchStatus.Pending,BatchStatus.Audited,BatchStatus.Rejected}) { batch.Status=(byte)status; Check(!service.PostJournalReversalBatchEntry(entry.Id,0,header) && calls==0, "Unauthorized batch cannot post"); }
        batch.Status=(byte)BatchStatus.Posted;
        Check(!service.PostJournalReversalBatchEntry(entry.Id,0,header) && entry.Status==(int)BatchEntryStatus.Pending && commits==0, "False reversal leaves entry retryable");
        throwReverse=true;
        try { service.PostJournalReversalBatchEntry(entry.Id,0,header); throw new Exception("Expected failure"); } catch(InvalidOperationException) { Check(entry.Status==(int)BatchEntryStatus.Pending && header.ApplicationUserName=="worker", "Posting exception retains Pending and restores worker identity"); }
        throwReverse=false; reverseResult=true; commitFailure=true;
        try { service.PostJournalReversalBatchEntry(entry.Id,0,header); throw new Exception("Expected failure"); } catch(InvalidOperationException) { Check(entry.Status==(int)BatchEntryStatus.Pending, "Failed commit does not retain Posted"); }
        commitFailure=false;
        Check(service.PostJournalReversalBatchEntry(entry.Id,0,header) && entry.Status==(int)BatchEntryStatus.Posted && commits==1, "Retry marks Posted after reversal");
        var before=calls;
        Check(service.PostJournalReversalBatchEntry(entry.Id,0,header) && calls==before && commits==1, "Duplicate delivery is successful no-op");
    }
    class Stub : RealProxy { readonly Func<IMethodCallMessage, object> handler; public Stub(Type t, Func<IMethodCallMessage,object> h):base(t){handler=h;} public override IMessage Invoke(IMessage m){var c=(IMethodCallMessage)m;try{return new ReturnMessage(handler(c),null,0,c.LogicalCallContext,c);}catch(Exception e){return new ReturnMessage(e,c);}} }
}
