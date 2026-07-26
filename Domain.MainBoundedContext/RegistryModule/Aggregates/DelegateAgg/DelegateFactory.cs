using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg
{
    public static class DelegateFactory
    {
        public static Delegate CreateDelegate(Guid zoneId, Guid customerId, string remarks)
        {
            var @delegate = new Delegate();

            @delegate.GenerateNewIdentity();

            @delegate.ZoneId = zoneId;

            @delegate.CustomerId = customerId;

            @delegate.Remarks = remarks;

            @delegate.CreatedDate = DateTime.Now;

            return @delegate;
        }
    }
}
