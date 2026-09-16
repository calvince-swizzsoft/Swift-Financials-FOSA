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
        public bool UpdateSucceeds = true;
        public EmailService() : base(typeof(IEmailAlertAppService)) { }
        public override IMessage Invoke(IMessage message)
        {
            var call = (IMethodCallMessage)message;
            object result;
            if (call.MethodName == "GetType") result = typeof(IEmailAlertAppService);
            else if (call.MethodName == "FindEmailAlert") { Finds++; result = Record; }
            else if (call.MethodName == "UpdateEmailAlert") { Updates++; Record = (EmailAlertDTO)call.Args[0]; result = UpdateSucceeds; }
            else throw new Exception("Unexpected application operation: " + call.MethodName);
            return new ReturnMessage(result, null, 0, call.LogicalCallContext, call);
        }
    }
    private class SmtpService : RealProxy
    {
        public int Sends;
        public Exception Failure;
        public SmtpService() : base(typeof(ISmtpService)) { }
        public override IMessage Invoke(IMessage message)
        {
            var call = (IMethodCallMessage)message;
            if (call.MethodName == "GetType") return new ReturnMessage(typeof(ISmtpService), null, 0, call.LogicalCallContext, call);
            if (call.MethodName != "SendEmail") throw new Exception("Unexpected SMTP operation.");
            Sends++;
            if (Failure != null) return new ReturnMessage(Failure, call);
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
            Assert(app.Record.MailMessageDLRStatusDescription == "Sent", "Email success must be labelled Sent.");
            Assert(EnumHelper.GetDescription(DLRStatus.Delivered) == "Delivered", "Shared SMS status must remain Delivered.");
            foreach (var cc in new[] { "", "copy@example.invalid" })
            {
                app.Record = new EmailAlertDTO { Id = Guid.NewGuid(), MailMessageDLRStatus = (int)DLRStatus.Pending, MailMessageCC = cc };
                smtp.Failure = new System.Net.Mail.SmtpException("Simulated SMTP rejection");
                var sends = smtp.Sends;
                Process(processor, item);
                Assert(app.Record.MailMessageDLRStatus == (int)DLRStatus.Failed && app.Record.MailMessageSendRetry == 1, "SMTP errors must persist Failed and attempt count.");
                Process(processor, item);
                Assert(smtp.Sends == sends + 1, "Failed messages must not be automatically resent.");
            }
            app.Record = new EmailAlertDTO { Id = Guid.NewGuid(), MailMessageDLRStatus = (int)DLRStatus.UnKnown };
            smtp.Failure = new FormatException("Invalid recipient");
            Process(processor, item);
            Assert(app.Record.MailMessageDLRStatus == (int)DLRStatus.Failed, "Non-SMTP preparation errors must persist Failed.");
            app.Record = new EmailAlertDTO { Id = Guid.NewGuid(), MailMessageDLRStatus = (int)DLRStatus.Pending };
            app.UpdateSucceeds = false;
            ExpectFailure(processor, item, "Could not save the failed email status");
            smtp.Failure = null;
            app.Record = new EmailAlertDTO { Id = Guid.NewGuid(), MailMessageDLRStatus = (int)DLRStatus.Pending };
            ExpectFailure(processor, item, "Email was sent but its status could not be saved");
            Assert(app.Record.MailMessageDLRStatus == (int)DLRStatus.Delivered, "A status-save failure after SMTP success must not mark sending as Failed.");
            Console.WriteLine("PASS: email status labels, send success, SMTP/format failures, CC path, duplicate suppression, persistence failures, missing domain/record. No SQL or SMTP operations performed.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
