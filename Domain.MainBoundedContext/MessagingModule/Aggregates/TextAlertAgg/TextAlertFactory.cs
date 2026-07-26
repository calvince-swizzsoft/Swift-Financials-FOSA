using Domain.MainBoundedContext.ValueObjects;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertAgg
{
    public static class TextAlertFactory
    {
        public static TextAlert CreateTextAlert(Guid? companyId, TextMessage textMessage, ServiceHeader serviceHeader)
        {
            var textAlert = new TextAlert();

            textAlert.GenerateNewIdentity();

            textAlert.BranchId = (companyId != null && companyId != Guid.Empty) ? companyId : null;

            textAlert.TextMessage = textMessage;

            textAlert.CreatedBy = serviceHeader.ApplicationUserName;

            textAlert.CreatedDate = DateTime.Now;
            
            return textAlert;
        }
    }
}
