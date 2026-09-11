using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg;
using Infrastructure.Crosscutting.Framework.Utils;

class Program
{
    static int checks;
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); checks++; }
    static readonly MethodInfo Post = typeof(JournalVoucherAppService).GetMethod("PostVoucherEntries", BindingFlags.Static | BindingFlags.NonPublic);
    static readonly ServiceHeader Header = new ServiceHeader { ApplicationUserName = "voucher-test" };
    static JournalEntryPostingService PostingService()
    {
        var ctor = typeof(JournalEntryPostingService).GetConstructors().Single();
        return (JournalEntryPostingService)ctor.Invoke(ctor.GetParameters().Select(p => new UnusedDependency(p.ParameterType).GetTransparentProxy()).ToArray());
    }
    static Journal Run(JournalVoucherDTO voucher, List<JournalVoucherEntryDTO> entries)
    {
        var journal = new Journal { TotalValue = voucher.TotalValue, ValueDate = new DateTime(2026, 9, 11) };
        journal.GenerateNewIdentity();
        Post.Invoke(null, new object[] { journal, voucher, entries, PostingService(), Header });
        return journal;
    }
    static void Reject(JournalVoucherDTO voucher, List<JournalVoucherEntryDTO> entries)
    {
        try { Run(voucher, entries); throw new Exception("Invalid voucher accepted"); }
        catch (TargetInvocationException ex) { Check(ex.InnerException is InvalidOperationException, "Invalid voucher rejected before posting"); }
    }
    static int Main()
    {
        try
        {
            foreach (var type in new[] { 0, 1, 2, 3 })
            foreach (var split in new[] { false, true })
            {
                var main = Guid.NewGuid();
                var customer = Guid.NewGuid();
                var voucher = new JournalVoucherDTO { Type = (byte)type, ChartOfAccountId = main, CustomerAccountId = customer, TotalValue = 1000m };
                var entries = new List<JournalVoucherEntryDTO> { new JournalVoucherEntryDTO { ChartOfAccountId = Guid.NewGuid(), Amount = split ? 600m : 1000m } };
                if (split) entries.Add(new JournalVoucherEntryDTO { ChartOfAccountId = Guid.NewGuid(), CustomerAccountId = Guid.NewGuid(), Amount = 400m });
                var journal = Run(voucher, entries);
                var lines = journal.JournalEntries.ToList();
                var sign = type == 0 || type == 2 ? 1m : -1m;
                Check(lines.Count == entries.Count * 2, "One pair per allocation");
                Check(lines.Sum(e => e.Amount) == 0m, "Journal balances");
                Check(lines.Where(e => e.Amount > 0m).Sum(e => e.Amount) == 1000m && lines.Where(e => e.Amount < 0m).Sum(e => -e.Amount) == 1000m, "Debit and credit totals are each 1000");
                Check(lines.Where(e => e.ChartOfAccountId == main).Sum(e => e.Amount) == sign * 1000m, "Primary account moves in the selected direction");
                foreach (var allocation in entries)
                {
                    var primary = lines.Single(e => e.ChartOfAccountId == main && e.ContraChartOfAccountId == allocation.ChartOfAccountId);
                    var opposite = lines.Single(e => e.ChartOfAccountId == allocation.ChartOfAccountId);
                    Check(primary.Amount == sign * allocation.Amount && opposite.Amount == -primary.Amount, "Each allocation moves the expected amounts");
                    Check(opposite.ContraChartOfAccountId == main && primary.ContraChartOfAccountId != main, "Reciprocal contra accounts, no false self-contra");
                    Check(primary.CustomerAccountId == (type >= 2 ? (Guid?)customer : null) && opposite.CustomerAccountId == allocation.CustomerAccountId, "Customer identities retained only on their own legs");
                }
                foreach (var line in lines)
                {
                    var hash = line.IntegrityHash;
                    line.GenerateIntegrityHash();
                    Check(hash == line.IntegrityHash && !string.IsNullOrEmpty(hash), "Contra and amount included in valid integrity hash");
                    Check(line.ValueDate == journal.ValueDate && line.JournalId == journal.Id, "Same journal and value date retained");
                }
            }
            var invalid = new JournalVoucherDTO { Type = 0, TotalValue = 1000m, ChartOfAccountId = Guid.NewGuid() };
            Reject(invalid, new List<JournalVoucherEntryDTO>());
            Reject(invalid, new List<JournalVoucherEntryDTO> { new JournalVoucherEntryDTO { Amount = 999m } });
            Reject(invalid, new List<JournalVoucherEntryDTO> { new JournalVoucherEntryDTO { Amount = -1m }, new JournalVoucherEntryDTO { Amount = 1001m } });
            invalid.Type = 9;
            Reject(invalid, new List<JournalVoucherEntryDTO> { new JournalVoucherEntryDTO { Amount = 1000m } });
            invalid.Type = 2;
            Reject(invalid, new List<JournalVoucherEntryDTO> { new JournalVoucherEntryDTO { Amount = 1000m } });
            Console.WriteLine(checks + " voucher movement, contra, customer identity and validation checks passed.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
    class UnusedDependency : RealProxy
    {
        public UnusedDependency(Type type) : base(type) { }
        public override IMessage Invoke(IMessage message) { throw new Exception("Unexpected database access in posting checks"); }
    }
}
