using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertAgg
{
    public static class TextAlertExtensions
    {
        public static void GenerateNewCustomIdentity(this TextAlert textAlert)
        {
            if (textAlert == null)
                throw new ArgumentNullException(nameof(textAlert));

            var newIdentity = GuidUtility.Create(GuidUtility.IsoOidNamespace, textAlert.GenerateIntegrityHash());

            textAlert.ChangeCurrentIdentity(newIdentity, textAlert.SequentialId, textAlert.CreatedBy, textAlert.CreatedDate);
        }

        public static string GenerateIntegrityHash(this TextAlert textAlert)
        {
            if (textAlert == null)
                throw new ArgumentNullException(nameof(textAlert));

            using (SHA1 sha1 = SHA1.Create())
            {
                var nfi = new NumberFormatInfo();
                nfi.CurrencySymbol = string.Empty;

                var theString = string.Join("|",
                                 textAlert.TextMessage.Recipient,
                                 textAlert.TextMessage.Body,
                                 textAlert.TextMessage.DLRStatus,
                                 textAlert.TextMessage.Origin,
                                 textAlert.TextMessage.Priority,
                                 textAlert.TextMessage.SendRetry,
                                 textAlert.TextMessage.SecurityCritical,
                                 textAlert.CreatedBy,
                                 textAlert.CreatedDate.ToString("dd/MM/yyyy HH:mm"));

                return BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(theString))).Replace("-", String.Empty);
            }
        }
    }
}
