using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveApplicationAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.HolidayAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;

static class LeaveWorkflowTests
{
    static int checks;
    static void Check(bool value, string label) { if (!value) throw new Exception(label); checks++; }
    static void Reject(Action action, string label) { try { action(); } catch (InvalidOperationException) { checks++; return; } throw new Exception(label); }
    class Proxy : RealProxy
    {
        readonly Func<IMethodCallMessage, object> call;
        public Proxy(Type t, Func<IMethodCallMessage, object> f) : base(t) { call=f; }
        public override IMessage Invoke(IMessage m) { var c=(IMethodCallMessage)m; try { return new ReturnMessage(call(c),null,0,c.LogicalCallContext,c); } catch(Exception e) { return new ReturnMessage(e,c); } }
    }
    static object Stub(Type t, Func<IMethodCallMessage,object> f) { return new Proxy(t,f).GetTransparentProxy(); }
    public static void Run()
    {
        var policy=new LeaveTypeDTO { Id=Guid.NewGuid(), Description="Annual", Entitlement=30, UnitType=3, ExcludeWeekends=true, ExcludeHolidays=true };
        var holiday=new HolidayDTO { DurationStartDate=new DateTime(2026,12,24), DurationEndDate=new DateTime(2026,12,26) };
        Check(LeaveCalendar.ChargeDates(new DateTime(2026,12,25),new DateTime(2026,12,25),policy,new[]{holiday}).Count==0,"Overlapping holiday range excluded");
        var entityHoliday=HolidayFactory.CreateHoliday(Guid.NewGuid(),"Holiday",new Duration(holiday.DurationStartDate,holiday.DurationEndDate));
        Check(HolidaySpecifications.HolidayWithinDurationDates(new DateTime(2026,12,25),new DateTime(2026,12,25)).SatisfiedBy().Compile()(entityHoliday),"Holiday repository specification includes overlapping intervals");
        Check(LeaveCalendar.ChargeDates(new DateTime(2026,9,14),new DateTime(2026,9,20),policy,null).Count==5,"Weekends excluded");
        var encoded=LeaveCalendar.Encode(new[]{new DateTime(2026,9,14),new DateTime(2026,9,15)});
        Check(LeaveCalendar.Decode(encoded).Count==2,"Charge snapshots round trip");
        policy.IsAccrued=true;
        Reject(()=>LeaveCalendar.Entitlement(policy,null,new DateTime(2026,9,22)),"Accrual must require commencement");
        Check(LeaveCalendar.Entitlement(policy,new DateTime(2026,12,31),new DateTime(2027,1,1))==0,"New Year does not grant two years of accrual");
        Check(LeaveCalendar.Entitlement(policy,new DateTime(2026,1,1),new DateTime(2027,1,1))==30,"One completed year accrues once");
        policy.UnitType=2;policy.Entitlement=2;
        Check(LeaveCalendar.Entitlement(policy,new DateTime(2026,1,31),new DateTime(2026,2,27))==0,"Incomplete monthly service earns no whole period");
        Check(LeaveCalendar.Entitlement(policy,new DateTime(2026,1,31),new DateTime(2026,2,28))==2,"Month-end anniversary accrues correctly");
        policy.UnitType=3;policy.Entitlement=30;policy.IsAccrued=false;policy.ExcludeWeekends=false;policy.ExcludeHolidays=false;
        var employee=new EmployeeDTO { Id=Guid.NewGuid(), EmploymentStartDate=new DateTime(2026,1,1) };
        var applications=new List<LeaveApplication>();bool failNotification=false;bool deny=false;int locks=0,commits=0,transactions=0;
        var scope=Stub(typeof(IDbContextScope),c=>{if(c.MethodName=="SaveChanges"){commits++;return 1;}return null;});
        var read=Stub(typeof(IDbContextReadOnlyScope),c=>null);
        var constructor=typeof(LeaveApplicationAppService).GetConstructors().Single();
        var args=constructor.GetParameters().Select(p=>Stub(p.ParameterType,c=>{
            switch(p.Name)
            {
                case "dbContextScopeFactory": if(c.MethodName=="CreateReadOnly")return read;Check(c.MethodName=="CreateWithTransaction"&&(System.Data.IsolationLevel)c.Args[0]==System.Data.IsolationLevel.Serializable,"Writes require serializable transaction");transactions++;return scope;
                case "employeeAppService":return employee;
                case "leaveTypeAppService":return policy;
                case "holidayAppService":return new List<HolidayDTO>();
                case "navigationItemInRoleAppService":return deny?new string[0]:new[]{"HR"};
                case "leaveApplicationRepository":
                    if(c.MethodName=="DatabaseSqlQuery"){locks++;return new[]{0};}
                    if(c.MethodName=="AllMatching")return applications.Where(((ISpecification<LeaveApplication>)c.Args[0]).SatisfiedBy().Compile()).ToList();
                    if(c.MethodName=="Get")
                    {
                        var found=applications.SingleOrDefault(x=>x.Id==(Guid)c.Args[0]);
                        if(((MethodInfo)c.MethodBase).IsGenericMethod)return found==null?null:new LeaveApplicationDTO {Id=found.Id,Status=found.Status,NotificationPending=found.NotificationPending};
                        return found;
                    }
                    break;
                case "brokerService":if(failNotification)throw new InvalidOperationException("Simulated notification outage");return true;
            }
            throw new Exception(p.Name+"."+c.MethodName);
        })).ToArray();
        var service=(LeaveApplicationAppService)constructor.Invoke(args);
        var header=new ServiceHeader {ApplicationUserName="approver",ApplicationUserRoles=new List<string>{"HR"}};
        Func<DateTime,DateTime,LeaveApplication> make=(start,end)=>{
            var app=LeaveApplicationFactory.CreateLeaveApplication(employee.Id,policy.Id,new Duration(start,end),"Test",0,null,null,null,null,null);
            app.CreatedBy="applicant";app.Status=2;app.ChargedDates=LeaveCalendar.Encode(LeaveCalendar.ChargeDates(start,end,policy,null));return app;
        };
        var cross=make(new DateTime(2026,12,30),new DateTime(2027,1,4));applications.Add(cross);
        var preview=service.PreviewLeave(employee.Id,policy.Id,new DateTime(2027,1,5),new DateTime(2027,1,6),null,header);
        Check(preview.CanSubmit&&preview.Cycles.Single().Used==4&&preview.Cycles.Single().Remaining==24,"Cross-year leave consumes four days in January");
        applications.Clear();preview=service.PreviewLeave(employee.Id,policy.Id,new DateTime(2026,12,30),new DateTime(2027,1,4),null,header);
        Check(preview.Cycles.Count==2&&preview.Cycles[0].Requested==2&&preview.Cycles[1].Requested==4,"New cross-year request reserves each year's days separately");
        var futureRecall=make(DateTime.Today.AddDays(-1),DateTime.Today.AddDays(3));
        futureRecall.Status=(byte)LeaveApplicationStatus.Recalled;futureRecall.EffectiveReturnDate=DateTime.Today.AddDays(1);
        Check(LeaveApplicationSpecifications.ActiveLeaveApplicationWithEmployeeId(employee.Id).SatisfiedBy().Compile()(futureRecall),"Future recall remains active until return date");
        futureRecall.EffectiveReturnDate=DateTime.Today;
        Check(!LeaveApplicationSpecifications.ActiveLeaveApplicationWithEmployeeId(employee.Id).SatisfiedBy().Compile()(futureRecall),"Leave stops being active on return date");
        var current=make(DateTime.Today.AddDays(-2),DateTime.Today.AddDays(2));applications.Add(current);
        service.RecallLeaveApplication(new LeaveApplicationDTO {Id=current.Id,EffectiveReturnDate=DateTime.Today},header);
        Check(current.Status==8&&current.EffectiveReturnDate==DateTime.Today,"Recall records return date");
        Check(service.FindEmployeeLeaveBalances(employee.Id,policy.Id,header)==28,"Recall retains two days already taken");
        Reject(()=>service.RecallLeaveApplication(new LeaveApplicationDTO {Id=current.Id,EffectiveReturnDate=DateTime.Today},header),"Repeated recall rejected");
        applications.Clear();current=make(DateTime.Today.AddDays(-6),DateTime.Today.AddDays(-2));applications.Add(current);
        Reject(()=>service.RecallLeaveApplication(new LeaveApplicationDTO {Id=current.Id,EffectiveReturnDate=DateTime.Today},header),"Completed leave cannot be recalled");
        applications.Clear();current=make(DateTime.Today.AddDays(2),DateTime.Today.AddDays(3));current.Status=1;applications.Add(current);
        Check(!service.PreviewLeave(employee.Id,policy.Id,current.Duration.StartDate,current.Duration.EndDate,null,header).CanSubmit,"Overlapping pending leave rejected");
        Check(service.PreviewLeave(employee.Id,policy.Id,current.Duration.StartDate,current.Duration.EndDate,current.Id,header).CanSubmit,"Editing excludes own reservation");
        header.ApplicationUserName="applicant";
        Reject(()=>service.AuthorizeLeaveApplication(new LeaveApplicationDTO {Id=current.Id,Status=2},header),"Submitting user cannot approve");
        header.ApplicationUserName="approver";header.ApplicationUserEmployeeId=employee.Id;
        Reject(()=>service.AuthorizeLeaveApplication(new LeaveApplicationDTO {Id=current.Id,Status=2},header),"Employee cannot approve own leave submitted by HR");
        header.ApplicationUserEmployeeId=null;employee.IsLocked=true;
        Reject(()=>service.AuthorizeLeaveApplication(new LeaveApplicationDTO {Id=current.Id,Status=2},header),"Locked employee rejected at approval");
        employee.IsLocked=false;policy.IsLocked=true;
        Reject(()=>service.AuthorizeLeaveApplication(new LeaveApplicationDTO {Id=current.Id,Status=2},header),"Locked type rejected at approval");
        policy.IsLocked=false;failNotification=true;
        Check(service.AuthorizeLeaveApplication(new LeaveApplicationDTO {Id=current.Id,Status=2},header)&&current.Status==2&&current.NotificationPending,"Notification failure does not undo successful approval and remains retryable");
        failNotification=false;Check(service.RetryLeaveNotification(current.Id,header)&&!current.NotificationPending,"Retry clears notification marker after enqueue");
        Reject(()=>service.AuthorizeLeaveApplication(new LeaveApplicationDTO {Id=current.Id,Status=2},header),"Repeated decision rejected");
        applications.Clear();current=make(DateTime.Today.AddDays(4),DateTime.Today.AddDays(5));current.Status=1;applications.Add(current);header.ApplicationUserName="applicant";
        Check(service.WithdrawLeaveApplication(current.Id,header)&&current.Status==16,"Pending leave can be withdrawn by submitter");
        Check(service.FindEmployeeLeaveBalances(employee.Id,policy.Id,header)==30,"Withdrawal releases reserved days");
        deny=true;Reject(()=>service.WithdrawLeaveApplication(current.Id,header),"Role permission enforced");deny=false;
        applications.Clear();current=make(new DateTime(2026,9,14),new DateTime(2026,9,20));current.ChargedDates=LeaveCalendar.Encode(new[]{new DateTime(2026,9,14),new DateTime(2026,9,15)});applications.Add(current);
        Check(service.FindEmployeeLeaveBalances(employee.Id,policy.Id,header)==28,"Historical charged dates preserved despite current calendar policy");
        applications.Clear();
        current=make(new DateTime(2026,12,30),new DateTime(2027,1,4)); applications.Add(current);
        var statistics=service.GetEmployeeLeaveStatistics(employee.Id,policy.Id,new DateTime(2027,1,5),0,header);
        Check(statistics.Balance.Used==4&&statistics.Balance.Available==26&&statistics.History.Single().ChargedDaysInYear==4,"Statistics split cross-year charges and history correctly");
        Check(statistics.TakenDays+statistics.UpcomingDays==4,"Taken and upcoming approved days reconcile to used balance");
        var reservation=make(new DateTime(2027,2,1),new DateTime(2027,2,2));reservation.Status=1;applications.Add(reservation);
        var rejected=make(new DateTime(2027,3,1),new DateTime(2027,3,2));rejected.Status=4;applications.Add(rejected);
        var recalled=make(new DateTime(2027,4,1),new DateTime(2027,4,5));recalled.Status=8;recalled.EffectiveReturnDate=new DateTime(2027,4,3);applications.Add(recalled);
        statistics=service.GetEmployeeLeaveStatistics(employee.Id,policy.Id,new DateTime(2027,4,8),0,header);
        Check(statistics.Balance.Reserved==2&&statistics.Balance.Used==6&&statistics.Balance.Available==22,"Statistics distinguish reservations, partial recall and rejected leave");
        Check(statistics.History.Single(x=>x.Id==rejected.Id).ChargedDaysInYear==0&&statistics.History.Single(x=>x.Id==recalled.Id).ChargedDaysInYear==2,"History retains rejected and recalled records with correct charged days");
        for(int i=0;i<8;i++) applications.Add(make(new DateTime(2027,5,1).AddDays(i),new DateTime(2027,5,1).AddDays(i)));
        statistics=service.GetEmployeeLeaveStatistics(employee.Id,policy.Id,new DateTime(2027,6,1),1,header);
        Check(statistics.HistoryCount==12&&statistics.History.Count==2&&statistics.Balance.Used==14,"History pagination does not truncate balance totals");
        employee.IsLocked=true;policy.IsLocked=true;
        Check(service.GetEmployeeLeaveStatistics(employee.Id,policy.Id,new DateTime(2027,6,1),0,header).Balance!=null,"Historical statistics remain readable for locked records");
        employee.IsLocked=false;policy.IsLocked=false;
        policy.IsAccrued=true;employee.EmploymentStartDate=null;
        statistics=service.GetEmployeeLeaveStatistics(employee.Id,policy.Id,new DateTime(2027,6,1),0,header);
        Check(statistics.Balance==null&&!string.IsNullOrEmpty(statistics.BalanceError)&&statistics.HistoryCount==12,"Missing accrual start date shows an error while preserving history");
        policy.IsAccrued=false;employee.EmploymentStartDate=new DateTime(2026,1,1);
        deny=true;Reject(()=>service.GetEmployeeLeaveStatistics(employee.Id,policy.Id,new DateTime(2027,6,1),0,header),"Statistics require leave permission");deny=false;
        Check(locks>0&&transactions>0&&commits>0,"Workflow uses employee lock and transaction boundaries");
        Console.WriteLine("PASS: "+checks+" leave calendar, balance, recall, approval, withdrawal and notification assertions.");
    }
}
