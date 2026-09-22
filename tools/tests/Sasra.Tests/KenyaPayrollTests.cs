using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;

static class KenyaPayrollTests
{
    static int checks;
    static void Check(bool ok, string label) { if (!ok) throw new Exception(label); checks++; }
    static void Reject(Action action) { try { action(); } catch (InvalidOperationException) { checks++; return; } throw new Exception("Expected payroll rejection"); }
    static List<PaySlipEntryDTO> Entries(decimal gross)
    {
        var result = new List<PaySlipEntryDTO> { new PaySlipEntryDTO { SalaryHeadType=(int)SalaryHeadType.PartTimeBasicPayEarning, SalaryHeadCategory=1, Principal=gross, CustomerAccountId=Guid.NewGuid(), ChartOfAccountId=Guid.NewGuid() } };
        foreach(var type in new[]{SalaryHeadType.NSSFDeduction,SalaryHeadType.SHIFDeduction,SalaryHeadType.AffordableHousingLevyDeduction,SalaryHeadType.PAYEDeduction})
            result.Add(new PaySlipEntryDTO { SalaryHeadType=(int)type, SalaryHeadCategory=2, Principal=999, ChartOfAccountId=Guid.NewGuid() });
        return result;
    }
    static decimal Amount(List<PaySlipEntryDTO> entries, SalaryHeadType type) { return entries.Single(x=>x.SalaryHeadType==(int)type).Principal; }
    public static void Run()
    {
        var date = new DateTime(2026,9,1);
        var period = new SalaryProcessingDTO { TaxReliefAmount=2400, MaximumProvidentFundReliefAmount=30000, MaximumInsuranceReliefAmount=5000 };
        var card = new SalaryCardDTO();
        Check(KenyaPayrollRules.Nssf(200000,new DateTime(2025,1,1))==2160,"January 2025 NSSF ceiling");
        Check(KenyaPayrollRules.Nssf(200000,new DateTime(2025,2,1))==4320,"February 2025 NSSF ceiling");
        Check(KenyaPayrollRules.Nssf(200000,new DateTime(2026,1,1))==4320,"January 2026 retains previous ceiling");
        Check(KenyaPayrollRules.Nssf(200000,new DateTime(2026,2,1))==6480,"February 2026 NSSF ceiling");
        Check(KenyaPayrollRules.Nssf(9000,date)==540,"NSSF tier one boundary");
        Check(KenyaPayrollRules.Nssf(108000,date)==6480,"NSSF upper boundary");
        Check(KenyaPayrollRules.Shif(5000)==300 && KenyaPayrollRules.Shif(0)==0 && KenyaPayrollRules.Shif(100000)==2750,"SHIF minimum, zero pay and uncapped percentage");
        Check(KenyaPayrollRules.GrossTax(24000)==2400 && KenyaPayrollRules.GrossTax(32333)==4483.25m,"First two tax bands");
        Check(KenyaPayrollRules.GrossTax(500000)==144783.35m && KenyaPayrollRules.GrossTax(800000)==242283.35m && KenyaPayrollRules.GrossTax(900000)==277283.35m,"Upper tax bands");
        var entries=Entries(100000);
        KenyaPayrollRules.Apply(date,entries,100000,period,card);
        Check(Amount(entries,SalaryHeadType.NSSFDeduction)==6000 && Amount(entries,SalaryHeadType.SHIFDeduction)==2750 && Amount(entries,SalaryHeadType.AffordableHousingLevyDeduction)==1500,"100,000 monthly statutory contributions");
        Check(Amount(entries,SalaryHeadType.PAYEDeduction)==19308.35m,"PAYE deducts pension, SHIF and Housing Levy before tax");
        Check(100000-entries.Where(x=>x.SalaryHeadCategory==2).Sum(x=>x.Principal)==70441.65m,"Net pay excludes employer contributions");
        entries=Entries(10000);KenyaPayrollRules.Apply(date,entries,10000,period,card);
        Check(Amount(entries,SalaryHeadType.PAYEDeduction)==0,"Low pay clears any configured fixed PAYE amount");
        entries=Entries(120000);KenyaPayrollRules.Apply(date,entries,100000,period,card);
        Check(Amount(entries,SalaryHeadType.AffordableHousingLevyDeduction)==1500 && Amount(entries,SalaryHeadType.SHIFDeduction)==3300,"Irregular earnings excluded only from Housing Levy");
        entries=Entries(100000);entries.Add(new PaySlipEntryDTO{SalaryHeadType=(int)SalaryHeadType.VoluntaryProvidentFundDeduction,SalaryHeadCategory=2,Principal=40000});
        KenyaPayrollRules.Apply(date,entries,100000,period,card);
        Check(Amount(entries,SalaryHeadType.PAYEDeduction)==12108.35m,"Combined registered pension allowance capped at 30,000");
        entries=Entries(50000);entries.Add(new PaySlipEntryDTO{SalaryHeadType=(int)SalaryHeadType.VoluntaryProvidentFundDeduction,SalaryHeadCategory=2,Principal=20000});
        KenyaPayrollRules.Apply(date,entries,50000,period,card);
        Check(Amount(entries,SalaryHeadType.PAYEDeduction)==2245.85m,"Pension allowance also respects thirty percent of pensionable cash income");
        entries=Entries(100000);card.InsuranceReliefAmount=8000;KenyaPayrollRules.Apply(date,entries,100000,period,card);
        Check(Amount(entries,SalaryHeadType.PAYEDeduction)==14308.35m,"Insurance relief cap");
        card.InsuranceReliefAmount=0;card.IsTaxExempt=true;card.TaxExemption=150000;KenyaPayrollRules.Apply(date,entries,100000,period,card);
        Check(Amount(entries,SalaryHeadType.PAYEDeduction)==0,"Exemption never produces negative PAYE");
        Reject(()=>KenyaPayrollRules.ValidateHeads(date,new[]{SalaryHeadType.NHIFDeduction}));
        Reject(()=>KenyaPayrollRules.Apply(date,Entries(10000).Take(4).ToList(),10000,period,new SalaryCardDTO()));
        Reject(()=>KenyaPayrollRules.Nssf(100000,new DateTime(2027,2,1)));
        Reject(()=>KenyaPayrollRules.ValidateHeads(new DateTime(2024,9,1),new[]{SalaryHeadType.SHIFDeduction}));
        ServiceTests();
        PersistenceTests();
        Console.WriteLine("PASS: "+checks+" Kenya payroll rate, relief, processing and posting assertions.");
    }

