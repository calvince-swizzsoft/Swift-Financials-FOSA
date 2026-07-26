using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Application.MainBoundedContext.DTO
{
    public class PasswordSettings
    {
        public bool EnabledPasswordReset { get; set; }

        public bool EnablePasswordRetrieval { get; set; }

        public int MaxInvalidPasswordAttempts { get; set; }

        public int MinRequiredNonAlphanumericCharacters { get; set; }

        public int MinRequiredPasswordLength { get; set; }

        public int PasswordAttemptWindow { get; set; }

        public string PasswordStrengthRegularExpression { get; set; }

        public bool RequiresUniqueEmail { get; set; }

        public bool RequiresQuestionAndAnswer { get; set; }
    }
}
