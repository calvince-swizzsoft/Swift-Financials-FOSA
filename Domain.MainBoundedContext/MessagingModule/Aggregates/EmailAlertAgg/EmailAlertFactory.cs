using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.EmailAlertAgg
{
    public static class EmailAlertFactory
    {
        public static EmailAlert CreateEmailAlert(Guid? companyId, MailMessage mailMessage)
        {
            var emailAlert = new EmailAlert();

            emailAlert.GenerateNewIdentity();

            emailAlert.BranchId = (companyId != null && companyId != Guid.Empty) ? companyId : null;

            emailAlert.MailMessage = mailMessage;

            emailAlert.CreatedDate = DateTime.Now;

            return emailAlert;
        }
    }
}
