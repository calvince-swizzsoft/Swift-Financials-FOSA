using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using WebApplication1.Areas.BackOffice.Controllers;

static class LoanStageMakerCheckerChecks
{
    sealed class WorkflowStub : RealProxy
    {
        public readonly Guid CaseId=Guid.NewGuid(), WorkflowId=Guid.NewGuid(), ItemId=Guid.NewGuid();
        public bool Block;
        public int Checks;
        public WorkflowStub():base(typeof(IWorkflowAppService)) {}
        public override IMessage Invoke(IMessage message)
        {
            var call=(IMethodCallMessage)message;
            object result=null;
            if(call.MethodName=="FindWorkflow") result=new WorkflowDTO { Id=WorkflowId };
            else if(call.MethodName=="FindWorkflowItem") result=new WorkflowItemDTO { Id=ItemId,WorkflowId=WorkflowId,WorkflowRecordId=CaseId,
                WorkflowSystemPermissionType=(int)SystemPermissionType.BackOfficeLoanAppraisal,Status=(int)WorkflowRecordStatus.Pending,
                RoleName="Appraiser",RequiredApprovals=1,WorkflowRequiredApprovals=1 };
            else if(call.MethodName=="ValidateWorkflowItemMakerChecker")
            {
                Checks++;
                if(Block) return new ReturnMessage(new MakerCheckerViolationException(),call);
            }
            else return new ReturnMessage(new Exception("Unexpected workflow mutation: "+call.MethodName),call);
            return new ReturnMessage(result,null,0,call.LogicalCallContext,call);
        }
    }
    public static void Run()
    {
        var bypass=typeof(WorkflowAppService).GetMethod("IsLocalLoanMakerCheckerBypass",BindingFlags.Static|BindingFlags.NonPublic);
        const string local="Data Source=(local);Initial Catalog=SwiftFinancialsDB_Live;Integrated Security=True";
        foreach(var permission in new[]{SystemPermissionType.BackOfficeLoanAppraisal,SystemPermissionType.BackOfficeLoanApproval,
            SystemPermissionType.BackOfficeLoanAudit,SystemPermissionType.FrontOfficeLoanAppraisal,
            SystemPermissionType.FrontOfficeLoanApproval,SystemPermissionType.FrontOfficeLoanAudit})
            if(!(bool)bypass.Invoke(null,new object[]{"true","SwiftFin_Dev",local,(int)permission}))throw new Exception("Local loan bypass did not activate.");
        foreach(var input in new[]{
            new object[]{"false","SwiftFin_Dev",local,(int)SystemPermissionType.BackOfficeLoanAppraisal},
            new object[]{null,"SwiftFin_Dev",local,(int)SystemPermissionType.BackOfficeLoanAppraisal},
            new object[]{"true","Production",local,(int)SystemPermissionType.BackOfficeLoanAppraisal},
            new object[]{"true","SwiftFin_Dev",local.Replace("(local)","remote-server"),(int)SystemPermissionType.BackOfficeLoanAppraisal},
            new object[]{"true","SwiftFin_Dev",local.Replace("SwiftFinancialsDB_Live","OtherDatabase"),(int)SystemPermissionType.BackOfficeLoanAppraisal},
            new object[]{"true","SwiftFin_Dev","invalid connection",(int)SystemPermissionType.BackOfficeLoanAppraisal},
            new object[]{"true","SwiftFin_Dev",local,0}})
            if((bool)bypass.Invoke(null,input))throw new Exception("Local loan bypass escaped its scope.");
        Console.WriteLine("PASS local single-user switch: six loan stages allowed; disabled, missing, remote, other database/domain and unrelated workflows remain protected");
        var stub=new WorkflowStub();
        var controller=(LoanCaseController)FormatterServices.GetUninitializedObject(typeof(LoanCaseController));
        typeof(LoanCaseController).GetField("_workflowAppService",BindingFlags.Instance|BindingFlags.NonPublic)
            .SetValue(controller,stub.GetTransparentProxy());
        var validate=typeof(LoanCaseController).GetMethod("ValidateFinalLoanStageItem",BindingFlags.Instance|BindingFlags.NonPublic);
        var header=new ServiceHeader {ApplicationUserName="maker",ApplicationUserRoles=new List<string>{"Appraiser"}};
        var args=new object[]{stub.CaseId,stub.ItemId,SystemPermissionType.BackOfficeLoanAppraisal,header,null};
        stub.Block=true;
        bool blocked=false;
        try { validate.Invoke(controller,args); }
        catch(TargetInvocationException ex) { if(!(ex.InnerException is MakerCheckerViolationException))throw; blocked=true; }
        if(!blocked || stub.Checks!=1)throw new Exception("Maker must be rejected by loan-stage preflight.");
        stub.Block=false;
        header.ApplicationUserName="checker";
        if(validate.Invoke(controller,args)!=null || stub.Checks!=2)throw new Exception("Different authorized checker must pass.");
        header.ApplicationUserRoles.Clear();
        if(validate.Invoke(controller,args)==null || stub.Checks!=2)throw new Exception("An unassigned user must still be rejected.");
        Console.WriteLine("PASS loan-stage preflight checks maker/checker before permitting stage execution; assigned role remains required");
    }
}