    class Proxy:RealProxy
    {
        readonly Func<IMethodCallMessage,object> call;
        public Proxy(Type type,Func<IMethodCallMessage,object> call):base(type){this.call=call;}
        public override IMessage Invoke(IMessage msg){var c=(IMethodCallMessage)msg;try{return new ReturnMessage(call(c),null,0,c.LogicalCallContext,c);}catch(Exception e){return new ReturnMessage(e,c);}}
    }
    static object Stub(Type type,Func<IMethodCallMessage,object> call){return new Proxy(type,call).GetTransparentProxy();}
    static void ServiceTests()
    {
        var writes=new List<string>();var pairs=new List<Tuple<decimal,Guid,Guid>>();int commits=0;bool missingMapping=false, failJournal=false;
        var stored=SalaryPeriodFactory.CreateSalaryPeriod(Guid.NewGuid(),9,(int)EmployeeCategory.PartTime,2400,30000,5000,"Payroll test");
        stored.Status=(int)SalaryPeriodStatus.Open;
        var period=new SalaryProcessingDTO{Id=stored.Id,PostingPeriodId=stored.PostingPeriodId,Month=9,Status=(int)SalaryPeriodStatus.Open,TaxReliefAmount=2400,MaximumProvidentFundReliefAmount=30000,MaximumInsuranceReliefAmount=5000,ExecutePayoutStandingOrders=true};
        var posting=new PostingPeriodDTO{Id=stored.PostingPeriodId,DurationEndDate=new DateTime(2026,12,31)};
        var account=new CustomerAccountDTO{Id=Guid.NewGuid(),CustomerAccountTypeTargetProductId=Guid.NewGuid(),BranchId=Guid.NewGuid()};
        var savings=new SavingsProductDTO{Id=account.CustomerAccountTypeTargetProductId,ChartOfAccountId=Guid.NewGuid()};
        var employeeType=new EmployeeTypeDTO{Id=Guid.NewGuid(),ChartOfAccountId=Guid.NewGuid()};
        var salaryCard=new SalaryCardDTO{Id=Guid.NewGuid()};
        var salaryEntries=Entries(100000).Select(x=>new SalaryCardEntryDTO{SalaryGroupEntrySalaryHeadType=x.SalaryHeadType,SalaryGroupEntrySalaryHeadCategory=x.SalaryHeadCategory,SalaryGroupEntrySalaryHeadChartOfAccountId=x.ChartOfAccountId,ChargeType=(int)ChargeType.FixedAmount,ChargeFixedAmount=x.Principal}).ToList();
        List<PaySlipDTO> calculated=null;
        var paySlip=new PaySlipDTO{Id=Guid.NewGuid(),SalaryPeriodId=period.Id,SalaryPeriodPostingPeriodId=posting.Id,SalaryCardId=salaryCard.Id,SalaryCardEmployeeEmployeeTypeCategory=(int)EmployeeCategory.PartTime,SalaryCardEmployeeEmployeeTypeId=employeeType.Id,SalaryCardEmployeeEmployeeTypeChartOfAccountId=employeeType.ChartOfAccountId,SalaryCardEmployeeBranchId=account.BranchId,Status=(int)PaySlipStatus.Pending};
        var nssfExpense=Guid.NewGuid();var housingExpense=Guid.NewGuid();
        var adapter=Stub(typeof(ITypeAdapter),c=>period);
        TypeAdapterFactory.SetCurrent((ITypeAdapterFactory)Stub(typeof(ITypeAdapterFactory),c=>adapter));
        var scope=Stub(typeof(IDbContextScope),c=>{if(c.MethodName=="SaveChanges"){writes.Add("commit");commits++;return 1;}return null;});
        var read=Stub(typeof(IDbContextReadOnlyScope),c=>null);
        var ctor=typeof(SalaryPeriodAppService).GetConstructors().Single();
        var args=ctor.GetParameters().Select(p=>Stub(p.ParameterType,c=>{
            switch(p.Name)
            {
                case "dbContextScopeFactory":return c.MethodName=="CreateReadOnly"?read:scope;
                case "salaryPeriodRepository":if(c.MethodName=="Get")return stored;break;
                case "postingPeriodAppService":return posting;
                case "salaryCardAppService":
                    if(c.MethodName=="FindSalaryCardByEmployeeId")return salaryCard;
                    if(c.MethodName=="FindSalaryCardEntriesBySalaryCardId")return salaryEntries;
                    if(c.MethodName=="ZeroizeOneOffEarnings"){writes.Add("zeroize");return true;}break;
                case "sqlCommandAppService":if(c.MethodName=="FindCustomerAccountById")return account;return new List<CustomerAccountDTO>{account};
                case "paySlipAppService":
                    if(c.MethodName=="FindPaySlipsBySalaryPeriodId")return new List<PaySlipDTO>();
                    if(c.MethodName=="PurgePaySlips"){writes.Add("purge");return true;}
                    if(c.MethodName=="AddNewPaySlips"){writes.Add("draft");calculated=(List<PaySlipDTO>)c.Args[0];return true;}
                    if(c.MethodName=="FindPaySlip")return paySlip;
                    if(c.MethodName=="FindPaySlipEntriesByPaySlipId")return calculated[0].PaySlipEntries.ToList();
                    if(c.MethodName=="MarkPaySlipPosted"){writes.Add("mark");paySlip.Status=(int)PaySlipStatus.Posted;return true;}break;
                case "employeeTypeAppService":return employeeType;
                case "chartOfAccountAppService":
                    if(c.MethodName=="FindChartOfAccount")return new ChartOfAccountDTO{Id=(Guid)c.Args[0],AccountCategory=(int)ChartOfAccountCategory.DetailAccount};
                    if(c.MethodName=="GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode")return missingMapping?Guid.Empty:(int)c.Args[0]==(int)SystemGeneralLedgerAccountCode.EmployerNSSFContribution?nssfExpense:housingExpense;
                    break;
                case "customerAccountAppService":if(c.MethodName=="FetchCustomerAccountBalances")return null;break;
                case "savingsProductAppService":return savings;
                case "commissionAppService":if(c.MethodName=="ComputeTariffsBySavingsProduct")return new List<TariffWrapper>();break;
                case "appCache":return period;
                case "journalEntryPostingService":
                    if(c.MethodName=="PerformDoubleEntry"){pairs.Add(Tuple.Create(((Journal)c.Args[0]).TotalValue,(Guid)c.Args[1],(Guid)c.Args[2]));return null;}
                    if(c.MethodName=="BulkSave"){writes.Add("journals");if(failJournal)throw new InvalidOperationException("Simulated journal failure");return true;}break;
                case "recurringBatchAppService":writes.Add("queue");return true;
            }
            throw new Exception("Unexpected payroll dependency "+p.Name+"."+c.MethodName);
        })).ToArray();
        var service=(SalaryPeriodAppService)ctor.Invoke(args);var h=new ServiceHeader{ApplicationUserName="test"};
        var employees=new List<EmployeeDTO>{new EmployeeDTO{Id=Guid.NewGuid(),EmployeeTypeCategory=(int)EmployeeCategory.PartTime}};
        Check(service.ProcessSalaryPeriod(new SalaryProcessingDTO{Id=period.Id,TaxReliefAmount=999999},employees,h),"Real part-time payroll processing");
        Check(string.Join(",",writes)=="purge,draft,commit" && commits==1,"Draft replacement in one transaction after successful calculations");
        Check(Amount(calculated[0].PaySlipEntries.ToList(),SalaryHeadType.PAYEDeduction)==19308.35m,"Stored period reliefs override caller-supplied reliefs");
        var removed=salaryEntries.Last();salaryEntries.Remove(removed);writes.Clear();Reject(()=>service.ProcessSalaryPeriod(new SalaryProcessingDTO{Id=period.Id},employees,h));
        Check(writes.Count==0 && commits==1,"Missing statutory head leaves existing draft untouched");salaryEntries.Add(removed);
        writes.Clear();missingMapping=true;Reject(()=>service.PostPaySlip(paySlip.Id,1,h));Check(writes.Count==0 && commits==1,"Missing employer mapping rejected before marking posted");missingMapping=false;
        writes.Clear();failJournal=true;Reject(()=>service.PostPaySlip(paySlip.Id,1,h));Check(!writes.Contains("commit")&&!writes.Contains("queue"),"Failed journals cannot commit or queue payout");failJournal=false;paySlip.Status=(int)PaySlipStatus.Pending;
        pairs.Clear();writes.Clear();Check(service.PostPaySlip(paySlip.Id,1,h),"Real statutory payslip posting");
        Check(string.Join(",",writes)=="mark,journals,zeroize,commit,queue","Posting commits status, journals and one-off clearing before payout dispatch");
        Check(pairs.Any(x=>x.Item1==6000&&x.Item3==nssfExpense)&&pairs.Any(x=>x.Item1==1500&&x.Item3==housingExpense),"NSSF and Housing Levy employer expenses posted separately");
        Check(pairs.Single(x=>x.Item2==savings.ChartOfAccountId).Item1==70441.65m,"Posted net pay excludes employer contributions");
    }

