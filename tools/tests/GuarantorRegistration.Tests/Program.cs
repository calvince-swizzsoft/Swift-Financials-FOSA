using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;

class Program
{
    static int checks;
    static void Check(bool condition, string name) { if (!condition) throw new Exception(name); checks++; }
    static int Main()
    {
        try { Run(); Console.WriteLine(checks + " guarantor policy and AppService checks passed."); return 0; }
        catch (Exception exception) { Console.Error.WriteLine(exception); return 1; }
    }
    static void Run()
    {
        Check(GuarantorRegistrationRules.ValidateEditableCase(null) != null, "Missing case rejected");
        foreach (LoanCaseStatus status in Enum.GetValues(typeof(LoanCaseStatus)))
            Check((GuarantorRegistrationRules.ValidateEditableCase(new LoanCaseDTO { Status = (int)status }) == null) == (status == LoanCaseStatus.Registered), "Edit lifecycle guard: " + status);
        var caseId = Guid.NewGuid();
        var commitments = new[] { new LoanGuarantorDTO { LoanCaseId = caseId, AmountGuaranteed = 700 }, new LoanGuarantorDTO { LoanCaseId = Guid.NewGuid(), AmountGuaranteed = 200 }, new LoanGuarantorDTO { AmountGuaranteed = 100 } };
        Check(GuarantorRegistrationRules.CommittedShares(commitments, caseId) == 300, "Replacement excludes this case only");
        Check(GuarantorRegistrationRules.CommittedShares(commitments, null) == 1000, "Registration includes all commitments");
        var mixedCommitments = commitments.Concat(new[] {
            new LoanGuarantorDTO { LoanCaseId = Guid.NewGuid(), Status = (int)LoanGuarantorStatus.Released, AmountGuaranteed = 9000 },
            new LoanGuarantorDTO { LoanCaseId = caseId, Status = (int)LoanGuarantorStatus.Released, AmountGuaranteed = 8000 },
            new LoanGuarantorDTO { Status = (int)LoanGuarantorStatus.Released, AmountGuaranteed = 7000 }
        }).ToArray();
        Check(GuarantorRegistrationRules.CommittedShares(mixedCommitments, null) == 1000, "Released guarantees do not reserve capacity during registration");
        Check(GuarantorRegistrationRules.CommittedShares(mixedCommitments, caseId) == 300, "Editing excludes released guarantees and current-case commitments");
        Check(GuarantorRegistrationRules.CommittedShares(mixedCommitments.Where(item => item.Status == (int)LoanGuarantorStatus.Released), null) == 0, "Released-only history reserves no capacity");
        Check(GuarantorRegistrationRules.CommittedShares(new LoanGuarantorDTO[0], null) == 0, "No guarantees reserves no capacity");
        var released = new LoanGuarantorDTO { Status = (int)LoanGuarantorStatus.Attached, AmountGuaranteed = 600 };
        Check(GuarantorRegistrationRules.CommittedShares(new[] { released }, null) == 600, "Attached guarantee reserves its amount");
        released.Status = (int)LoanGuarantorStatus.Released;
        Check(GuarantorRegistrationRules.CommittedShares(new[] { released }, null) == 0 && released.AmountGuaranteed == 600, "Releasing restores capacity without erasing historical amount");
        var depositId = Guid.NewGuid();
        var bosa = new LoanProductDTO { LoanRegistrationLoanProductSection = (int)LoanProductSection.BOSA };
        var balances = new[] {
            new CustomerAccountDTO { CustomerAccountTypeProductCode = (int)ProductCode.Investment, CustomerAccountTypeTargetProductId = depositId, BookBalance = 20000 },
            new CustomerAccountDTO { CustomerAccountTypeProductCode = (int)ProductCode.Investment, CustomerAccountTypeTargetProductId = Guid.NewGuid(), BookBalance = 30000 },
            new CustomerAccountDTO { CustomerAccountTypeProductCode = (int)ProductCode.Savings, CustomerAccountTypeTargetProductId = depositId, BookBalance = 10000 }
        };
        Check(GuarantorRegistrationRules.EligibleShares(bosa, balances, new[] { depositId, depositId }) == 20000, "BOSA includes designated investment balances only, without duplicates");
        Check(GuarantorRegistrationRules.EligibleShares(bosa, balances, new[] { Guid.NewGuid() }) == 0, "No designated account gives zero capacity");
        try { GuarantorRegistrationRules.EligibleShares(bosa, balances, new Guid[0]); throw new Exception("Missing BOSA designation allowed"); }
        catch (InvalidOperationException) { checks++; }
        balances[0].BookBalance = 5000;
        Check(GuarantorRegistrationRules.EligibleShares(bosa, balances, new[] { depositId }) == 5000, "BOSA uses maturity-adjusted book balance supplied by account service");
        bosa.LoanRegistrationLoanProductSection = (int)LoanProductSection.FOSA;
        Check(GuarantorRegistrationRules.EligibleShares(bosa, balances, new Guid[0]) == 45000, "FOSA eligibility remains unchanged");
        var customer = new CustomerDTO { Id = Guid.NewGuid(), RecordStatus = (byte)RecordStatus.Approved };
        var product = new LoanProductDTO { Id = Guid.NewGuid(), LoanRegistrationMinimumGuarantors = 1, LoanRegistrationMaximumGuarantees = 2, LoanRegistrationGuarantorSecurityMode = (int)GuarantorSecurityMode.Income };
        var loan = new LoanCaseDTO { CustomerId = Guid.NewGuid(), LoanProductId = product.Id, AmountApplied = 1000 };
        Func<LoanGuarantorDTO> row = () => new LoanGuarantorDTO { GuarantorId = customer.Id, AmountGuaranteed = 1000, TotalShares = 999999, AppraisalFactor = 999 };
        var constructor = typeof(LoanCaseAppService).GetConstructors().Single();
        var service = (LoanCaseAppService)constructor.Invoke(constructor.GetParameters().Select(parameter =>
            new Stub(parameter.ParameterType, call => {
                switch (call.MethodName)
                {
                    case "FindLoanProduct": return product;
                    case "FindCustomer": return customer;
                    case "FindCustomerAccountsByCustomerId": return new List<CustomerAccountDTO> { new CustomerAccountDTO { CustomerAccountTypeProductCode = (int)ProductCode.Investment, BookBalance = 1000 } };
                    case "GetGuarantorAppraisalFactor": return 1d;
                    case "CreateReadOnly": return new Stub(((MethodInfo)call.MethodBase).ReturnType, c => null).GetTransparentProxy();
                    default: throw new Exception("Unexpected dependency call: " + call.MethodName);
                }
            }).GetTransparentProxy()).ToArray());
        var header = new ServiceHeader();
        var rows = new List<LoanGuarantorDTO> { row() };
        Check(service.ValidateRegistrationGuarantors(loan, rows, header) == null, "Income mode accepts positive guarantee without shares");
        Check(rows[0].TotalShares == 0 && rows[0].AppraisalFactor == 0, "Client-supplied capacity is overwritten");
        Check(rows[0].CustomerId == customer.Id && rows[0].LoaneeCustomerId == loan.CustomerId, "Server assigns guarantee identities");
        Check(service.GetRegistrationGuarantorEligibility(customer.Id, product.Id, header).Item2.LoanProductLoanRegistrationGuarantorSecurityMode == 0, "Lookup reports income security mode");
        foreach (var micro in new[] { false, true })
        {
            product.LoanRegistrationMicrocredit = micro;
            Check(service.ValidateRegistrationGuarantors(loan, new List<LoanGuarantorDTO>(), header) != null, "Minimum is enforced regardless of microcredit or security toggle");
        }
        customer.IsLocked = true;
        Check(service.ValidateRegistrationGuarantors(loan, rows, header) != null, "Locked member rejected during registration");
        try { service.GetRegistrationGuarantorEligibility(customer.Id, product.Id, header); throw new Exception("Locked lookup allowed"); } catch (InvalidOperationException) { checks++; }
        customer.IsLocked = false; customer.RecordStatus = (byte)RecordStatus.New;
        Check(service.ValidateRegistrationGuarantors(loan, rows, header) != null, "Unapproved member rejected");
        customer.RecordStatus = (byte)RecordStatus.Approved; customer.InhibitGuaranteeing = true;
        Check(service.ValidateRegistrationGuarantors(loan, rows, header) != null, "Inhibited member rejected");
        customer.InhibitGuaranteeing = false;
        foreach (var amount in new[] { 0m, -1m, 1.001m })
        {
            var invalid = row(); invalid.AmountGuaranteed = amount;
            Check(service.ValidateRegistrationGuarantors(loan, new List<LoanGuarantorDTO> { invalid }, header) != null, "Invalid income pledge rejected");
        }
        Check(service.ValidateRegistrationGuarantors(loan, new List<LoanGuarantorDTO> { row(), row() }, header) != null, "Duplicate members rejected");
        Check(service.ValidateRegistrationGuarantors(loan, new List<LoanGuarantorDTO> { null }, header) != null, "Null entry rejected");
        loan.CustomerId = customer.Id;
        Check(service.ValidateRegistrationGuarantors(loan, rows, header) != null, "Self-guarantee rejected when disabled");
        product.LoanRegistrationAllowSelfGuarantee = true; product.LoanRegistrationMaximumSelfGuaranteeEligiblePercentage = 50;
        Check(service.ValidateRegistrationGuarantors(loan, rows, header) != null, "Self-guarantee percentage enforced");
        loan.CustomerId = Guid.NewGuid();
        product.LoanRegistrationGuarantorSecurityMode = (int)GuarantorSecurityMode.Investments;
        // Pure policy cases isolate capacity and coverage from database projection dependencies.
        var secured = row(); secured.TotalShares = 1000; secured.AppraisalFactor = 1;
        Check(GuarantorRegistrationRules.ValidateAmount(product, loan, secured) == null, "Exact investment capacity accepted for microcredit");
        secured.CommittedShares = 1;
        Check(GuarantorRegistrationRules.ValidateAmount(product, loan, secured) != null, "Existing commitments reduce capacity");
        secured.CommittedShares = 0; secured.AmountGuaranteed = 500;
        product.LoanRegistrationSecurityRequired = true;
        Check(GuarantorRegistrationRules.ValidateCoverage(product, loan, new[] { secured }) != null, "Microcredit required coverage enforced");
        loan.TotalCollateralAmount = 500;
        Check(GuarantorRegistrationRules.ValidateCoverage(product, loan, new[] { secured }) == null, "Collateral completes coverage");
        product.LoanRegistrationMaximumGuarantees = 0;
        Check(GuarantorRegistrationRules.ValidateCount(product, rows) != null, "Inconsistent product counts rejected");
    }
    class Stub : RealProxy
    {
        readonly Func<IMethodCallMessage, object> handler;
        public Stub(Type type, Func<IMethodCallMessage, object> handler) : base(type) { this.handler = handler; }
        public override IMessage Invoke(IMessage message)
        {
            var call = (IMethodCallMessage)message;
            try { return new ReturnMessage(handler(call), null, 0, call.LogicalCallContext, call); }
            catch (Exception exception) { return new ReturnMessage(exception, call); }
        }
    }
}
