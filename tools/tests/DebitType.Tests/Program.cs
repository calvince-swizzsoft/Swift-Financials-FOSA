using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
class Program
{
    static int checks;
    static int Main() { try { Run(); Console.WriteLine(checks + " debit type AppService validation checks passed."); return 0; } catch(Exception e) { Console.Error.WriteLine(e); return 1; } }
    static void Check(bool value, string message) { if (!value) throw new Exception(message); checks++; }
    class Validated : Exception { }
    static void Run()
    {
        bool productExists = true, commissionExists = true;
        int writes = 0;
        var constructor = typeof(DebitTypeAppService).GetConstructors().Single();
        var service = (DebitTypeAppService)constructor.Invoke(constructor.GetParameters().Select(parameter => new Stub(parameter.ParameterType, call => {
            switch(call.MethodName) {
                case "FindSavingsProduct": return productExists ? new SavingsProductDTO { Code = 11 } : null;
                case "FindLoanProduct": return productExists ? new LoanProductDTO { Code = 22 } : null;
                case "FindInvestmentProduct": return productExists ? new InvestmentProductDTO { Code = 33 } : null;
                case "FindCommission": return commissionExists ? new CommissionDTO { Id = (Guid)call.Args[0] } : null;
                case "Create": writes++; throw new Validated();
                default: throw new Exception("Unexpected dependency: " + call.MethodName);
            }
        }).GetTransparentProxy()).ToArray());
        Func<DebitTypeDTO> dto = () => new DebitTypeDTO { Description = "  Fee  ", CustomerAccountTypeProductCode = 1, CustomerAccountTypeTargetProductId = Guid.NewGuid(), CustomerAccountTypeTargetProductCode = 999 };
        Action<DebitTypeDTO, List<CommissionDTO>> reject = (item, charges) => {
            var before = writes;
            try { service.SaveConfiguredDebitType(item, charges, new ServiceHeader()); throw new Exception("Invalid data accepted"); }
            catch(ArgumentException) { Check(writes == before, "Invalid data must not begin writes"); }
        };
        var empty = new List<CommissionDTO>();
        reject(null, empty);
        var invalid = dto(); invalid.Description = " "; reject(invalid, empty);
        invalid = dto(); invalid.CustomerAccountTypeProductCode = 999; reject(invalid, empty);
        invalid = dto(); invalid.CustomerAccountTypeTargetProductId = Guid.Empty; reject(invalid, empty);
        productExists = false; reject(dto(), empty); productExists = true;
        reject(dto(), null);
        reject(dto(), new List<CommissionDTO> { null });
        reject(dto(), new List<CommissionDTO> { new CommissionDTO() });
        var charge = new CommissionDTO { Id = Guid.NewGuid() };
        reject(dto(), new List<CommissionDTO> { charge, charge });
        commissionExists = false; reject(dto(), new List<CommissionDTO> { charge }); commissionExists = true;
        foreach (var code in new[] {1,2,3}) {
            var item = dto(); item.CustomerAccountTypeProductCode = code;
            try { service.SaveConfiguredDebitType(item, empty, new ServiceHeader()); throw new Exception("Expected transaction boundary"); }
            catch(Validated) { Check(item.CustomerAccountTypeTargetProductCode == code * 11 && item.Description == "Fee", "Server derives product code and trims description"); }
        }
    }
    class Stub : RealProxy {
        readonly Func<IMethodCallMessage, object> handler;
        public Stub(Type type, Func<IMethodCallMessage, object> handler) : base(type) { this.handler = handler; }
        public override IMessage Invoke(IMessage message) { var call = (IMethodCallMessage)message; try { return new ReturnMessage(handler(call), null, 0, call.LogicalCallContext, call); } catch(Exception e) { return new ReturnMessage(e, call); } }
    }
}