    static void PersistenceTests()
    {
        var period=Guid.NewGuid();var card=Guid.NewGuid();var h=new ServiceHeader{ApplicationUserName="Payroll regression"};
        var old=Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipAgg.PaySlipFactory.CreatePaySlip(period,card,"Old draft");
        var child=Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipEntryAgg.PaySlipEntryFactory.CreatePaySlipEntry(old.Id,Guid.NewGuid(),Guid.NewGuid(),"Old basic",(int)SalaryHeadType.FullTimeBasicPayEarning,1,30000,0,0,new Domain.MainBoundedContext.ValueObjects.Charge((int)ChargeType.FixedAmount,0,30000));
        old.PaySlipEntries.Add(child);
        var writes=new List<string>();
        Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipAgg.PaySlip added=null;
        bool fail=false;
        var scope=Stub(typeof(IDbContextScope),c=>{if(c.MethodName=="SaveChanges"){writes.Add("save");return 0;}return null;});
        var ctor=typeof(PaySlipAppService).GetConstructors().Single();
        var args=ctor.GetParameters().Select(p=>Stub(p.ParameterType,c=>{
            if(p.Name=="dbContextScopeFactory"&&c.MethodName=="Create")return scope;
            if(p.Name=="paySlipRepository")
            {
                if(c.MethodName=="AllMatching")return new List<Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipAgg.PaySlip>{old};
                if(c.MethodName=="Remove"){writes.Add("remove slip");return null;}
                if(c.MethodName=="Add"){if(fail)throw new InvalidOperationException("Simulated persistence failure");added=(Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipAgg.PaySlip)c.Args[0];writes.Add("add aggregate");return null;}
            }
            if(p.Name=="paySlipEntryRepository"&&c.MethodName=="Remove"){writes.Add("remove entry");return null;}
            throw new Exception("Unexpected separate-connection persistence: "+p.Name+"."+c.MethodName);
        })).ToArray();
        var service=(PaySlipAppService)ctor.Invoke(args);
        var draft=new PaySlipDTO{SalaryPeriodId=period,SalaryPeriodMonth=9,SalaryCardId=card,Remarks="New draft"};
        foreach(var entry in Entries(100000))draft.PaySlipEntries.Add(entry);
        Check(service.AddNewPaySlips(new List<PaySlipDTO>{draft},h),"Payslip persistence supports a joined scope with deferred commit");
        Check(string.Join(",",writes)=="remove entry,remove slip,add aggregate,save","Old draft and new aggregate use the same repository scope without bulk SQL");
        Check(added.SalaryPeriodId==period&&added.SalaryCardId==card&&added.Status==(int)PaySlipStatus.Pending&&added.CreatedBy==h.ApplicationUserName,"Replacement preserves payroll identity and pending status");
        Check(added.PaySlipEntries.Count==5&&added.PaySlipEntries.All(x=>x.PaySlipId==added.Id)&&added.PaySlipEntries.Sum(x=>x.Principal)==103996,"All entries belong to the replacement header and retain amounts");
        writes.Clear();fail=true;
        Reject(()=>service.AddNewPaySlips(new List<PaySlipDTO>{draft},h));
        Check(!writes.Contains("save"),"Failed aggregate persistence does not commit the draft deletion");
        Check(!service.AddNewPaySlips(new List<PaySlipDTO>(),h),"Empty input does not report a saved payroll");
    }
}
