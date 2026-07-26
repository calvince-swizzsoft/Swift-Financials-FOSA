using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.UnPayReasonAgg
{
    public static class UnPayReasonFactory
    {
        public static UnPayReason CreateUnPayReason(int code, string description)
        {
            var unPayReason = new UnPayReason();

            unPayReason.GenerateNewIdentity();

            unPayReason.Code = (byte)code;

            unPayReason.Description = description;

            unPayReason.CreatedDate = DateTime.Now;

            return unPayReason;
        }
    }
}
