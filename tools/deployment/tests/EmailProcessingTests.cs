using System;
using System.Configuration;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using SwiftFinancials.AppServiceContainer;
using SwiftFinancials.EmailAlertDispatcher.Configuration;
using Unity;

public class EmailProcessingTests
{
    private class EmailService : RealProxy
    {
        public EmailAlertDTO Record;
        public int Finds, Updates;
        public EmailService() : base(typeof(IEmailAlertAppService)) { }
        public override IMessage Invoke(IMessage message)
        {
            var call = (IMethodCallMessage)message;
            object result;
            if (call.MethodName == "FindEmailAlert") { Finds++; result = Record; }
            else if (call.MethodName == "UpdateEmailAlert") { Updates++; Record = (EmailAlertDTO)call.Args[0]; result = true; }
            else throw new Exception("Unexpected application operation: " + call.MethodName);
            return new ReturnMessage(result, null, 0, call.LogicalCallContext, call);
        }
    }
    private class SmtpService : RealProxy
    {
        public int Sends;
        public SmtpService() : base(typeof(ISmtpService)) { }
        public override IMessage Invoke(IMessage message)
        {
            var call = (IMethodCallMessage)message;
            if (call.MethodName != "SendEmail") throw new Exception("Unexpected SMTP operation.");
            Sends++;
            return new ReturnMessage(null, null, 0, call.LogicalCallContext, call);
        }
    }
    private static void Assert(bool value, string message) { if (!value) throw new Exception(message); }
    private static void Process(EmailMessageProcessor processor, QueueDTO item)
    {
        var task = (Task)typeof(EmailMessageProcessor).GetMethod("Process", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(processor, new object[] { item, (int)MessageCategory.EmailAlert });
        task.GetAwaiter().GetResult();
    }
    private static void ExpectFailure(EmailMessageProcessor processor, QueueDTO item, string fragment)
    {
        try { Process(processor, item); }
        catch (InvalidOperationException ex) { Assert(ex.Message.Contains(fragment), ex.Message); return; }
        throw new Exception("Expected processing failure: " + fragment);
    }
    public static int Main()
    {
        try
        {
            var config = (EmailDispatcherConfigSection)ConfigurationManager.GetSection("emailDispatcherConfiguration");
            var domain = config.EmailDispatcherSettingsItems[0].UniqueId;
            // Bypass the constructor so this unit test never opens/creates an MSMQ queue.
            var processor = (EmailMessageProcessor)FormatterServices.GetUninitializedObject(typeof(EmailMessageProcessor));
            typeof(EmailMessageProcessor).GetField("_emailDispatcherConfigSection", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(processor, config);
            var smtp = new SmtpService();
            typeof(EmailMessageProcessor).GetField("_smtpService", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(processor, smtp.GetTransparentProxy());
            var app = new EmailService();
            Container.Current.RegisterInstance<IEmailAlertAppService>((IEmailAlertAppService)app.GetTransparentProxy());
            ExpectFailure(processor, new QueueDTO { AppDomainName = "unconfigured-test-domain", RecordId = Guid.NewGuid() }, "No email dispatcher configuration");
            Assert(app.Finds == 0 && smtp.Sends == 0, "Mismatched domains must not access the database or SMTP.");
            ExpectFailure(processor, new QueueDTO { AppDomainName = domain, RecordId = Guid.NewGuid() }, "was not found");
            Assert(app.Finds == 1 && smtp.Sends == 0, "Missing records must not send mail.");
            app.Record = new EmailAlertDTO { Id = Guid.NewGuid(), MailMessageDLRStatus = (int)DLRStatus.Pending,
                MailMessageTo = "diagnostic@example.invalid", MailMessageSubject = "Unit test", MailMessageBody = "No real SMTP connection" };
            var item = new QueueDTO { AppDomainName = domain, RecordId = app.Record.Id };
            Process(processor, item);
            Assert(smtp.Sends == 1 && app.Updates == 1 && app.Record.MailMessageDLRStatus == (int)DLRStatus.Delivered, "Successful processing must send and update delivery status.");
            Process(processor, item);
            Assert(smtp.Sends == 1 && app.Updates == 1, "An already delivered record must not be resent.");
            Console.WriteLine("PASS: domain mismatch, missing record, successful delivery update, already-delivered deduplication. No SQL or SMTP operations performed.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
